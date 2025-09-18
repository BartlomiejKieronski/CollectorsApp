using AutoMapper;
using CollectorsApp.Filters;
using CollectorsApp.Models.APILogs;
using CollectorsApp.Models.DTO.APILogs;
using CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectorsApp.Controllers
{
    /// <summary>
    /// Exposes API for reading and managing API logs.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class APILogController : ControllerBase
    {
        private readonly IAPILogRepository _repository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Creates a new <see cref="APILogController"/>.
        /// </summary>
        public APILogController(IAPILogRepository repository, IMapper mapper) 
        {
            _repository = repository;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Queries logs. Currently allowed anonymously for troubleshooting.
        /// </summary>
        [HttpGet]
        [Route("query")]
        [AllowAnonymous]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<APILogResponse>>> QueryComments([FromQuery] APILogFilter entity)
        {
            var items = await _repository.QueryEntity(entity);
            var dto = _mapper.Map<IEnumerable<APILogResponse>>(items);
            return Ok(dto);
        }

        /// <summary>
        /// Returns all logs (admin only).
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<APILogResponse>>> GetAll()
        {
            var items = await _repository.GetAllAsync();
            var dto = _mapper.Map<IEnumerable<APILogResponse>>(items);
            return Ok(dto);
        }

        /// <summary>
        /// Returns a single log by id (admin only).
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<APILogResponse>> GetAPILog(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            var dto = _mapper.Map<APILogResponse>(item);
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new log entry (admin only).
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> PostAPILog(APILog log)
        {
            await _repository.PostAsync(log);
            return CreatedAtAction("Created Succesfully", log);
        }

        /// <summary>
        /// Updates a log entry (admin only).
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> PutAPILog(int id, APILog log)
        {
            await _repository.UpdateAsync(log, id);
            return NoContent();
        }

        /// <summary>
        /// Deletes a log entry (admin only).
        /// </summary>
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
