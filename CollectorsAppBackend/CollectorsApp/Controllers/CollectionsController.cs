using AutoMapper;
using CollectorsApp.Filters;
using CollectorsApp.Models;
using CollectorsApp.Models.DTO.Collections;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollectorsApp.Controllers
{
    /// <summary>
    /// Handles collection CRUD, queries, and owner-scoped access control.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionsController : ControllerBase
    {
        
        private readonly ICollectionRepository _repository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IGoogleSecretStorageVault _vault;
        private readonly IMapper _mapper;
        /// <summary>
        /// Creates a new <see cref="CollectionsController"/>.
        /// </summary>
        public CollectionsController(ICollectionRepository repository, IAuthorizationService authorizationService, IGoogleSecretStorageVault vault, IMapper mapper)
        {
            _repository = repository;
            _authorizationService = authorizationService;
            _vault = vault;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all collections (admin only).
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CollectionResponse>>> GetCollections()
        {
            var items = await _repository.GetAllAsync();
            var dto = _mapper.Map<IEnumerable<CollectionResponse>>(items);
            return Ok(dto);
        }

        /// <summary>
        /// Query collections with optional owner scoping.
        /// </summary>
        [Authorize]
        [HttpGet("query")]

        public async Task<ActionResult<IEnumerable<CollectionResponse>>> QueryCollections([FromQuery] CollectionFilters entity)
        {
            if (User.IsInRole("Admin"))
            {
                var items = await _repository.QueryEntity(entity);
                var dto = _mapper.Map<IEnumerable<CollectionResponse>>(items);
                return Ok(dto);
            }

            if (Request.Query.ContainsKey("OwnerId"))
            {
                var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, entity, "EntityOwner");
                if (!authorization.Succeeded)
                    return Unauthorized(new { error = "User credentials do not match" });
                var items = await _repository.QueryEntity(entity);
                var dto = _mapper.Map<IEnumerable<CollectionResponse>>(items);
                return Ok(dto);
            }

            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(callerId))
                return Unauthorized();

            entity.OwnerId = int.Parse(callerId);
            var list = await _repository.QueryEntity(entity);
            var dtObject = _mapper.Map<IEnumerable<CollectionResponse>>(list);
            return Ok(dtObject);
        }

        /// <summary>
        /// Returns a single collection by id (admin only).
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CollectionResponse>> GetCollections(int id)
        {
            var collections = await _repository.GetByIdAsync(id);

            if (collections == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<CollectionResponse>(collections);
            return dto;
        }
        
        /// <summary>
        /// Returns collections for the provided user (owner-only access per item).
        /// </summary>
        [HttpGet("GetCollectionsByUserId/{id}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CollectionResponse>>> GetCollectionsByUserId(int id)
        {
            var data = await _repository.GetCollectionsByUserId(id);
            if(data ==null) 
                return NoContent();
            
            foreach (var collection in data) { 
                var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, collection, "EntityOwner");
                if (!authorization.Succeeded)
                    return Unauthorized();
            }
            var dto = _mapper.Map<IEnumerable<CollectionResponse>>(data);
            return Ok(dto);
        }

        /// <summary>
        /// Updates a collection (owner only).
        /// </summary>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCollections(int id, Collections collections)
        {
            if (id != collections.Id)
                return BadRequest(new { error = "Item id does not match" });
            

            var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, collections, "EntityOwner");

            if (!authorization.Succeeded)
                return Unauthorized();

            await _repository.UpdateAsync(collections,id);

            return NoContent();
        }

        /// <summary>
        /// Creates a new collection (owner only). Ensures name uniqueness per user.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Collections>> PostCollections(Collections collections)
        {
            var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, collections, "EntityOwner");
            
            if (!authorization.Succeeded)
                return Unauthorized();
            
            var collectionNameExists = await _repository.IsCollectionNameForUserUnique(collections.OwnerId, collections.Name);
            
            if (collectionNameExists == true)
            {    
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Conflict",
                    Detail = "Item with that name already exists."
                };
                return Conflict(problemDetails);
            }
            else { 
            await _repository.PostAsync(collections);
            return CreatedAtAction("GetCollections", new { id = collections.Id }, collections);
            }

        }
        
        /// <summary>
        /// Deletes a collection (owner only).
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCollections(int id)
        {
            var data = await _repository.GetByIdAsync(id);
            
            if(data == null)
                return NotFound();
            
            var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, data, "EntityOwner");
            
            if (!authorization.Succeeded)
                return Unauthorized();
            
            await _repository.DeleteAsync(id);
            
            return Ok();
        }

        /// <summary>
        /// Returns collections by userId and collection name (resource-owner policy enforced).
        /// </summary>
        [HttpGet("GetCollectionsByUserId/{userId}/{name}")]
        [Authorize]
        [Authorize(Policy = "ResourceOwner")]
        public async Task<ActionResult<IEnumerable<CollectionResponse>>> GetCollectionsByUserId(int userId, string name)
        {
            var items =await _repository.GetCollectionsByUserId(userId, name);
            var dto = _mapper.Map<IEnumerable<CollectionResponse>>(items);
            return Ok(dto);
        }

    }
}
