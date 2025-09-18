using AutoMapper;
using CollectorsApp.Filters;
using CollectorsApp.Models;
using CollectorsApp.Models.DTO.CollectableItems;
using CollectorsApp.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollectorsApp.Controllers
{
    /// <summary>
    /// Manages collectable items and enforces owner-based authorization.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CollectableItemsController : ControllerBase
    {
        private readonly ICollectableItemRepository _repository;
        private readonly IImagePathRepository _imagePathRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;
        /// <summary>
        /// Creates a new <see cref="CollectableItemsController"/>.
        /// </summary>
        public CollectableItemsController(ICollectableItemRepository repository, IImagePathRepository imagePathRepository, IAuthorizationService authorizationService, IMapper mapper)
        {
            _repository = repository;
            _imagePathRepository = imagePathRepository;
            _authorizationService = authorizationService;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all items (admin only).
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CollectableItemResponse>>> GetCollectableItems()
        {
            var list = await _repository.GetAllAsync();
            var dto = _mapper.Map<IEnumerable<CollectableItemResponse>>(list);
            return Ok(dto);
        }

        /// <summary>
        /// Queries items. Admin can query any owner; others default to caller.
        /// </summary>
        [Authorize]
        [HttpGet("query")]
        public async Task<ActionResult<IEnumerable<CollectableItemResponse>>> QueryCollection([FromQuery] CollectableItemsFilter entity)
        {
            if (User.IsInRole("admin"))
            {
                var items = await _repository.QueryEntity(entity);
                var dto = _mapper.Map<IEnumerable<CollectableItemResponse>>(items);
                return Ok(dto);
            }

            if (Request.Query.ContainsKey("OwnerId"))
            {
                var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, entity, "EntityOwner");
                if (!authorization.Succeeded)
                    return Unauthorized(new { error = "User credentials do not match" });
                
                var items = await _repository.QueryEntity(entity);
                var dto = _mapper.Map<IEnumerable<CollectableItemResponse>>(items);
                return Ok(dto);
            }

            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(callerId))
                return Unauthorized();

            entity.OwnerId = int.Parse(callerId);

            var list = await _repository.QueryEntity(entity);
            var dtObject = _mapper.Map<IEnumerable<CollectableItemResponse>>(list);
            return Ok(dtObject);
        }

        /// <summary>
        /// Returns a page of items for a user and collection (resource-owner enforced).
        /// </summary>
        [Authorize]
        [HttpGet("{page}/{userId}/{collectionId}/{numberOfItems}")]
//        [Authorize(Policy = "ResourceOwner")]
        public async Task<ActionResult<IEnumerable<CollectableItemResponse>>> GetSetAmmountItems(int page, int userId, int collectionId, int numberOfItems)
        {
            //return Ok();
            var items = await _repository.GetSetAmmountItems(page, userId, collectionId, numberOfItems);
            var dto = _mapper.Map<IEnumerable<CollectableItemResponse>>(items);
            return Ok(dto);
        }

        /// <summary>
        /// Gets item by id (admin only).
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CollectableItemResponse>> GetCollectableItems(int id)
        {
            var collectableItems = await _repository.GetByIdAsync(id);

            if (collectableItems == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<CollectableItemResponse>(collectableItems);

            return dto;
        }

        /// <summary>
        /// Updates an item (owner only).
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCollectableItems(int id, CollectableItemUpdateRequest collectableItems)
        {
            var dto = _mapper.Map<CollectableItems>(collectableItems);
            if (id != collectableItems.Id)
            {
                return BadRequest(new { error = "Item id does not match" });
            }
            var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, collectableItems, "EntityOwner");
            if(!authorization.Succeeded)
                return Unauthorized();

            await _repository.UpdateAsync(dto, id);
            return NoContent();
        }

        /// <summary>
        /// Returns items by user id (resource-owner enforced).
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("GetCollectableItemsByUserId/{userId}")]
        [Authorize(Policy = "ResourceOwner")]
        public async Task<ActionResult<IEnumerable<CollectableItemResponse>>> GetCollectableItemsByUserIdController(int userId)
        {
            var items = await _repository.GetCollectableItemsByUserId(userId);
            var dto = _mapper.Map<IEnumerable<CollectableItemResponse>>(items);
            return Ok(dto);
        }

        /// <summary>
        /// Returns items by user id and collection id (resource-owner enforced).
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("GetCollectableItemsByUserIdAndCollectionId/{userId}/{collectionId}")]
        [Authorize(Policy = "ResourceOwner")]
        public async Task<ActionResult<IEnumerable<CollectableItemResponse>>> GetCollectableItemsByUserIdController(int userId,int collectionId)
        {
            var items = await _repository.GetCollectableItemsByUserIdAndCollectionId(userId, collectionId);
            var dto = _mapper.Map<IEnumerable<CollectableItemResponse>>(items);
            return Ok(dto);
        }

        /// <summary>
        /// Returns item by item id and user id (resource-owner enforced).
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("GetCollectableItemsByUserIdAndItemId/{ItemId}/{userId}")]
        [Authorize(Policy = "ResourceOwner")]
        public async Task<ActionResult<CollectableItemResponse>> GetCollectableItemByUserIdController(int ItemId, int userId)
        {
            var item =  await _repository.GetCollectableItemByUserId(ItemId,userId);
            
            if (item == null)
            {
                return NotFound();
            }
            else
            {
                var dto = _mapper.Map<CollectableItemResponse>(item);
                return Ok(dto);
            }
        }

        /// <summary>
        /// Creates a new item and optionally persists its primary photo path.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CollectableItemResponse>> PostCollectableItems(CollectableItemCreateRequest collectableItems)
        {
            var dto = _mapper.Map<CollectableItems>(collectableItems);
            var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, dto, "EntityOwner");
            if (!authorization.Succeeded)
                return Unauthorized();

            await _repository.PostAsync(dto);

            if(collectableItems.PhotoFilePath != null)
            {
                ImagePath path = new ImagePath();
                path.ItemId = dto.Id;
                path.Path = dto.PhotoFilePath;
                path.OwnerId = dto.OwnerId;
                await _imagePathRepository.PostAsync(path);
            }
            var responseDto = _mapper.Map<CollectableItemResponse>(dto);
            return CreatedAtAction("GetCollectableItems", new { id = dto.Id }, responseDto);
        }

        /// <summary>
        /// Deletes an item (owner only).
        /// </summary>
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

        /// <summary>
        /// Returns item count for a collection and user (resource-owner enforced).
        /// </summary>
        [HttpGet("getData")]
        [Authorize(Policy = "ResourceOwner")]
        public async Task<IActionResult> GetData([FromQuery] int collection, [FromQuery] int userId)
        {
            return Ok(await _repository.GetItemsCount(collection, userId));
        }
    }
}
