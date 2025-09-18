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
    /// <summary>
    /// API controller for managing admin comments. All endpoints require authorization.
    /// Only users with the "admin" role can access the actions below.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminCommentsController : Controller
    {
        private readonly IAdminCommentRepository _repository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Creates a new instance of <see cref="AdminCommentsController"/>.
        /// </summary>
        /// <param name="repository">Repository used to perform CRUD operations on <see cref="AdminComment"/>.</param>
        public AdminCommentsController(IAdminCommentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Queries admin comments using filter criteria passed via query string.
        /// </summary>
        /// <param name="entity">Filter options bound from the query string.</param>
        /// <returns>A list of comments matching the provided filter.</returns>
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("query")]
        public async Task<ActionResult<IEnumerable<AdminCommentResponse>>> QueryComments([FromQuery] AdminCommentFilter entity)
        {
            var items = await _repository.QueryEntity(entity);
            var dto = _mapper.Map<IEnumerable<AdminCommentResponse>>(items);
            return Ok(dto);
        }

        /// <summary>
        /// Returns all admin comments.
        /// </summary>
        /// <returns>200 OK with the full list of comments.</returns>
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminComment>>> GetAll()
        {
            var items = await _repository.GetAllAsync();
            var dto=_mapper.Map<IEnumerable<AdminCommentResponse>>(items);
            return Ok(dto);
        }

        /// <summary>
        /// Returns a single admin comment by its identifier.
        /// </summary>
        /// <param name="id">The comment identifier.</param>
        /// <returns>200 OK with the comment, if found.</returns>
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminCommentResponse>> GetComment(int id)
        {
            var items = await _repository.GetByIdAsync(id);
            var dto = _mapper.Map<AdminCommentResponse>(items);
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new admin comment.
        /// </summary>
        /// <param name="comment">The comment payload.</param>
        /// <returns>201 Created on success.</returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> PostComment(AdminCommentCreateRequest comment)
        {
            var dto = _mapper.Map<AdminComment>(comment);
            await _repository.PostAsync(dto);
            // Note: CreatedAtAction typically references a GET action and route values to locate the created resource.
            // Here it is used to signal creation without a specific location.
            return  CreatedAtAction("Created Succesfully", comment);
        }

        /// <summary>
        /// Updates an existing admin comment.
        /// </summary>
        /// <param name="id">The comment identifier.</param>
        /// <param name="comment">The updated comment payload.</param>
        /// <returns>204 No Content on success.</returns>
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> PutComment(int id, AdminCommentCreateRequest comment)
        {
            var dto = _mapper.Map<AdminComment>(comment);
            await _repository.UpdateAsync(dto,id);
            return NoContent();
        }

        /// <summary>
        /// Deletes an admin comment by its identifier.
        /// </summary>
        /// <param name="id">The comment identifier.</param>
        /// <returns>204 No Content when deleted; 404 Not Found if the entity does not exist.</returns>
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
