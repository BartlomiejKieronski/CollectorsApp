namespace CollectorsApp.Middleware
{
    /// <summary>
    /// Middleware to handle exceptions and return Problem Details responses according to RFC 7807.
    /// </summary>
    public class ProblemDetailsMiddleware
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// Creates a new <see cref="ProblemDetailsMiddleware"/>.
        /// </summary>
        /// <param name="next">RequestDelegate to continiue app pipline</param>
        public ProblemDetailsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Middleware to handle exceptions and return Problem Details responses acording to RFC - 7807 - Problem Details standard.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
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
