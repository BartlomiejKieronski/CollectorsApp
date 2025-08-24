using CollectorsApp.Models.APILogs;
using CollectorsApp.Models.Filters;
using CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectorsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APILogController : ControllerBase
    {
        private readonly IAPILogRepository _repository;
        public APILogController(IAPILogRepository repository) 
        {
            _repository = repository;
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("query")]
        public async Task<ActionResult<IEnumerable<APILog>>> QueryComments([FromQuery] APILogFilter entity)
        {
            return Ok(await _repository.QueryEntity(entity));
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<APILog>>> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<APILog>> GetAPILog(int id)
        {
            return Ok(await _repository.GetByIdAsync(id));
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> PostAPILog(APILog log)
        {
            await _repository.PostAsync(log);
            return CreatedAtAction("Created Succesfully", log);
        }
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> PutAPILog(int id, APILog log)
        {
            await _repository.UpdateAsync(log, id);
            return NoContent();
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAPILog(int id)
        {
            bool isSuccesful = await _repository.DeleteAsync(id);
            if (isSuccesful)
                return NoContent();
            return NotFound();
        }
    }
}
