using CollectorsApp.Models.APILogs;

namespace CollectorsApp.Middleware
{
    /// <summary>
    /// Provides extension methods for registering custom middleware components in the application's request pipeline.
    /// </summary>
    /// <remarks>This class contains methods to simplify the addition of middleware components, such as
    /// logging, username extraction,  and problem details handling, to an ASP.NET Core application's middleware
    /// pipeline.</remarks>
    public static class MiddlewareExtentions
    {

        /// <summary>
        /// Creates middleware to log controller actions and outcomes
        ///</summary>
        public static IApplicationBuilder UseControllerAnalyticsMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AnalyticsMiddleware>();
        }
        /// <summary>
        /// Creates middleware to extract the username from the authenticated user's claims
        /// </summary>
        public static IApplicationBuilder UseLoginUsernameExtraction(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoginUsernameExtractionMiddleware>();
        }
        /// <summary>
        /// Creates middleware to handle and format problem details responses
        /// </summary>
        public static IApplicationBuilder UseProblemDetails(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ProblemDetailsMiddleware>();
        }
    }
}
    