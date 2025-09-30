using CollectorsApp.Models.APILogs;
using Microsoft.AspNetCore.Http;

namespace CollectorsApp.Middleware.Analytics;

internal static class APILogBuilder
{
    public static APILog BuildSelection(HttpContext context, int durationMs)
    {
        return new APILog
        {
            StatusCode = context.Response.StatusCode,
            IsSuccess = context.Response.StatusCode >= 200 && context.Response.StatusCode < 300,
            TimeStamp = DateTime.UtcNow,
            DurationMs = durationMs,
            HttpMethod = context.Request.Method,
            Controller = context.Request.RouteValues["controller"]?.ToString(),
            Action = context.Request.RouteValues["action"]?.ToString(),
        };
    }
}