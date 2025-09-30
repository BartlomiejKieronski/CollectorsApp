namespace CollectorsApp.Services.Cookie
{
    /// <summary>
    /// Provides functionality for managing and appending cookies to HTTP responses,  including authentication tokens,
    /// device identifiers, and refresh tokens.
    /// </summary>
    /// <remarks>This service is designed to simplify the process of appending cookies with specific 
    /// configurations, such as expiration times, security settings, and HTTP-only flags.  It supports appending cookies
    /// for authentication, device identification, and token  refreshing, each with customizable expiration
    /// periods.</remarks>
    public class CookieService : ICookieService
    {
        private readonly IConfiguration _configuration;
        /// <summary>
        /// Initializes a new instance of the <see cref="CookieService"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration settings used to initialize the service.</param>
        public CookieService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Appends an authentication token as a cookie to the specified HTTP response.
        /// </summary>
        /// <remarks>The cookie is configured with the following options: <list type="bullet">
        /// <item><description><see cref="CookieOptions.HttpOnly"/> is set to <see
        /// langword="false"/>.</description></item> <item><description><see cref="CookieOptions.SameSite"/> is set to
        /// <see cref="SameSiteMode.None"/>.</description></item> <item><description><see cref="CookieOptions.Secure"/>
        /// is set to <see langword="true"/>.</description></item> </list></remarks>
        /// <param name="response">The <see cref="HttpResponse"/> to which the authentication token cookie will be added.</param>
        /// <param name="token">The authentication token to include in the cookie.</param>
        /// <param name="expieryTime">The expiration time of the cookie, in minutes. Defaults to 5 minutes if not specified.</param>
        /// <returns>A completed <see cref="Task"/> representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Appends a device identifier as an HTTP-only cookie to the specified HTTP response.
        /// </summary>
        /// <remarks>The cookie is configured with the following properties: <list type="bullet">
        /// <item><description>HTTP-only: The cookie is inaccessible to client-side scripts.</description></item>
        /// <item><description>Secure: The cookie is transmitted only over HTTPS.</description></item>
        /// <item><description>SameSite: The cookie is set with <see cref="SameSiteMode.None"/> to allow cross-site
        /// usage.</description></item> </list></remarks>
        /// <param name="response">The <see cref="HttpResponse"/> to which the cookie will be added.</param>
        /// <param name="deviceId">The device identifier to store in the cookie. Cannot be <see langword="null"/> or empty.</param>
        /// <param name="expieryTime">The expiration time of the cookie, in years. Defaults to 10 years.</param>
        /// <returns>A completed <see cref="Task"/> representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Appends a refresh token as an HTTP-only cookie to the specified HTTP response.
        /// </summary>
        /// <remarks>The cookie is configured to be HTTP-only, secure, and use the "None" value for the
        /// SameSite attribute. This ensures the cookie is not accessible via client-side scripts and is suitable for
        /// cross-site requests.</remarks>
        /// <param name="response">The <see cref="HttpResponse"/> to which the refresh token cookie will be added.</param>
        /// <param name="token">The refresh token to be stored in the cookie.</param>
        /// <param name="expieryTime">The expiration time of the cookie, in months. Defaults to 3 months.</param>
        /// <returns>A completed <see cref="Task"/> representing the asynchronous operation.</returns>
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
