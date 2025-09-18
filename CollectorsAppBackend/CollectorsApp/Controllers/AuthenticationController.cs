using AutoMapper;
using CollectorsApp.Models;
using CollectorsApp.Models.DTO.Auth;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Authentication;
using CollectorsApp.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CollectorsApp.Controllers
{
    /// <summary>
    /// Handles authentication flows: login, refresh (reauthenticate), logout, and registration.
    /// </summary>
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Creates a new <see cref="AuthenticationController"/>.
        /// </summary>
        public AuthenticationController(IAuthService authService, IUserRepository userRepository, IUserService userService, IMapper mapper)
        {
            _authService = authService;
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>
        /// Performs user login and sets auth cookies. Rate limited via LoginPolicy.
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [EnableRateLimiting("LoginPolicy")]
        public async Task<IActionResult> Login(LoginRequest user)
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

        /// <summary>
        /// Re-authenticates using refresh token and device cookie.
        /// </summary>
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
                {
                    return Unauthorized(new { error = result.ErrorMessage });
                }
                
                return Ok();
            }
            catch
            {
                return BadRequest(new { error = "Something went wrong" });
            }
        }

        /// <summary>
        /// Logs out the user: invalidates refresh token and clears cookies.
        /// </summary>
        [AllowAnonymous]
        [Route("Logout/{id}")]
        [HttpGet]
        public async Task<ActionResult> Logout(int id)
        {
            try
            {
                var refresh = Request.Cookies["RefreshToken"];
                var device = Request.Cookies["DeviceInfo"];
             
                if (string.IsNullOrEmpty(refresh) || string.IsNullOrEmpty(device))
                    return BadRequest(new { error = "Cookies not found" });
                
                await _authService.LogoutAsync(id,refresh, device);
                return Ok();
            }
            catch
            {
                return BadRequest(new { error = "Something went wrong" });
            }
        }

        /// <summary>
        /// Registers a new user using the domain service.
        /// </summary>
        [AllowAnonymous]
        [Route("Register")]
        [HttpPost]
        public async Task<ActionResult> PostUsers(RegisterRequest users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Invalid model state" });
            }

            try
            {
                var dto = _mapper.Map<Users>(users);
                var result = await _userService.RegisterUserAsync(dto);

                if (result == "user exists")
                {
                    return BadRequest(new { error = result });
                }

                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                // Return 400 with error for easier debugging; middleware will log this as well
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
