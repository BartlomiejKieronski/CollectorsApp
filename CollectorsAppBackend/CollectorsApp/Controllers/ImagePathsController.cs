using AutoMapper;   
using CollectorsApp.Filters;
using CollectorsApp.Models;
using CollectorsApp.Models.DTO.ImagePaths;
using CollectorsApp.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollectorsApp.Controllers
{
    /// <summary>
    /// Manages image path resources and enforces ownership/authorization.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    
    public class ImagePathsController : ControllerBase
    {
        private readonly IImagePathRepository _repository;
        private readonly ICollectableItemRepository _itemsRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Creates a new <see cref="ImagePathsController"/>.
        /// </summary>
        public ImagePathsController(IImagePathRepository repository, ICollectableItemRepository itemsRepository, IAuthorizationService authorizationService, IMapper mapper)
        {

            _repository = repository;
            _itemsRepository = itemsRepository;
            _authorizationService = authorizationService;
            _mapper = mapper;
        }

        /// <summary>
        /// Query image paths with optional owner scoping.
        /// </summary>
        [HttpGet("query")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ImagePathResponse>>> QueryImagePaths([FromQuery] ImagePathFilter entity)
        {
            
            if (User.IsInRole("admin"))
            {
                var items = await _repository.QueryEntity(entity);
                var dto = _mapper.Map<IEnumerable<ImagePathResponse>>(items);
                return Ok(dto);
            }

            if (Request.Query.ContainsKey("OwnerId"))
            {
                var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, entity, "EntityOwner");
                if (!authorization.Succeeded)
                    return Unauthorized(new { error = "User credentials do not match" });

                var items = await _repository.QueryEntity(entity);
                var dto = _mapper.Map<IEnumerable<ImagePathResponse>>(items);
                return Ok(dto);
            }

            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(callerId))
                return Unauthorized();

            entity.OwnerId = int.Parse(callerId);
            var result = await _repository.QueryEntity(entity);
            var mapped = _mapper.Map<IEnumerable<ImagePathResponse>>(result);
            return Ok(mapped);
            
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ImagePathResponse>>> GetImagePaths()
        {
            var data = await _repository.GetAllAsync();
            
            if (data == null)
            {
                return NoContent();
            }
            foreach (var item in data)
            {
                var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, item, "EntityOwner");
                if (!authorization.Succeeded)
                    return Unauthorized();
            }
            var dto = _mapper.Map<IEnumerable<ImagePathResponse>>(data);
            return Ok(dto);
        }

        /// <summary>
        /// Returns an image path by id (admin only).
        /// </summary>
        // GET: api/ImagePaths/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ImagePathResponse>> GetImagePath(int id)
        {
            var imagePath = await _repository.GetByIdAsync(id);

            if (imagePath == null)
            {
                return NotFound();
            }

            var dto = _mapper.Map<ImagePathResponse>(imagePath);
            return dto;
        }

        /// <summary>
        /// Returns image paths for a specific item (resource-owner restricted).
        /// </summary>
        [HttpGet("GetImagePathsByItemId/{id}/{userId}")]
        [Authorize]
        [Authorize(Policy = "ResourceOwner")]
        public async Task<ActionResult<IEnumerable<ImagePathResponse>>> GetImagePathsByItemId(int id,int userId)
        {
            var validation = await _itemsRepository.GetByIdAsync(id);
            if(validation!=null && validation.OwnerId == userId) { 
                var items = await _repository.GetImagePathsByItemId(id);
                var dto = _mapper.Map<IEnumerable<ImagePathResponse>>(items);
                return Ok(dto);
            }
            else{
                return NoContent();
            }
        }

        /// <summary>
        /// Updates an image path (owner only).
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutImagePath(int id, ImagePathUpdateRequest imagePath)
        {
            if (id != imagePath.Id)
            {
                return BadRequest(new { error = "Item id does not match" });
            }
            var dto = _mapper.Map<ImagePath>(imagePath);
            var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, dto, "EntityOwner");
            if (!authorization.Succeeded)
                return Unauthorized();
            await _repository.UpdateAsync(dto,id);
            return NoContent();
        }

        /// <summary>
        /// Creates a new image path (owner only).
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ImagePathResponse>> PostImagePath(ImagePathCreateRequest imagePath)
        {
            var dto = _mapper.Map<ImagePath>(imagePath);
            var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, dto, "EntityOwner");
            if (!authorization.Succeeded)
                return Unauthorized();
            await _repository.PostAsync(dto);
            var response = _mapper.Map<ImagePathResponse>(dto);
            return CreatedAtAction("GetImagePath", new { id = dto.Id }, response);
        }

        /// <summary>
        /// Deletes an image path (owner only).
        /// </summary>
        // DELETE: api/ImagePaths/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteImagePath(int id)
        {
            var data = await _repository.GetByIdAsync(id);
            
            if (data == null)
                return NotFound();
            
            var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, data, "EntityOwner");
            
            if (!authorization.Succeeded)
                return Unauthorized();
            
            await _repository.DeleteAsync(id);
            
            return NoContent();
        }
        
    }
}
