using System.Text.Json;

namespace CollectorsApp.Middleware
{
    public sealed class LoginUsernameExtractionMiddleware
    {
        private readonly RequestDelegate _next;
        public LoginUsernameExtractionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/api/Authentication"
                && HttpMethods.IsPost(context.Request.Method)
                && !string.IsNullOrEmpty(context.Request.ContentType)
                && context.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase)
                && (context.Request.ContentLength ?? 0) > 0)
            {
                context.Request.EnableBuffering();

                string body;
                using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
                {
                    body = await reader.ReadToEndAsync(context.RequestAborted);
                    context.Request.Body.Position = 0;
                }

                try
                {
                    using var doc = JsonDocument.Parse(body);
                    if (doc.RootElement.TryGetProperty("name", out var nameProp))
                    {
                        context.Items["username"] = nameProp.GetString() ?? "anon";
                    }
                }
                catch (JsonException)
                {
                    // Malformed or non-JSON body: ignore and proceed (rate limiter falls back to "anon")
                }
            }

            await _next(context);
        }
    }
}