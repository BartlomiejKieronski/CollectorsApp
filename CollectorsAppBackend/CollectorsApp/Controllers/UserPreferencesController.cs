using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollectorsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPreferencesController : ControllerBase
    {
        private readonly IUserPreferencesRepository _repository;
        public UserPreferencesController(IUserPreferencesRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserPreferences>>> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<UserPreferences>> GetById(int id)
        {
            return Ok(await _repository.GetByIdAsync(id));
        }
        [HttpPost]
        public async Task<ActionResult> Post(UserPreferences entity)
        {
            await _repository.PostAsync(entity);
            return CreatedAtAction("Created Succesfully", entity);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, UserPreferences entity)
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
