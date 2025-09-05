using CollectorsApp.Filters;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollectorsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserConsentController : ControllerBase
    {
        private readonly IUserConsentRepository _repository;
        public UserConsentController(IUserConsentRepository repository)
        {
            _repository = repository;
        }
        
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<UserConsent>>> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [HttpGet("query")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserConsent>>> Query([FromQuery] UserConsentFilter entity)
        {
            return Ok(await _repository.QueryEntity(entity));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserConsent>> GetById(int id)
        {
            return Ok(await _repository.GetByIdAsync(id));
        }
        [HttpPost]
        public async Task<ActionResult> Post(UserConsent entity)
        {
            await _repository.PostAsync(entity);
            return CreatedAtAction("Created Succesfully", entity);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, UserConsent entity)
        {
            if (id != entity.Id)
            {
                return BadRequest(new { error = "Item id does not match" });
            }
            await _repository.UpdateAsync(entity, id);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            bool isSuccesful = await _repository.DeleteAsync(id);
            if (isSuccesful)
                return NoContent();
            return NotFound();
        }
    }
}
