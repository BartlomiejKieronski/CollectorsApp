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
    /// <summary>
    /// CRUD and query endpoints for user preferences. Enforces ownership via authorization service.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserPreferencesController : ControllerBase
    {
        private readonly IUserPreferencesRepository _repository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Creates a new <see cref="UserPreferencesController"/>.
        /// </summary>
        /// <param name="repository">Repository for persistence.</param>
        /// <param name="authorizationService">Service used to check resource ownership.</param>
        /// <param name="mapper">AutoMapper instance.</param>
        public UserPreferencesController(IUserPreferencesRepository repository, IAuthorizationService authorizationService, IMapper mapper)
        {
            _repository = repository;
            _authorizationService = authorizationService;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all preferences (authorized users only).
        /// </summary>
        [HttpGet]
        [Authorize(Roles ="admin")]
        public async Task<ActionResult<IEnumerable<UserPreferencesResponse>>> GetAll()
        {
            var items = await _repository.GetAllAsync();
            var dto = _mapper.Map<IEnumerable<UserPreferencesResponse>>(items);
            return Ok(dto);
        }

        /// <summary>
        /// Queries preferences according to the provided filter.
        /// - Admins can query any owner.
        /// - If OwnerId provided, enforce EntityOwner policy.
        /// - Otherwise, default OwnerId to caller.
        /// </summary>
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

        /// <summary>
        /// Returns a single preference item by id.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize] 
        public async Task<ActionResult<UserPreferencesResponse>> GetById(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            var dto = _mapper.Map<UserPreferencesResponse>(item);
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new preference entry.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserPreferencesResponse>> Post(UserPreferences entity)
        {
            await _repository.PostAsync(entity);
            var dto = _mapper.Map<UserPreferencesResponse>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dto);
        }

        /// <summary>
        /// Updates an existing preference entry.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Put(int id, UserPreferencesUpdateRequest entity)
        {
            var model = _mapper.Map<UserPreferences>(entity);
            await _repository.UpdateAsync(model, id);
            return NoContent();
        }

        /// <summary>
        /// Deletes a preference entry by id.
        /// </summary>
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
