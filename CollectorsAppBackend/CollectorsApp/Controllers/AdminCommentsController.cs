using AutoMapper;
using CollectorsApp.Filters;
using CollectorsApp.Models.Analytics;
using CollectorsApp.Models.DTO.AdminComments;
using CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace CollectorsApp.Controllers
{
    [AllowAnonymous]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminCommentsController : Controller
    {
        private readonly IAdminCommentRepository _repository;
        
        public AdminCommentsController(IAdminCommentRepository repository)
        private readonly IMapper _mapper;
        public AdminCommentsController(IAdminCommentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("query")]
        public async Task<ActionResult<IEnumerable<AdminCommentResponse>>> QueryComments([FromQuery] AdminCommentFilter entity)
        {
            var items = await _repository.QueryEntity(entity);
            var dto = _mapper.Map<IEnumerable<AdminCommentResponse>>(items);
            return Ok(dto);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminComment>>> GetAll()
        {
            var items = await _repository.GetAllAsync();
            var dto=_mapper.Map<IEnumerable<AdminCommentResponse>>(items);
            return Ok(dto);
        }
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminCommentResponse>> GetComment(int id)
        {
            var items = await _repository.GetByIdAsync(id);
            var dto = _mapper.Map<AdminCommentResponse>(items);
            return Ok(dto);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> PostComment(AdminCommentCreateRequest comment)
        {
            var dto = _mapper.Map<AdminComment>(comment);
            await _repository.PostAsync(dto);
            return  CreatedAtAction("Created Succesfully", comment);
        }
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> PutComment(int id, AdminCommentCreateRequest comment)
        {
            var dto = _mapper.Map<AdminComment>(comment);
            await _repository.UpdateAsync(dto,id);
            return NoContent();
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteComment(int id)
        {
            bool isSuccesful = await _repository.DeleteAsync( id);
            if(isSuccesful)
                return NoContent();
            return NotFound();
        }
    }
}
