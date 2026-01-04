// Generate unit tests using xunit for the SlidingWindowRateLimiter class:
// - Allows up to limit
// - Rejects when limit exceeded
// - Allows after window expires
// - Multiple keys handled independently

public class SlidingWindowRateLimiterTests
{
    [Fact]
    public void TryAcquire_WithinLimit_ShouldAllow()
    {
        // Arrange
        var limiter = new SlidingWindowRateLimiter(TimeSpan.FromMinutes(1), 5);
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
        var limiter = new SlidingWindowRateLimiter(TimeSpan.FromMinutes(1), 3);
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
    public void TryAcquire_AfterWindowExpires_ShouldAllowAgain()
    {
        // Arrange
        var limiter = new SlidingWindowRateLimiter(TimeSpan.FromSeconds(1), 2);
        var key = "test-client";

        // Act - Make requests up to the limit
        for (int i = 0; i < 2; i++)
        {
            limiter.TryAcquire(key);
        }

        // Wait for the window to expire
        Thread.Sleep(1100);

        var result = limiter.TryAcquire(key);

        // Assert
        Assert.True(result.IsAllowed);
        Assert.Equal(1, result.Remaining);
        Assert.Equal(2, result.Limit);
    }

    [Fact]
    public void TryAcquire_MultipleClients_ShouldTrackSeparately()
    {
        // Arrange
        var limiter = new SlidingWindowRateLimiter(TimeSpan.FromMinutes(1), 2);

        // Act
        var result1 = limiter.TryAcquire("client1");
        var result2 = limiter.TryAcquire("client2");

        // Assert
        Assert.True(result1.IsAllowed);
        Assert.True(result2.IsAllowed);
        Assert.Equal(1, result1.Remaining);
        Assert.Equal(1, result2.Remaining);
    }
}