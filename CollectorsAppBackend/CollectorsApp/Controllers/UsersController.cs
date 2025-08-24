using Microsoft.AspNetCore.Mvc;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace CollectorsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repository;
        
        public UsersController(IUserRepository repository)
        {
            _repository = repository;
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
            var result = await _repository.PostUser(users);

            if (result == "user exists")
            {
                return BadRequest(new { error = result });
            }

            return Ok(result);
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
