using CollectorsApp.Models.Analytics;
using CollectorsApp.Models.Filters;
using CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectorsApp.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminCommentsController : Controller
    {
        private readonly IAdminCommentsRepository _repository;
        
        public AdminCommentsController(IAdminCommentsRepository repository)
        {
            _repository = repository;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("query")]
        public async Task<ActionResult<IEnumerable<AdminComment>>> QueryComments([FromQuery] AdminCommentFilter entity)
        {
            return Ok(await _repository.QueryEntity(entity));
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminComment>>> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminComment>> GetComment(int id)
        {
            return Ok(await _repository.GetByIdAsync(id));
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> PostComment(AdminComment comment)
        {
            await _repository.PostAsync(comment);
            return  CreatedAtAction("Created Succesfully", comment);
        }
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> PutComment(int id, AdminComment comment)
        {
            await _repository.UpdateAsync(comment,id);
            return NoContent();
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteComment(int id)
        {
            bool isSuccesful = await _repository.DeleteAsync( id);
            if(isSuccesful)
                return NoContent();
            return BadRequest();
        }
    }
}
