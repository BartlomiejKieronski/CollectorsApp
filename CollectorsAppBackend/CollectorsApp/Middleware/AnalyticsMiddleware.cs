using CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Encryption;
using CollectorsApp.Middleware.Analytics;

namespace CollectorsApp.Middleware
{
    public class AnalyticsMiddleware
    {
        private readonly RequestDelegate _next;

        public AnalyticsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Resolve from the existing request scope to avoid creating a nested scope
            var repository = context.RequestServices.GetRequiredService<IAPILogRepository>();
            var aes = context.RequestServices.GetRequiredService<IAesEncryption>();
            var consent = context.RequestServices.GetRequiredService<IUserConsentRepository>();
            var hash = context.RequestServices.GetRequiredService<IDataHash>();
            var (responseBody, time) = await RequestResponseCapturer.ExecuteAndCaptureResponseAsync(context, _next);

            try
            {
                var log = APILogBuilder.BuildSelection(context, time);

                var (userId, consents) = await UserConsentFetcher.GetUserConsentDataAsync(context, consent);

                log = await ConsentApplier.ApplyConsentAsync(log, userId, aes, consents, responseBody, context, hash);

                log = RoutePathSanitizer.RemoveRouteParamValues(log, context);
                log.Title = $"{log.HttpMethod} {log.RequestPath}";
                await repository.PostAsync(log);
            }
            catch
            {
                // Swallow logging failures silently.
            }
        }
    }
}
