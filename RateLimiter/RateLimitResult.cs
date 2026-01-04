namespace RateLimiter;

/// <summary>
/// Represents the result of a rate limit check.
/// </summary>
public class RateLimitResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the request is allowed.
    /// </summary>
    public bool IsAllowed { get; set; }

    /// <summary>
    /// Gets or sets the remaining number of requests allowed in the current window.
    /// </summary>
    public int Remaining { get; set; }

    /// <summary>
    /// Gets or sets the time when the rate limit will reset.
    /// </summary>
    public DateTime ResetTime { get; set; }

    /// <summary>
    /// Gets or sets the total number of requests allowed in the window.
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// Creates a successful rate limit result.
    /// </summary>
    public static RateLimitResult Success(int remaining, DateTime resetTime, int limit)
    {
        return new RateLimitResult
        {
            IsAllowed = true,
            Remaining = remaining,
            ResetTime = resetTime,
            Limit = limit
        };
    }

    /// <summary>
    /// Creates a failed rate limit result.
    /// </summary>
    public static RateLimitResult Failure(DateTime resetTime, int limit)
    {
        return new RateLimitResult
        {
            IsAllowed = false,
            Remaining = 0,
            ResetTime = resetTime,
            Limit = limit
        };
    }
}
