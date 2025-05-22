using Microsoft.AspNetCore.Mvc;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace CollectorsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ImagePathsController : ControllerBase
    {
        private readonly IImagePathRepository _repository;
        private readonly ICollectableItemsRepository _itemsRepository;
        private readonly IAuthorizationService _authorizationService;
        public ImagePathsController(IImagePathRepository repository, ICollectableItemsRepository itemsRepository, IAuthorizationService authorizationService)
        {

            _repository = repository;
            _itemsRepository = itemsRepository;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ImagePath>>> GetImagePaths()
        {
            var data = await _repository.GetImagePaths();
            
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
            return Ok(data);
        }

        // GET: api/ImagePaths/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ImagePath>> GetImagePath(int id)
        {
            var imagePath = await _repository.GetImagePathByImageId(id);

            if (imagePath == null)
            {
                return NotFound();
            }

            return imagePath;
        }
        [HttpGet("GetImagePathsByItemId/{id}/{userId}")]
        [Authorize]
        [Authorize(Policy = "ResourceOwner")]
        public async Task<ActionResult<IEnumerable<ImagePath>>> GetImagePathsByItemId(int id,int userId)
        {
            var validation = await _itemsRepository.GetCollectableItem(id);
            if(validation!=null && validation.OwnerId == userId) { 
                return Ok(await _repository.GetImagePathsByItemId(id));
            }
            else{
                return NoContent();
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutImagePath(int id, ImagePath imagePath)
        {
            if (id != imagePath.Id)
            {
                return BadRequest();
            }
            var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, imagePath, "EntityOwner");
            if (!authorization.Succeeded)
                return Unauthorized();
            await _repository.UpdateImagePath(imagePath,id);
            return NoContent();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ImagePath>> PostImagePath(ImagePath imagePath)
        {
            var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, imagePath, "EntityOwner");
            if (!authorization.Succeeded)
                return Unauthorized();
            await _repository.PostImagePath(imagePath);
            return CreatedAtAction("GetImagePath", new { id = imagePath.Id }, imagePath);
        }

        // DELETE: api/ImagePaths/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteImagePath(int id)
        {
            var data = await _repository.GetImagePathByImageId(id);
            
            if (data == null)
                return NotFound();
            
            var authorization = await _authorizationService.AuthorizeAsync(HttpContext.User, data, "EntityOwner");
            
            if (!authorization.Succeeded)
                return Unauthorized();
            
            await _repository.DeleteImagePath(id);
            
            return NoContent();
        }
        
    }
}
