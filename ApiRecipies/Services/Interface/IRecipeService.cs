using RecipeAPI.Models;

namespace RecipeAPI.Services.Interface
{
    public interface IRecipeService
    {
        Task<List<Recipe>> GetRecipes();
        Task<List<Recipe>> GetRecipesWithComments();
        Task<Recipe> GetRecipeById(string id);
        Task<Recipe> Create(Recipe recipe);
        Task Update(string id, Recipe recipeIn);
        Task Delete(string id);
        Task<List<Recipe>> SearchByName(string name);
        Task<List<Recipe>> FilterByCategory(string category);
        Task<List<Recipe>> AdvancedSearch(string? name, string? category, int? prepTime, int? cookTime);
        Task AddComment(string recipeId, Comment comment);
        Task DeleteComment(string recipeId, string commentId);

    }
}
