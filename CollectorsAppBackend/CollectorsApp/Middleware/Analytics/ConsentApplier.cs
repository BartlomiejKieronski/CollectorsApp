using CollectorsApp.Models;
using CollectorsApp.Models.APILogs;
using CollectorsApp.Services.Encryption;
using System.Text.Json;

namespace CollectorsApp.Middleware.Analytics;

internal static class ConsentApplier
{
    // Size limits to avoid storing excessively large payloads.
    private const int MaxDescriptionLength = 4000; // Arbitrary safety cap.
    private const int MaxErrorMessageLength = 2000; // Matches APILog.ErrorMessage [MaxLength(2000)].

    public static async Task<APILog> ApplyConsentAsync(APILog log, int? userId, IAesEncryption aes, IEnumerable<UserConsent>? consents, string responseText, HttpContext context, IDataHash hash)
    {
        var granted = BuildGrantedConsentSet(consents);

        if (userId.HasValue)
        {
            if (HasConsent(granted, "PersonalIdentifyingData"))
            {
                if (HasConsent(granted, "UserId"))
                {
                    var (uID, uIV) = await aes.AesEncrypt(userId.Value.ToString());
                    log.UserId = uID;
                    log.UserIDIV = uIV;
                    log.UserIdHash = await hash.GenerateHmacAsync(userId.Value.ToString());
                }
                if (HasConsent(granted, "IpAddress"))
                {
                    var ip = context.Connection.RemoteIpAddress?.ToString();

                    if (!string.IsNullOrEmpty(ip))
                    {
                        var (encryptedIp, iv) = await aes.AesEncrypt(ip);
                        log.IpAddress = encryptedIp;
                        log.IpIV = iv;
                    }
                }

                if (!string.IsNullOrEmpty(responseText))
                {
                    log.Description = Truncate(responseText, MaxDescriptionLength);
                }
                else
                {
                    log.Description = string.Empty;
                }
            }
            else
            {
                log.Description = "{}";
            }
            if (context.Response.StatusCode >= 400 && !string.IsNullOrWhiteSpace(responseText))
            {
                if (HasConsent(granted, "ErrorReporting"))
                {
                    var extracted = ExtractErrorMessage(responseText);
                    log.ErrorMessage = Truncate(extracted, MaxErrorMessageLength);
                }
                else
                {
                    log.ErrorMessage = "Error reporting consent not granted"; 
                }
            }
        }

        return log;
    }

    private static HashSet<string> BuildGrantedConsentSet(IEnumerable<UserConsent>? consents)
    {
        if (consents == null) return EmptyHashSet;
        return consents
            .Where(c => c.IsGranted && !string.IsNullOrWhiteSpace(c.ConsentType))
            .Select(c => c.ConsentType!.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static readonly HashSet<string> EmptyHashSet = new(StringComparer.OrdinalIgnoreCase);

    private static bool HasConsent(HashSet<string> granted, string type)
    {
        if (granted.Count == 0 || string.IsNullOrWhiteSpace(type)) return false;
        return granted.Contains(type);
    }

    private static string ExtractErrorMessage(string responseText)
    {
        try
        {
            using var doc = JsonDocument.Parse(responseText);
            if (doc.RootElement.TryGetProperty("error", out var errorElement))
            {
                return errorElement.GetString() ?? responseText;
            }
            if (doc.RootElement.TryGetProperty("title", out var titleElement) && titleElement.ValueKind == JsonValueKind.String)
            {
                return titleElement.GetString() ?? responseText;
            }
            if (doc.RootElement.TryGetProperty("detail", out var detailElement) && detailElement.ValueKind == JsonValueKind.String)
            {
                return detailElement.GetString() ?? responseText;
            }
        }
        catch { /* ignore parsing errors */ }
        return responseText;
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength) return value;
        const string suffix = "...[truncated]";
        var allowed = Math.Max(0, maxLength - suffix.Length);
        return value.Substring(0, allowed) + suffix;
    }
}