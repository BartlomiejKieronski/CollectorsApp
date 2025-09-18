using AutoMapper;
using CollectorsApp.Filters;
using CollectorsApp.Models;
using CollectorsApp.Models.DTO.UserConsent;
using CollectorsApp.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollectorsApp.Controllers
{
    /// <summary>
    /// Handles user consent CRUD and queries with ownership checks.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserConsentController : ControllerBase
    {
        private readonly IUserConsentRepository _repository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Creates a new <see cref="UserConsentController"/>.
        /// </summary>
        public UserConsentController(IUserConsentRepository repository, IAuthorizationService authorizationService, IMapper mapper)
        {
            _repository = repository;
            _authorizationService = authorizationService;
            _mapper = mapper;   

        }
        
        /// <summary>
        /// Returns all consent entries (admin only).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<UserConsentResponse>>> GetAll()
        {
            var items = await _repository.GetAllAsync();
            var dto = _mapper.Map<IEnumerable<UserConsentResponse>>(items);
            return Ok(dto);
        }

        /// <summary>
        /// Queries user consent with optional owner scoping.
        /// </summary>
        [HttpGet("query")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserConsentResponse>>> Query([FromQuery] UserConsentFilter entity)
        {
            if (User.IsInRole("Admin"))
            {
                var items = await _repository.QueryEntity(entity);
                var dto = _mapper.Map<IEnumerable<UserConsentResponse>>(items);
                return Ok(dto);
            }

            if (Request.Query.ContainsKey("OwnerId"))
            {
                var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, entity, "EntityOwner");
                if (!authorization.Succeeded)
                    return Unauthorized(new { error = "User credentials do not match" });

                var items = await _repository.QueryEntity(entity);
                var dto = _mapper.Map<IEnumerable<UserConsentResponse>>(items);
                return Ok(dto);
            }

            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(callerId))
                return Unauthorized();

            entity.OwnerId = int.Parse(callerId);
            var result = await _repository.QueryEntity(entity);
            var mapped = _mapper.Map<IEnumerable<UserConsentResponse>>(result);
            return Ok(mapped);
        }

        /// <summary>
        /// Gets a consent entry by id.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserConsentResponse>> GetById(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            var dto = _mapper.Map<UserConsentResponse>(item);
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new consent entry.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserConsentResponse>> Post(UserConsentCreateOrUpdateRequest entity)
        {
            var model = _mapper.Map<UserConsent>(entity);
            await _repository.PostAsync(model);
            var response = _mapper.Map<UserConsentResponse>(model);
            return CreatedAtAction(nameof(GetById), new { id = model.Id }, response);
        }

        /// <summary>
        /// Updates a consent entry.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Put(int id, UserConsentCreateOrUpdateRequest entity)
        {
            var model = _mapper.Map<UserConsent>(entity);
            await _repository.UpdateAsync(model, id);
            return NoContent();
        }

        /// <summary>
        /// Deletes a consent entry by id.
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
