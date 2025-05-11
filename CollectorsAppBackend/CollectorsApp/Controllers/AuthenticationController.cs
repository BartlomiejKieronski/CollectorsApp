using CollectorsApp.Models;
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
        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
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
            
            try
            {
                
                var refresh = Request.Cookies["RefreshToken"];
                var device = Request.Cookies["DeviceInfo"];
                
                var result = await _authService.ReauthenticateAsync(refresh!, device!);
                
                if (!result.Success)
                    return Unauthorized(new { error = result.ErrorMessage });

                return Ok();
            }
            catch
            {
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
    }
}
