namespace CollectorsApp.Services.Cookie
{
    public interface ICookieService
    {
        Task AppendAuthTokenCookie(HttpResponse response,string token, int expieryTime = 5);
        Task AppendRefreshTokenCookie(HttpResponse response, string token, int expieryTime = 3);
        Task AppendDeviceIdCookie(HttpResponse response, string deviceId, int expieryTime = 10);
    }
}
