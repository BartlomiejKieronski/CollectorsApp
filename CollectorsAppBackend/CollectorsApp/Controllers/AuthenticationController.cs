using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Authentication;
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
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthenticationController> _logger;
        public AuthenticationController(IAuthService authService, IUserRepository userRepository, ILogger<AuthenticationController> logger)
        {
            _authService = authService;
            _userRepository = userRepository;
            _logger = logger;
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
                    return BadRequest(login.ErrorMessage);
                
                return Ok(login.User);
                
            }
            catch
            {
                return BadRequest("Something went wrong");
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
                return BadRequest("Something went wrong");
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
                    return BadRequest();
                
                await _authService.LogoutAsync(id,refresh, device);
                return Ok();
            }
            catch
            {
                return BadRequest("Something went wrong");
            }
        }
        [AllowAnonymous]
        [Route("Register")]
        [HttpPost]
        public async Task<ActionResult> PostUsers(Users users)
        {
            var result = await _userRepository.PostUser(users);

            if (result == "user exists")
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
