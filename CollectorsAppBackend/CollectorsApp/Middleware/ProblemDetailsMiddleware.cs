namespace CollectorsApp.Middleware
{
    public class ProblemDetailsMiddleware
    {
        private readonly RequestDelegate _next;
        public ProblemDetailsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }

            //handle error acording to RFC 7807 - Problem Details for HTTP APIs
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500; // Internal Server Error
                var problemDetails = new
                {
                    title = "An unexpected error occurred!",
                    status = 500,
                    detail = ex.Message,
                    instance = context.Request.Path
                };
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                };
                var jsonResponse = System.Text.Json.JsonSerializer.Serialize(problemDetails, options);
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
