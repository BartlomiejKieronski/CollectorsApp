namespace CollectorsApp.Services.Cookie
{
    public class CookieService : ICookieService
    {
        private readonly IConfiguration _configuration;
        public CookieService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task AppendAuthTokenCookie(HttpResponse response, string token, int expieryTime = 5)
        {
            var authCookie = new CookieOptions
            {
                HttpOnly = false,
                Expires = DateTime.UtcNow.AddMinutes(expieryTime),
                SameSite = SameSiteMode.None,
                Secure = true
            };

            response.Cookies.Append("AuthToken", token, authCookie);
            return Task.CompletedTask;
        }

        public Task AppendDeviceIdCookie(HttpResponse response, string deviceId, int expieryTime = 10)
        {
            var httpOnlyDeviceCookie = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddYears(expieryTime),
                SameSite = SameSiteMode.None,
                Secure = true
            };

            response.Cookies.Append("DeviceInfo", deviceId, httpOnlyDeviceCookie);
            return Task.CompletedTask;
        }

        public Task AppendRefreshTokenCookie(HttpResponse response, string token, int expieryTime = 3)
        {
            var httpOnlyRefreshCookie = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMonths(expieryTime),
                SameSite = SameSiteMode.None,
                Secure = true
            };
            response.Cookies.Append("RefreshToken", token, httpOnlyRefreshCookie);
            return Task.CompletedTask;
        }
    }
}
