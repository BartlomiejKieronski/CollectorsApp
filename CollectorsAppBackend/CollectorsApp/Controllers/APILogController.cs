using AutoMapper;
using CollectorsApp.Filters;
using CollectorsApp.Models.APILogs;
using CollectorsApp.Models.DTO.APILogs;
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
        private readonly IMapper _mapper;
        public APILogController(IAPILogRepository repository, IMapper mapper) 
        {
            _repository = repository;
            _mapper = mapper;
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("query")]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<APILogResponse>>> QueryComments([FromQuery] APILogFilter entity)
        {
            var items = await _repository.QueryEntity(entity);
            var dto = _mapper.Map<IEnumerable<APILogResponse>>(items);
            return Ok(dto);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<APILogResponse>>> GetAll()
        {
            var items = await _repository.GetAllAsync();
            var dto = _mapper.Map<IEnumerable<APILogResponse>>(items);
            return Ok(dto);
        }
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<APILogResponse>> GetAPILog(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            var dto = _mapper.Map<APILogResponse>(item);
            return Ok(dto);
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
