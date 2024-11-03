using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeAPI.DTO.Comment;
using RecipeAPI.Models;
using RecipeAPI.Services;
using RecipeAPI.Services.Interface;
using System.Security.Claims;

namespace RecipeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IUserService _userService;
        private readonly IRecipeService _recipeService;

        public CommentsController(ICommentService commentService, IUserService userService, IRecipeService recipeService)
        {
            _commentService = commentService;
            _userService = userService;
            _recipeService = recipeService;
        }
        [HttpGet("GetComments")]
        public async Task<ActionResult<List<Comment>>> GetComments()
        {
            return await _commentService.GetComments();
        }
        // GET: api/Comments/{id}
        [HttpGet("{id:length(24)}", Name = "GetComment")]
        public async Task<ActionResult<Comment>> Get(string id)
        {
            var comment = await _commentService.GetCommentById(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        // GET: api/Comments/byRecipe/{recipeId}
        [HttpGet("byRecipe/{recipeId:length(24)}")]
        public async Task<ActionResult<List<Comment>>> GetByRecipe(string recipeId)
        {
            var comments = await _commentService.GetByRecipeId(recipeId);
            return comments;
        }

        // POST: api/Comments
        [HttpPost("Create")]
        [Authorize]
        public async Task<ActionResult<Comment>> Create([FromBody] CommentCreateRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetUserById(userId);

            if (user == null)
            {
                return Unauthorized();
            }

            // Verificar que la receta exista
            var recipe = await _recipeService.GetRecipeById(request.RecipeId);
            if (recipe == null)
            {
                return BadRequest("Receta no encontrada.");
            }

            var comment = new Comment
            {
                RecipeId = request.RecipeId,
                UserId = user.Id,
                Content = request.Content,
                Date = DateTime.UtcNow
            };

            await _commentService.Create(comment);

            return CreatedAtRoute("GetComment", new { id = comment.Id.ToString() }, comment);
        }

        // PUT: api/Comments/{id}
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, [FromBody] CommentUpdateRequest request)
        {
            var comment = await _commentService.GetCommentById(id);

            if (comment == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (comment.UserId != userId)
            {
                return Forbid();
            }

            comment.Content = request.Content;
            comment.Date = DateTime.UtcNow; // Actualizar la fecha si es necesario

            await _commentService.Update(id, comment);

            return NoContent();
        }

        // DELETE: api/Comments/{id}
        [HttpDelete("{id:length(24)}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var comment = await _commentService.GetCommentById(id);

            if (comment == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (comment.UserId != userId)
            {
                return Forbid();
            }

            await _commentService.Delete(id);

            return NoContent();
        }
    }
}
