using System.Security.Claims;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CollectorsApp.Middleware.Analytics;

internal static class UserConsentFetcher
{
    public static async Task<(int? userId, IEnumerable<UserConsent>? consents)> GetUserConsentDataAsync(HttpContext context, IUserConsentRepository userConsentRepository)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userId, out var parsedUserId))
        {
            var consent = await userConsentRepository.GetByUserIdAsync(parsedUserId);
            return (parsedUserId, consent);
        }
        return (null, null);
    }
}