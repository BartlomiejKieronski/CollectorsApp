using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace CollectorsApp.Middleware.Analytics;

internal static class RequestResponseCapturer
{
    public static async Task<(string responseBody, int durationMs)> ExecuteAndCaptureResponseAsync(HttpContext context, RequestDelegate next)
    {
        var originalBodyStream = context.Response.Body;

        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        var stopwatch = Stopwatch.StartNew();
        string captured = string.Empty;
        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            // Read response body as string
            captured = await new StreamReader(responseBody, Encoding.UTF8, leaveOpen: true).ReadToEndAsync();

            // Reset position and copy to original stream
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }

        return (captured, (int)stopwatch.ElapsedMilliseconds);
    }
}
