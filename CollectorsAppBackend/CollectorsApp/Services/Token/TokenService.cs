using CollectorsApp.Models.DTO.Auth;
using CollectorsApp.Services.Security;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CollectorsApp.Services.Token
{
    /// <summary>
    /// Provides functionality for generating and validating JSON Web Tokens (JWTs) and refresh tokens.
    /// </summary>
    /// <remarks>This service is designed to handle token-related operations, including the creation of JWTs
    /// with user-specific claims, generation of secure refresh tokens, and validation of refresh token expiration. It
    /// relies on configuration settings and secure storage for cryptographic keys.</remarks>
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IGoogleSecretStorageVault _vault;
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenService"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration settings. Used to retrieve configuration values required by the service.</param>
        /// <param name="vault">The Google Secret Storage Vault instance. Provides secure access to secrets required by the service.</param>
        public TokenService(IConfiguration configuration, IGoogleSecretStorageVault vault)
        {
            _configuration = configuration;
            _vault = vault;
        }

        /// <summary>
        /// Generates a JSON Web Token (JWT) for the specified user with the given expiration time.
        /// </summary>
        /// <remarks>The generated token includes claims for the user's identifier, name, email, and role.
        /// The signing key is retrieved from the configuration or a secret storage vault, and the token is signed using
        /// the HMAC-SHA256 algorithm.</remarks>
        /// <param name="userInfo">The user information to include in the token claims. This must include the user's ID, name, email, and role.</param>
        /// <param name="expires">The expiration time of the token, in minutes, from the current time.</param>
        /// <returns>A string representing the generated JWT.</returns>
        public async Task<string> GenerateJwtToken(LoggedUserInfo userInfo, int expires)
        {
            var key = _configuration["GoogleSecretStorage:Resolved:JWT_KEY"]
                      ?? await _vault.GetSecretsAsync(_configuration["GoogleSecretStorage:Secrets:JWT_KEY"]);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var crudentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
                new Claim(ClaimTypes.Name, userInfo.Name),
                new Claim(ClaimTypes.Email, userInfo.Email),
                new Claim(ClaimTypes.Role, userInfo.Role),
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: crudentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generates a cryptographically secure refresh token.
        /// </summary>
        /// <remarks>The generated token is a URL-safe Base64-encoded string, with padding removed and 
        /// characters '+' and '/' replaced by '-' and '_', respectively. This ensures compatibility  with systems that
        /// require URL-safe tokens.</remarks>
        /// <returns>A URL-safe Base64-encoded string representing the refresh token.</returns>
        public string GenerateRefreshToken()
        {
            var token = new byte[64];
            RandomNumberGenerator.Fill(token);
            return Convert.ToBase64String(token).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        /// <summary>
        /// Determines whether a refresh token is still valid based on its issue date and the allowed validity period.
        /// </summary>
        /// <remarks>The method calculates the expiration date by adding the specified number of valid
        /// days to the issue date. It then compares the expiration date to the current UTC time to determine
        /// validity.</remarks>
        /// <param name="issued">The date and time when the refresh token was issued.</param>
        /// <param name="validDays">The number of days the refresh token remains valid from the issue date.</param>
        /// <returns><see langword="true"/> if the refresh token is still valid; otherwise, <see langword="false"/>.</returns>
        public bool ValidateRefreshTokenDate(DateTime issued, int validDays)
        {
            return issued.AddDays(validDays) > DateTime.UtcNow;
        }
    }
}
