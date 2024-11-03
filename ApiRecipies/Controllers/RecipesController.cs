using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeAPI.DTO.Recipe;
using RecipeAPI.Models;
using RecipeAPI.Services.Interface;
using System.Security.Claims;

namespace RecipeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly IUserService _userService;
        public RecipesController(IRecipeService recipeService, IUserService userService)
        {
            _recipeService = recipeService;
            _userService = userService;
        }

        [HttpGet("GetRecipesWithComments")]
        public async Task<IActionResult> GetRecipesWithComments()
        {
            try
            {
                var recipes = await _recipeService.GetRecipesWithComments();
                return Ok(recipes);
            }
            catch (Exception ex)
            {
                // Registra el error y devuelve un mensaje de error genérico
                // Puedes usar logs o algo como ILogger para registrar el error
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetRecipes")]
        public async Task<ActionResult<List<Recipe>>> GetRecipes()
        {
            return await _recipeService.GetRecipes();
        }

        [HttpGet("{id:length(24)}", Name = "GetRecipeById")]
        public async Task<ActionResult<Recipe>> GetRecipeById(string id)
        {
            var recipe = await _recipeService.GetRecipeById(id);

            if (recipe == null)
            {
                return NotFound();
            }

            return recipe;
        }

        [HttpPost("Create")]
        [Authorize]
        public async Task<ActionResult<Recipe>> Create([FromBody] RecipeCreateDto recipeDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetUserById(userId);

            if (user == null)
            {
                return NotFound();
            }

            // Procesar la imagen en Base64
            //string imageBase64 = recipeDto.ImageBase64;

            var recipe = new Recipe
            {
                Name = recipeDto.Name,
                Description = recipeDto.Description,
                Category = recipeDto.Category,
                PreparationTime = recipeDto.PreparationTime,
                CookingTime = recipeDto.CookingTime,
                Servings = recipeDto.Servings,
                Ingredients = recipeDto.Ingredients,
                Steps = recipeDto.Steps,
                ImageBase64 = recipeDto.ImageBase64, // Almacenar la imagen como Base64 en la base de datos
                CreatedBy = user.Id,
                Comments = new List<Comment>()
            };

            await _recipeService.Create(recipe);

            return CreatedAtRoute("GetRecipeById", new { id = recipe.Id.ToString() }, recipe);
        }






        [HttpPut("{id:length(24)}", Name = "Update")]
        [Authorize]
        public async Task<IActionResult> Update(string id, RecipeUpdateDto recipeIn)
        {
            var recipe = await _recipeService.GetRecipeById(id);

            if (recipe == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (recipe.CreatedBy != userId)
            {
                return Forbid();
            }

            // Aquí puedes mapear los campos necesarios de recipeIn a recipe
            recipe.Name = recipeIn.Name;
            recipe.Description = recipeIn.Description;
            recipe.Category = recipeIn.Category;
            recipe.PreparationTime = recipeIn.PreparationTime;
            recipe.CookingTime = recipeIn.CookingTime;
            recipe.Servings = recipeIn.Servings;
            recipe.Ingredients = recipeIn.Ingredients;
            recipe.Steps = recipeIn.Steps;
            recipe.ImageBase64 = recipeIn.ImageBase64; // Actualizar la imagen si es necesario
                                                       // No se debe cambiar el CreatedBy
                                                       // recipe.CreatedBy = recipe.CreatedBy; // Esto es redundante, puedes omitirlo

            await _recipeService.Update(id, recipe);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}", Name = "Delete")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var recipe = await _recipeService.GetRecipeById(id);

            if (recipe == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (recipe.CreatedBy != userId)
            {
                return Forbid();
            }

            await _recipeService.Delete(recipe.Id);

            return NoContent();
        }

        [HttpGet("SearchByName")]
        public async Task<ActionResult<List<Recipe>>> SearchByName([FromQuery] string name)
        {
            var recipes = await _recipeService.SearchByName(name);
            return recipes;
        }

        [HttpGet("FilterByCategory")]
        public async Task<ActionResult<List<Recipe>>> FilterByCategory([FromQuery] string category)
        {
            var recipes = await _recipeService.FilterByCategory(category);
            return recipes;
        }

        [HttpGet("AdvancedSearch")]
        public async Task<ActionResult<List<Recipe>>> AdvancedSearch([FromQuery] string? name, [FromQuery] string? category,
            [FromQuery] int? prepTime, [FromQuery] int? cookTime)
        {
            var recipes = await _recipeService.AdvancedSearch(name, category, prepTime, cookTime);
            return recipes;
        }

    }
}
