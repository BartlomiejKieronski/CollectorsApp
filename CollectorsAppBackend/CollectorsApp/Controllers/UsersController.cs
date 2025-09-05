using Microsoft.AspNetCore.Mvc;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using CollectorsApp.Services.User;

namespace CollectorsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IUserService _userService;
        public UsersController(IUserRepository repository, IUserService userService)
        {
            _repository = repository;
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Users>> GetUsers(int id)
        {
            var users = await _repository.GetByIdAsync(id);

            if (users == null)
            {
                return NotFound();
            }
            return users;
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUsers(int id, Users users)
        {
            if (id != users.Id)
            {
                return BadRequest(new { error = "Item id does not match" });
            }

            await _repository.UpdateAsync(users,id);
            return NoContent();
        }

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

        [HttpDelete("{id}")]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> DeleteUsers(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
