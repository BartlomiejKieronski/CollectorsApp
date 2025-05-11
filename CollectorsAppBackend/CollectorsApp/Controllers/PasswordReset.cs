using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Email;
using CollectorsApp.Services.Encryption;
using CollectorsApp.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CollectorsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordReset : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPwdReset _pwdRepository;
        private readonly IEmailSenderService _emailSender;
        private readonly IDataHash _dataHash;
        private readonly IConfiguration _configuration;
        private readonly IGoogleSecretStorageVault _vault;
        public PasswordReset(IUserRepository userRepository, IPwdReset pwdRepository, IEmailSenderService emailSender, IDataHash dataHash, IConfiguration configuration,IGoogleSecretStorageVault vault)
        {
            _userRepository = userRepository;
            _pwdRepository = pwdRepository;
            _emailSender = emailSender;
            _dataHash = dataHash;
            _configuration = configuration;
            _vault = vault;
        }
        [AllowAnonymous]
        [Route("PwdReset")]
        [HttpPost]
        public async Task<ActionResult> ResetPwd(PwdResetModel reset)
        {
            
            var user = await _userRepository.GetUserByNameOrEmailAsync(new LoginInfo() { name = reset.Email });

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(await _vault.GetSecretsAsync("JWT_KEY")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, reset.Email),
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: credentials
                );

            if (user != null)
            {
                var urlToken = new JwtSecurityTokenHandler().WriteToken(token);
                string link = $"http://localhost:3000/ResetPassword/{urlToken}";
            
                string htmlMessage = $@"
                        <html>
                            <body>
                                <div>
                                    <p>Hello,</p>
                                    <p>Kliknij w link poniżej, aby zresetować hasło:</p>
                                    <p><a href='{link}' target='_blank'>Zresetuj hasło</a></p>
                                    <p>Jeśli nie zażądałeś tego emaila, możesz zignorować ten emial.</p>
                                </div>
                            </body>
                        </html>";
                
                PasswordResetModel passwordResetModel = new PasswordResetModel();
                
                passwordResetModel.OwnerId = user.Id;
                passwordResetModel.Token = urlToken;
                passwordResetModel.Email = reset.Email;
             
                await _pwdRepository.PostPasswordReset(passwordResetModel);
                await _emailSender.SendEmailAsync(reset.Email, "Reset hasła", htmlMessage);
                
                return Ok();
            }
            else
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not found",
                    Detail = "Invalid validation data."
                };
                return NotFound(problemDetails);
            }
        }
        [AllowAnonymous]
        [Route("PasswordReset")]
        [HttpPost]
        public async Task<ActionResult> ResetPassword(NewPassword password)
        {
            var email = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepository.GetUserByNameOrEmailAsync(new LoginInfo() { name = email! });
            var validationData = await _pwdRepository.GetPasswordResetModelByEmail(email!);
            
            try
            {
                if (validationData != null && validationData.Token == await _dataHash.GenerateHmacAsync(password.Token) && validationData.OwnerId == user.Id)
                {
                    var newPassword = await _dataHash.GetCredentialsAsync(password.Password);
                    user.Salt = newPassword.Item1;
                    user.Password = newPassword.Item2;
                    await _userRepository.UpdateUser(user, user.Id);
                    await _pwdRepository.DeletePasswordReset(validationData.Id);
                    return Ok("Succes");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return BadRequest("Validation unsuccessful");

        }
        [AllowAnonymous]
        [Route("IsPasswordCorrect")]
        [HttpPost]
        [Authorize]
        [EnableRateLimiting("LoginPolicy")]
        public async Task<IActionResult> IsPasswordCorrect(LoginInfo login)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            
            var data = await _userRepository.GetUserByNameOrEmailAsync(new LoginInfo { name = login.name });
            bool isValid = await _dataHash.RecreateDataAsync(login.password, data.Salt!) == data.Password;

            if (isValid == true)
            {
                return Ok(true);
            }
            return Unauthorized();
        }
    }
}
