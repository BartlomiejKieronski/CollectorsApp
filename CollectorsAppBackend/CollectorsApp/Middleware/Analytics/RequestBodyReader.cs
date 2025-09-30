using System.Text;
using Microsoft.AspNetCore.Http;

namespace CollectorsApp.Middleware.Analytics;

internal static class RequestBodyReader
{
    public static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();
        if (request.ContentLength > 0 && request.Body.CanSeek)
        {
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }
        return string.Empty;
    }
}