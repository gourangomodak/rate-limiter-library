# RateLimiter

> Simple, dependency-free .NET rate limiter library implementing fixed-window and sliding-window algorithms.

## Features

-   Fixed-window limiter: `FixedWindowRateLimiter` ([source](RateLimiter/FixedWindowRateLimiter.cs))
-   Sliding-window limiter: `SlidingWindowRateLimiter` ([source](RateLimiter/SlidingWindowRateLimiter.cs))
-   Common interface: `IRateLimiter` ([source](RateLimiter/IRateLimiter.cs))
-   Config model: `RateLimiterOptions` ([source](RateLimiter/RateLimiterOptions.cs))
-   Result model: `RateLimitResult` ([source](RateLimiter/RateLimitResult.cs))

## Quick start

1. Build the solution:

```bash
dotnet build RateLimiter.sln
```

2. Use the limiter in your code:

```csharp
using RateLimiter;

var options = new RateLimiterOptions(limit: 5, window: TimeSpan.FromMinutes(1));
var fixedLimiter = new FixedWindowRateLimiter(options);

var result = fixedLimiter.TryAcquire("client-1");
if (result.IsAllowed)
{
    // proceed
}
else
{
    // throttle: check result.ResetTime or result.Remaining
}
```

## Running tests

Run unit tests included in the `RateLimiter.Tests` project:

```bash
dotnet test RateLimiter.Tests/RateLimiter.Tests.csproj
```

## Project layout

-   `RateLimiter/` — implementation sources:
    -   `FixedWindowRateLimiter.cs`
    -   `SlidingWindowRateLimiter.cs`
    -   `IRateLimiter.cs`
    -   `RateLimiterOptions.cs`
    -   `RateLimitResult.cs`
-   `RateLimiter.Tests/` — unit tests for each limiter

## Notes

-   The implementations include per-key synchronization and input validation.
-   Intended for simple, in-process rate limiting scenarios. For distributed limits use a centralized store.

## Contributing

PRs and issues are welcome. Keep changes focused and include or update tests.

## License

MIT (add LICENSE file if you want an explicit license)
