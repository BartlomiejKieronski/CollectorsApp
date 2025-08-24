using Microsoft.AspNetCore.Mvc;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using CollectorsApp.Services.Security;

namespace CollectorsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionsController : ControllerBase
    {
        
        private readonly ICollectionRepository _repository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IGoogleSecretStorageVault _vault;
        public CollectionsController(ICollectionRepository repository, IAuthorizationService authorizationService, IGoogleSecretStorageVault vault)
        {
            _repository = repository;
            _authorizationService = authorizationService;
            _vault = vault;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Collections>>> GetCollections()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Collections>> GetCollections(int id)
        {
            var collections = await _repository.GetByIdAsync(id);

            if (collections == null)
            {
                return NotFound();
            }

            return collections;
        }
        
        [HttpGet("GetCollectionsByUserId/{id}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Collections>>> GetCollectionsByUserId(int id)
        {
            var data = await _repository.GetCollectionsByUserId(id);
            if(data ==null) 
                return NoContent();
            
            foreach (var collection in data) { 
                var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, collection, "EntityOwner");
                if (!authorization.Succeeded)
                    return Unauthorized();
            }
            
            return Ok(data);
        }

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

        [HttpGet("GetCollectionsByUserId/{userId}/{name}")]
        [Authorize]
        [Authorize(Policy = "ResourceOwner")]
        public async Task<IEnumerable<Collections>> GetCollectionsByUserId(int userId, string name)
        {
            return await _repository.GetCollectionsByUserId(userId, name);
        }

    }
}
