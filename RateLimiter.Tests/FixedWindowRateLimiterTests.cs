namespace RateLimiter.Tests;

public class FixedWindowRateLimiterTests
{
    [Fact]
    public void TryAcquire_WithinLimit_ShouldAllow()
    {
        // Arrange
        var options = new RateLimiterOptions(limit: 5, window: TimeSpan.FromMinutes(1));
        var limiter = new FixedWindowRateLimiter(options);
        var key = "test-client";

        // Act
        var result = limiter.TryAcquire(key);

        // Assert
        Assert.True(result.IsAllowed);
        Assert.Equal(4, result.Remaining);
        Assert.Equal(5, result.Limit);
    }

    [Fact]
    public void TryAcquire_ExceedingLimit_ShouldDeny()
    {
        // Arrange
        var options = new RateLimiterOptions(limit: 3, window: TimeSpan.FromMinutes(1));
        var limiter = new FixedWindowRateLimiter(options);
        var key = "test-client";

        // Act - Make requests up to the limit
        for (int i = 0; i < 3; i++)
        {
            limiter.TryAcquire(key);
        }

        var result = limiter.TryAcquire(key);

        // Assert
        Assert.False(result.IsAllowed);
        Assert.Equal(0, result.Remaining);
        Assert.Equal(3, result.Limit);
    }

    [Fact]
    public void TryAcquire_MultipleClients_ShouldTrackSeparately()
    {
        // Arrange
        var options = new RateLimiterOptions(limit: 2, window: TimeSpan.FromMinutes(1));
        var limiter = new FixedWindowRateLimiter(options);

        // Act
        var result1 = limiter.TryAcquire("client1");
        var result2 = limiter.TryAcquire("client2");

        // Assert
        Assert.True(result1.IsAllowed);
        Assert.True(result2.IsAllowed);
        Assert.Equal(1, result1.Remaining);
        Assert.Equal(1, result2.Remaining);
    }

    [Fact]
    public void TryAcquire_AfterWindowExpires_ShouldReset()
    {
        // Arrange
        var options = new RateLimiterOptions(limit: 2, window: TimeSpan.FromMilliseconds(100));
        var limiter = new FixedWindowRateLimiter(options);
        var key = "test-client";

        // Act - Exhaust the limit
        limiter.TryAcquire(key);
        limiter.TryAcquire(key);
        var deniedResult = limiter.TryAcquire(key);

        // Wait for window to expire
        Thread.Sleep(150);

        var allowedResult = limiter.TryAcquire(key);

        // Assert
        Assert.False(deniedResult.IsAllowed);
        Assert.True(allowedResult.IsAllowed);
        Assert.Equal(1, allowedResult.Remaining);
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrow()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new FixedWindowRateLimiter(null!));
    }

    [Fact]
    public void TryAcquire_WithNullKey_ShouldThrow()
    {
        // Arrange
        var options = new RateLimiterOptions(limit: 5, window: TimeSpan.FromMinutes(1));
        var limiter = new FixedWindowRateLimiter(options);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => limiter.TryAcquire(null!));
    }

    [Fact]
    public void TryAcquire_WithEmptyKey_ShouldThrow()
    {
        // Arrange
        var options = new RateLimiterOptions(limit: 5, window: TimeSpan.FromMinutes(1));
        var limiter = new FixedWindowRateLimiter(options);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => limiter.TryAcquire(string.Empty));
    }
}
