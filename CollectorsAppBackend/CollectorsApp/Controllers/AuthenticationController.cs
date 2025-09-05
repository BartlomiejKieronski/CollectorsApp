using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Authentication;
using CollectorsApp.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace CollectorsApp.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IUserService _userService;
        public AuthenticationController(IAuthService authService, IUserRepository userRepository, ILogger<AuthenticationController> logger, IUserService userService)
        {
            _authService = authService;
            _logger = logger;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [EnableRateLimiting("LoginPolicy")]
        public async Task<IActionResult> Login(LoginInfo user)
        {
            try
            { 
                string? readDeviceInfoCookie = string.Empty;
                
                if (Request.Cookies["DeviceInfo"] != null)
                {
                    readDeviceInfoCookie = Request.Cookies["DeviceInfo"]?.ToString();
                }
                
                var login = await _authService.LoginAsync(user, readDeviceInfoCookie);
                
                if(!login.Success)
                    return BadRequest(new { error = login.ErrorMessage });
                
                return Ok(login.User);
                
            }
            catch
            {
                return BadRequest(new { error = "Something went wrong" });
            }
        }

        [AllowAnonymous]
        [Route("Reauthenticate")]
        [HttpPost]
        public async Task<ActionResult> Reauthenticate()
        {
            
            _logger.LogInformation("reauthentication start");
            try
            {
                
                var refresh = Request.Cookies["RefreshToken"];
                _logger.LogInformation("old refresh token" + refresh);
                var device = Request.Cookies["DeviceInfo"];
                _logger.LogInformation("device" + device);

                var result = await _authService.ReauthenticateAsync(refresh!, device!);
                _logger.LogInformation("Succes?" + result.Success);
                
                if (!result.Success)
                {
                    _logger.LogInformation(result.ErrorMessage);
                    return Unauthorized(new { error = result.ErrorMessage });
                }
                
                return Ok();
            }
            catch
            {
                _logger.LogInformation("catch reauth controller error");
                return BadRequest(new { error = "Something went wrong" });
            }
        }

        [AllowAnonymous]
        [Route("Logout/{id}")]
        [HttpGet]
        public async Task<ActionResult> Logout(int id)
        {
            try
            {
                var refresh = Request.Cookies["RefreshToken"];
                var device = Request.Cookies["DeviceInfo"];
             
                if (refresh.IsNullOrEmpty()|| device.IsNullOrEmpty())
                    return BadRequest(new { error = "Cookies not found" });
                
                await _authService.LogoutAsync(id,refresh, device);
                return Ok();
            }
            catch
            {
                return BadRequest(new { error = "Something went wrong" });
            }
        }
        [AllowAnonymous]
        [Route("Register")]
        [HttpPost]
        public async Task<ActionResult> PostUsers(Users users)
        {
            var result = await _userService.RegisterUserAsync(users);

            if (result == "user exists")
            {
                return BadRequest(new { error = result });
            }

            return Ok(new { Message = result });
        }
    }
}
