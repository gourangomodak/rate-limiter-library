namespace RateLimiter;

/// <summary>
/// Interface for rate limiting implementations.
/// </summary>
public interface IRateLimiter
{
    /// <summary>
    /// Attempts to acquire permission to proceed with a request.
    /// </summary>
    /// <param name="key">The key identifying the client or resource being rate limited.</param>
    /// <returns>A RateLimitResult indicating whether the request is allowed.</returns>
    RateLimitResult TryAcquire(string key);
}
