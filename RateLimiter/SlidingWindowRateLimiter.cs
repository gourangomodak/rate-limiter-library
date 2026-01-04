using System.Collections.Concurrent;
using RateLimiter;

/// <summary>
/// Implements rate limiting using the Sliding Window algorithm.
/// Provides more accurate rate limiting by considering request timestamps
/// across a rolling time window instead of fixed intervals.
/// </summary>
public class SlidingWindowRateLimiter : IRateLimiter
{
    // Implement log-based sliding window rate limiting.
    //
    // For each key:
    // - Track timestamps of requests
    // - Remove timestamps older than (now - window)
    // - Allow request if count < limit
    // - Otherwise reject
    //
    // Edge cases:
    // - First request
    // - Boundary timestamps exactly at window start
    // - Prevent unbounded memory growth
    //
    // Thread safety:
    // - Multiple concurrent callers per key
    //
    // Performance:
    // - Avoid unnecessary allocations
    // - Cleanup must happen before validation
    private readonly TimeSpan _window;
    private readonly int _limit;
    private readonly ConcurrentDictionary<string, RequestLog> _logs;

    public SlidingWindowRateLimiter(TimeSpan window, int limit)
    {
        _window = window;
        _limit = limit;
        _logs = new ConcurrentDictionary<string, RequestLog>();
    }

    public RateLimitResult TryAcquire(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        var now = DateTime.UtcNow;
        var windowStart = now.Subtract(_window);

        var log = _logs.GetOrAdd(key, _ => new RequestLog());

        lock (log.Lock)
        {
            // Remove expired timestamps from the sliding window
            log.Timestamps.RemoveAll(timestamp => timestamp <= windowStart);

            // Check if we can accept this request
            if (log.Timestamps.Count < _limit)
            {
                log.Timestamps.Add(now);
                var remaining = _limit - log.Timestamps.Count;

                // Calculate when the next slot will be available (earliest timestamp + window)
                var successResetTime = log.Timestamps.Count > 0 
                    ? log.Timestamps[0].Add(_window)
                    : now.Add(_window);

                return RateLimitResult.Success(remaining, successResetTime, _limit);
            }

            // Request denied - calculate when the oldest request will expire
            var oldestTimestamp = log.Timestamps[0];
            var failureResetTime = oldestTimestamp.Add(_window);

            return RateLimitResult.Failure(failureResetTime, _limit);
        }
    }

    private class RequestLog
    {
        public object Lock { get; } = new object();
        public List<DateTime> Timestamps { get; } = new List<DateTime>();
    }
}