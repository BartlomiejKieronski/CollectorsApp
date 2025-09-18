using AutoMapper;
using CollectorsApp.Filters;
using CollectorsApp.Models;
using CollectorsApp.Models.DTO.UserPreferences;
using CollectorsApp.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollectorsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPreferencesController : ControllerBase
    {
        private readonly IUserPreferencesRepository _repository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;

        public UserPreferencesController(IUserPreferencesRepository repository, IAuthorizationService authorizationService, IMapper mapper)
        {
            _repository = repository;
            _authorizationService = authorizationService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles ="admin")]
        public async Task<ActionResult<IEnumerable<UserPreferencesResponse>>> GetAll()
        {
            var items = await _repository.GetAllAsync();
            var dto = _mapper.Map<IEnumerable<UserPreferencesResponse>>(items);
            return Ok(dto);
        }

        [Authorize]
        [HttpGet("query")]
        public async Task<ActionResult<IEnumerable<UserPreferencesResponse>>> Query([FromQuery] UserPreferencesFilter entity)
        {
            if (User.IsInRole("Admin"))
            {
                var items = await _repository.QueryEntity(entity);
                var dto = _mapper.Map<IEnumerable<UserPreferencesResponse>>(items);
                return Ok(dto);
            }

            if (Request.Query.ContainsKey("OwnerId"))
            {
                var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, entity, "EntityOwner");
                if (!authorization.Succeeded)
                    return Unauthorized(new { error = "User credentials do not match" });

                var items = await _repository.QueryEntity(entity);
                var dto = _mapper.Map<IEnumerable<UserPreferencesResponse>>(items);
                return Ok(dto);
            }

            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(callerId))
                return Unauthorized();

            entity.OwnerId = int.Parse(callerId);
            var result = await _repository.QueryEntity(entity);
            var mapped = _mapper.Map<IEnumerable<UserPreferencesResponse>>(result);
            return Ok(mapped);
        }

        [HttpGet("{id}")]
        [Authorize] 
        public async Task<ActionResult<UserPreferencesResponse>> GetById(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            var dto = _mapper.Map<UserPreferencesResponse>(item);
            return Ok(dto);
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserPreferencesResponse>> Post(UserPreferences entity)
        {
            await _repository.PostAsync(entity);
            var dto = _mapper.Map<UserPreferencesResponse>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dto);
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Put(int id, UserPreferencesUpdateRequest entity)
        {
            var model = _mapper.Map<UserPreferences>(entity);
            await _repository.UpdateAsync(model, id);
            return NoContent();
        }
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            bool isSuccesful = await _repository.DeleteAsync(id);
            if (isSuccesful)
                return NoContent();
            return NotFound();
        }

    }
}
