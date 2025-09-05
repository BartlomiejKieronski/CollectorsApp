using CollectorsApp.Models.APILogs;
using CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Encryption;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CollectorsApp.Middleware
{
    public class ControllerLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;
        public ControllerLoggingMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IAPILogRepository>();
            var aes = scope.ServiceProvider.GetRequiredService<IAesEncryption>();
            var userConsentRepository = scope.ServiceProvider.GetRequiredService<IUserConsentRepository>();

            context.Request.EnableBuffering();

            string requestBody = "";
            if (context.Request.ContentLength > 0 && context.Request.Body.CanSeek)
            {
                context.Request.Body.Position = 0;
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            var originalResponseBody = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();

            responseBody.Position = 0;
            string responseText = await new StreamReader(responseBody).ReadToEndAsync();
            responseBody.Position = 0;

            var log = new APILog
            {
                StatusCode = context.Response.StatusCode,
                IsSuccess = context.Response.StatusCode >= 200 && context.Response.StatusCode < 300,
                RequestPath = context.Request.Path,
                TimeStamp = DateTime.UtcNow,
                DurationMs = (int)stopwatch.ElapsedMilliseconds,
                HttpMethod = context.Request.Method,
                Controller = context.Request.RouteValues["controller"]?.ToString(),
                Action = context.Request.RouteValues["action"]?.ToString(),
                Title = $"{context.Request.Method} {context.Request.Path}"
            };

            var nameId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(nameId, out var userId))
            {
                var userConsent = await userConsentRepository.GetByUserIdAsync(userId);
                if (userConsent != null)
                {
                    foreach (var type in userConsent)
                    {
                        if (type.ConsentType == "IP" && type.IsGranted == true)
                        {
                            // Encrypt IP address if present
                            if (context.Connection.RemoteIpAddress is not null)
                            {
                                var (encryptedIp, ipIv) = await aes.AesEncrypt(context.Connection.RemoteIpAddress.ToString());
                                log.IpAddress = encryptedIp;
                                log.IpIV = ipIv;
                            }
                        }
                        if (type.ConsentType == "UserId" && type.IsGranted == true)
                        {
                            log.UserId = userId;
                        }
                    }
                }
            }

            // Sanitize response for Description
            string sanitizedResponse = responseText;
            if (!string.IsNullOrWhiteSpace(responseText) && context.Response.ContentType?.Contains("application/json") == true)
            {
                try
                {
                    var jsonNode = JsonNode.Parse(responseText);
                    if (jsonNode is JsonObject obj && obj.ContainsKey("ownerId"))
                    {
                        obj.Remove("ownerId");
                        sanitizedResponse = obj.ToJsonString();
                    }
                }
                catch { }
            }
            log.Description = sanitizedResponse;

            // Error message handling
            if (context.Response.StatusCode == 400 && !string.IsNullOrWhiteSpace(responseText))
            {
                try
                {
                    using var doc = JsonDocument.Parse(responseText);
                    if (doc.RootElement.TryGetProperty("error", out var errorElement))
                    {
                        log.ErrorMessage = errorElement.GetString();
                    }
                    else
                    {
                        log.ErrorMessage = responseText;
                    }
                }
                catch
                {
                    log.ErrorMessage = responseText;
                }
            }

            await repository.PostAsync(log);

            responseBody.Position = 0;
            await responseBody.CopyToAsync(originalResponseBody);
        }
    }
}