using System.Threading.RateLimiting;

namespace CollectorsApp.Middleware.RateLimiters
{
    public static class LoginRateLimitingExtention
    {
        /// <summary>
        /// Middleware to limit login attempts to 10 every 5 minutes per user+IP combination.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddLoginRateLimiter(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = 429;

                options.AddPolicy("LoginPolicy", context =>
                {
                    var username = context.Items["username"] as string ?? "anon";
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    var key = $"{username}|{ip}";

                    return RateLimitPartition.GetSlidingWindowLimiter(
                        key,
                        _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(5),
                            SegmentsPerWindow = 5,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        });
                });
            });

            return services;
        }
    }
}