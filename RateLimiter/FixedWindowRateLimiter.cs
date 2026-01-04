using System.Collections.Concurrent;

namespace RateLimiter;

/// <summary>
/// Implements rate limiting using the Fixed Window algorithm.
/// Each time window is independent and resets at fixed intervals.
/// </summary>
public class FixedWindowRateLimiter : IRateLimiter
{
    private readonly RateLimiterOptions _options;
    private readonly ConcurrentDictionary<string, WindowData> _windows;

    public FixedWindowRateLimiter(RateLimiterOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _windows = new ConcurrentDictionary<string, WindowData>();
    }

    public RateLimitResult TryAcquire(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        var now = DateTime.UtcNow;
        var windowData = _windows.AddOrUpdate(
            key,
            _ => new WindowData(now, _options.Window),
            (_, existing) =>
            {
                // Check if we need a new window
                if (now >= existing.WindowEnd)
                {
                    return new WindowData(now, _options.Window);
                }
                return existing;
            });

        lock (windowData.Lock)
        {
            // Double-check if window expired while waiting for lock
            if (now >= windowData.WindowEnd)
            {
                windowData.WindowStart = now;
                windowData.WindowEnd = now.Add(_options.Window);
                windowData.RequestCount = 0;
            }

            if (windowData.RequestCount < _options.Limit)
            {
                windowData.RequestCount++;
                var remaining = _options.Limit - windowData.RequestCount;
                return RateLimitResult.Success(remaining, windowData.WindowEnd, _options.Limit);
            }

            return RateLimitResult.Failure(windowData.WindowEnd, _options.Limit);
        }
    }

    private class WindowData
    {
        public object Lock { get; } = new object();
        public DateTime WindowStart { get; set; }
        public DateTime WindowEnd { get; set; }
        public int RequestCount { get; set; }

        public WindowData(DateTime start, TimeSpan windowDuration)
        {
            WindowStart = start;
            WindowEnd = start.Add(windowDuration);
            RequestCount = 0;
        }
    }
}
