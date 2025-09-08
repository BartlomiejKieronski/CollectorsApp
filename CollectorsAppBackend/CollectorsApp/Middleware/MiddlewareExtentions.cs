namespace CollectorsApp.Middleware
{
    public static class MiddlewareExtentions
    {
        public static IApplicationBuilder UseControllerLoggingMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ControllerLoggingMiddleware>();
        }
        public static IApplicationBuilder UseLoginUsernameExtraction(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoginUsernameExtractionMiddleware>();
        }
        }
    }
}
    