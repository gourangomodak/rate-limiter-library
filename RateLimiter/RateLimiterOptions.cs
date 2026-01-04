namespace RateLimiter;

/// <summary>
/// Configuration options for rate limiting.
/// </summary>
public class RateLimiterOptions
{
    /// <summary>
    /// Gets or sets the maximum number of requests allowed per window.
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// Gets or sets the time window duration.
    /// </summary>
    public TimeSpan Window { get; set; }

    /// <summary>
    /// Creates a new instance of RateLimiterOptions.
    /// </summary>
    /// <param name="limit">The maximum number of requests allowed.</param>
    /// <param name="window">The time window duration.</param>
    public RateLimiterOptions(int limit, TimeSpan window)
    {
        if (limit <= 0)
            throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be greater than 0.");
        
        if (window <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(window), "Window must be greater than zero.");

        Limit = limit;
        Window = window;
    }
}
