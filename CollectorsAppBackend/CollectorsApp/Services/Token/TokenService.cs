using CollectorsApp.Models.DTO.Auth;
using CollectorsApp.Services.Security;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CollectorsApp.Services.Token
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IGoogleSecretStorageVault _vault;
        public TokenService(IConfiguration configuration, IGoogleSecretStorageVault vault)
        {
            _configuration = configuration;
            _vault = vault;
        }
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
                expires: DateTime.UtcNow.AddMinutes(expires),
                signingCredentials: crudentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var token = new byte[64];
            RandomNumberGenerator.Fill(token);
            return Convert.ToBase64String(token).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        public bool ValidateRefreshTokenDate(DateTime issued, int validDays)
        {
            return issued.AddDays(validDays) > DateTime.UtcNow;
        }
    }
}
