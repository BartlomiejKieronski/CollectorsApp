using Microsoft.AspNetCore.Mvc;
using CollectorsApp.Models;
using Microsoft.AspNetCore.Authorization;
using CollectorsApp.Repository.Interfaces;

namespace CollectorsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectableItemsController : ControllerBase
    {
        private readonly ICollectableItemsRepository _repository;
        private readonly IImagePathRepository _imagePathRepository;
        private readonly IAuthorizationService _authorizationService;
        public CollectableItemsController(ICollectableItemsRepository repository, IImagePathRepository imagePathRepository, IAuthorizationService authorizationService)
        {
            _repository = repository;
            _imagePathRepository = imagePathRepository;
            _authorizationService = authorizationService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CollectableItems>>> GetCollectableItems()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [Authorize]
        [HttpGet("{page}/{userId}/{collectionId}/{numberOfItems}")]
        [Authorize(Policy = "ResourceOwner")]
        public async Task<ActionResult<IEnumerable<CollectableItems>>> GetSetAmmountItems(int page, int userId, int collectionId, int numberOfItems)
        {
            return Ok(await _repository.GetSetAmmountItems(page,userId,collectionId,numberOfItems));
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CollectableItems>> GetCollectableItems(int id)
        {
            var collectableItems = await _repository.GetByIdAsync(id);

            if (collectableItems == null)
            {
                return NotFound();
            }

            return collectableItems;
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCollectableItems(int id, CollectableItems collectableItems)
        {
            if (id != collectableItems.Id)
            {
                return BadRequest();
            }
            var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, collectableItems, "EntityOwner");
            if(!authorization.Succeeded)
                return Unauthorized();
            await _repository.UpdateAsync(collectableItems, id);
            return NoContent();
        }

        [Authorize]
        [HttpGet]
        [Route("GetCollectableItemsByUserId/{userId}")]
        [Authorize(Policy = "ResourceOwner")]
        public async Task<IEnumerable<CollectableItems>> GetCollectableItemsByUserIdController(int userId)
        {
            return await _repository.GetCollectableItemsByUserId(userId);
        }

        [Authorize]
        [HttpGet]
        [Route("GetCollectableItemsByUserIdAndCollectionId/{userId}/{collectionId}")]
        [Authorize(Policy = "ResourceOwner")]
        public async Task<IEnumerable<CollectableItems>> GetCollectableItemsByUserIdController(int userId,int collectionId)
        {
            return await _repository.GetCollectableItemsByUserIdAndCollectionId(userId, collectionId);
        }

        [HttpGet]
        [Authorize]
        [Route("GetCollectableItemsByUserIdAndItemId/{ItemId}/{userId}")]
        [Authorize(Policy = "ResourceOwner")]
        public async Task<ActionResult<CollectableItems>> GetCollectableItemByUserIdController(int ItemId, int userId)
        {
            var item =  await _repository.GetCollectableItemByUserId(ItemId,userId);
            if (item == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(item);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CollectableItems>> PostCollectableItems(CollectableItems collectableItems)
        {
            var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, collectableItems, "EntityOwner");
            if (!authorization.Succeeded)
                return Unauthorized();
            await _repository.PostAsync(collectableItems);

            if(collectableItems.PhotoFilePath != null)
            {
                ImagePath path = new ImagePath();
                path.ItemId = collectableItems.Id;
                path.Path = collectableItems.PhotoFilePath;
                path.OwnerId = collectableItems.OwnerId;
                await _imagePathRepository.PostAsync(path);
            }

            return CreatedAtAction("GetCollectableItems", new { id = collectableItems.Id }, collectableItems);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCollectableItems(int id)
        {
            var data = await _repository.GetByIdAsync(id);
            if(data != null) { 
                var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, data, "EntityOwner");
                if (!authorization.Succeeded)
                    return Unauthorized();
                await _repository.DeleteAsync(id);
            }
            return NoContent();
        }

        [HttpGet("getData")]
        [Authorize(Policy = "ResourceOwner")]
        public async Task<IActionResult> GetData([FromQuery] int collection, [FromQuery] int userId)
        {
            return Ok(await _repository.GetItemsCount(collection, userId));
        }
    }
}
