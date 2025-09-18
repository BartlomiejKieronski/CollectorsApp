using Microsoft.AspNetCore.Mvc;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using CollectorsApp.Services.User;
using AutoMapper;
using CollectorsApp.Models.DTO.Users;

namespace CollectorsApp.Controllers
{
    /// <summary>
    /// Manages user entities. Contains endpoints for CRUD and registration-related operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository repository, IUserService userService, IMapper mapper)
        {
            _repository = repository;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
        {
            var items = await _repository.GetAllAsync();
            var dto = _mapper.Map<IEnumerable<UserResponse>>(items);
            return Ok(dto);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserResponse>> GetUsers(int id)
        {
            var users = await _repository.GetByIdAsync(id);

            if (users == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<UserResponse>(users);
            return dto;
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUsers(int id, UserUpdateRequest users)
        {
            var model = _mapper.Map<Users>(users);
            // Ensure the id matches the route id when mapping from partial update
            if (id != model.Id && model.Id != 0)
            {
                return BadRequest(new { error = "Item id does not match" });
            }

            await _repository.UpdateAsync(model,id);
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
