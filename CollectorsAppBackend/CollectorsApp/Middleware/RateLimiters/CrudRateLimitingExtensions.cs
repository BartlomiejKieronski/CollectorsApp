using System.Security.Claims;
using System.Threading.RateLimiting;


namespace CollectorsApp.Middleware.RateLimiters;

/// <summary>
/// Registers rate limiting policies for write operations (POST, PUT, PATCH, DELETE) plus read/query.
/// Only registration; not applied to endpoints yet.
/// </summary>
public static class CrudRateLimitingExtensions
{
    
    public static IServiceCollection AddCrudRateLimiters(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            
            // Do not override global rejection code if already set elsewhere.
            if (options.RejectionStatusCode == 0)
            {
                options.RejectionStatusCode = 429;
            }

            // Generic write policy: limit combined write requests per IP.
            options.AddPolicy("WritePolicy", context =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                string key = $"{userId}:{ip}".GetHashCode().ToString();
                return RateLimitPartition.GetTokenBucketLimiter(key, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 100,          // burst size
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0,
                    ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                    TokensPerPeriod = 50,      // sustain rate
                    AutoReplenishment = true
                });
            });

            // POST policy
            options.AddPolicy("PostPolicy", context =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                string key = $"{userId}:{ip}".GetHashCode().ToString();
                return RateLimitPartition.GetFixedWindowLimiter(key+":POST", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 60,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
            });

            // PUT policy
            options.AddPolicy("PutPolicy", context =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                string key = $"{userId}:{ip}".GetHashCode().ToString();
                return RateLimitPartition.GetFixedWindowLimiter(key+":PUT", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 50,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
            });

            // PATCH policy
            options.AddPolicy("PatchPolicy", context =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                string key = $"{userId}:{ip}".GetHashCode().ToString();
                return RateLimitPartition.GetFixedWindowLimiter(key +":PATCH", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 50,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
            });

            // DELETE policy
            options.AddPolicy("DeletePolicy", context =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                string key = $"{userId}:{ip}".GetHashCode().ToString();
                return RateLimitPartition.GetConcurrencyLimiter(key+":DELETE", _ => new ConcurrencyLimiterOptions
                {
                    PermitLimit = 100, // concurrent deletes per IP
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
            });

            // READ policy (GET) - high throughput but controlled burst.
            options.AddPolicy("ReadPolicy", context =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                string key = $"{userId}:{ip}".GetHashCode().ToString();
                return RateLimitPartition.GetTokenBucketLimiter(key +":READ", _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 600,             // burst
                    TokensPerPeriod = 300,        // sustained per minute
                    ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0,
                    AutoReplenishment = true
                });
            });

            // QUERY policy (expensive GET /search etc.) - stricter sliding window.
            options.AddPolicy("QueryPolicy", context =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                string key = $"{userId}:{ip}".GetHashCode().ToString();
                return RateLimitPartition.GetSlidingWindowLimiter(key +":QUERY", _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 100,              // total allowed in window
                    Window = TimeSpan.FromMinutes(1),
                    SegmentsPerWindow = 6,         // 10s segments
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
            });
        });

        return services;
    }
}
