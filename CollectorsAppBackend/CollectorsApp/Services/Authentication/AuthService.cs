using CollectorsApp.Models;
using CollectorsApp.Models.AuthResults;
using CollectorsApp.Models.DTO.Auth;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Cookie;
using CollectorsApp.Services.Encryption;
using CollectorsApp.Services.Token;
using CollectorsApp.Services.Utility;
using Microsoft.IdentityModel.Tokens;
using System.Transactions;

namespace CollectorsApp.Services.Authentication
{
    /// <summary>
    /// Provides authentication services, including user login, logout, and reauthentication.
    /// </summary>
    /// <remarks>The <see cref="AuthService"/> class is responsible for managing authentication-related
    /// operations,  such as validating user credentials, generating and managing tokens, and handling cookies for 
    /// authentication and session management. It relies on various services and repositories to perform  these
    /// operations, including token generation, hashing, and HTTP context access.</remarks>
    public class AuthService : IAuthService
    {
        private readonly IAuthorizationRepository _repository;
        private readonly ITokenService _tokenService;
        private readonly ICookieService _cookieService;
        private readonly IDataHash _dataHash;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// Provides authentication operations such as login, logout, and reauthentication,
        /// using the specified repositories and services.
        /// </summary>
        /// <param name="repo">Authorization repository for storing and retrieving refresh tokens.</param>
        /// <param name="tokenSvc">Service for generating and validating JWT and refresh tokens.</param>
        /// <param name="cookieSvc">Service for managing authentication, refresh, and device cookies.</param>
        /// <param name="dataHash">Service for hashing and verifying user credentials.</param>
        /// <param name="httpContextAccessor">Provides access to the current HTTP context (for reading/writing cookies).</param>
        /// <param name="userRepository">Repository for retrieving user account information.</param>
        public AuthService(IAuthorizationRepository repo, ITokenService tokenSvc, ICookieService cookieSvc, IDataHash dataHash, IHttpContextAccessor httpContextAccessor,IUserRepository userRepository)
        {
            _repository = repo;
            _tokenService = tokenSvc;
            _cookieService = cookieSvc;
            _dataHash = dataHash;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository; 
        }

        /// <summary>
        /// Login a user with provided credentials and device ID. If no device ID is provided, a new one is generated.
        /// </summary>
        /// <param name="loginInfo">Class instance LoginRequest</param>
        /// <param name="deviceId">Device ID</param>
        /// <returns>LoginResult class instance</returns>
        public async Task<LoginResult> LoginAsync(LoginRequest loginInfo, string deviceId)
        {
            
            var user = await _userRepository.GetUserByNameOrEmailAsync(loginInfo);
            if (user == null)
                return LoginResult.Fail("Invalid credentials");

            bool validUser = await _dataHash.RecreateDataAsync(loginInfo.password, user.Salt) == user.Password;
            if (!validUser)
                return LoginResult.Fail("Invalid credentials");

            RefreshTokenInfo rTokenInfo = new RefreshTokenInfo();

            var response = _httpContextAccessor.HttpContext!.Response;

            if (string.IsNullOrEmpty(deviceId))
            {
                GenerateUniqueDeviceId device = new GenerateUniqueDeviceId();
                var issuer = await device.DeviceId();
                await _cookieService.AppendDeviceIdCookie(response, issuer);

                rTokenInfo.RefreshToken = _tokenService.GenerateRefreshToken();
                rTokenInfo.OwnerId = user.Id;
                rTokenInfo.DateOfIssue = DateTime.UtcNow;
                rTokenInfo.IssuerDeviceInfo = issuer;
                rTokenInfo.IsValid = true;

                await _repository.AddRefreshTokenAsync(rTokenInfo);
                await _cookieService.AppendRefreshTokenCookie(response, rTokenInfo.RefreshToken);
            }
            else
            {
                rTokenInfo.OwnerId = user.Id;
                rTokenInfo.RefreshToken = _tokenService.GenerateRefreshToken();
                rTokenInfo.DateOfIssue = DateTime.UtcNow;
                rTokenInfo.IssuerDeviceInfo = deviceId;
                rTokenInfo.IsValid = true;
                await _cookieService.AppendRefreshTokenCookie(response, rTokenInfo.RefreshToken);
                await _repository.AddRefreshTokenAsync(rTokenInfo);

                var refreshTokenInfo = await _repository.GetRefreshTokenByDeviceId(user.Id, deviceId);
                if(refreshTokenInfo != null ) { 
                    refreshTokenInfo.IsValid = false;

                    await _repository.UpdateRefreshTokenAsync(refreshTokenInfo);
                }
            }
            LoggedUserInfo loggedUser = new LoggedUserInfo
            {
                Email = user.Email,
                Role = user.Role,
                Name = user.Name,
                Id = user.Id
            };

            var identity = await _tokenService.GenerateJwtToken(loggedUser, 6);

            await _cookieService.AppendAuthTokenCookie(response, identity);

            return LoginResult.SuccessResult(loggedUser);
        }

        /// <summary>
        /// Logout user by invalidating the refresh token and clearing cookies.
        /// </summary>
        /// <param name="userId">UserID</param>
        /// <param name="refreshToken">Refresh Token string</param>
        /// <param name="deviceInfo">DeviceID string</param>
        public async Task LogoutAsync(int userId, string refreshToken, string deviceInfo)
        {
            var response = _httpContextAccessor.HttpContext!.Response;

            await _cookieService.AppendAuthTokenCookie(response, "", 0);
            await _cookieService.AppendRefreshTokenCookie(response, "", 0);
            await _cookieService.AppendDeviceIdCookie(response, "", 0);
            var token = await _repository.GetRefreshTokenAsync(refreshToken,deviceInfo);
            token.IsValid = false;
            await _repository.UpdateRefreshTokenAsync(token);
        }

        /// <summary>
        /// Reauthenticates a user using a refresh token and device information.
        /// </summary>
        /// <param name="refreshToken">Refresh Token string</param>
        /// <param name="deviceInfo">Device ID string</param>
        /// <returns></returns>
        public async Task<ReauthResult> ReauthenticateAsync(string refreshToken, string deviceInfo)
        {
            if (string.IsNullOrEmpty(deviceInfo) || string.IsNullOrEmpty(refreshToken))
                return ReauthResult.Fail("Unauthorized");

            var token = await _repository.GetRefreshTokenAsync(refreshToken, deviceInfo);
            
            if(token == null)
                return ReauthResult.Fail("Invalid RefreshToken");

            if (token.IsValid == false)
                return ReauthResult.Fail("Invalid RefreshToken");
            var user = await _repository.GetUserByRefreshTokenAsync(token.OwnerId);
            
            if(user==null)
                return ReauthResult.Fail("Unauthorized");
            
            bool validToken = _tokenService.ValidateRefreshTokenDate(token.DateOfIssue, 90);

            if (!validToken)
                return ReauthResult.Fail("refresh token expired");

            string readAuthInfoCookie = await _tokenService.GenerateJwtToken(new LoggedUserInfo { Id = user.Id, Name = user.Name, Email = user.Email, Role = user.Role }, 6);
            
            RefreshTokenInfo rTokenInfo = new RefreshTokenInfo();

            rTokenInfo.RefreshToken = _tokenService.GenerateRefreshToken();
            rTokenInfo.OwnerId = token.OwnerId;
            rTokenInfo.DateOfIssue = DateTime.UtcNow;
            rTokenInfo.IssuerDeviceInfo = deviceInfo;
            rTokenInfo.IsValid = true;
            
            var response = _httpContextAccessor.HttpContext!.Response;

            await _cookieService.AppendRefreshTokenCookie(response, rTokenInfo.RefreshToken);

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await _repository.AddRefreshTokenAsync(rTokenInfo);

                token.IsValid = false;

                await _repository.UpdateRefreshTokenAsync(token);
                scope.Complete();
            }
            
            await _cookieService.AppendAuthTokenCookie(response, readAuthInfoCookie);

            return ReauthResult.SuccessResult();
        }
    }
}