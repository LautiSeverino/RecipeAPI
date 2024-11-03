using RecipeAPI.Models;

namespace RecipeAPI.Services.Interface
{
    public interface ICommentService
    {
        Task<List<Comment>> GetComments();
        Task<Comment> GetCommentById(string id);
        Task<List<Comment>> GetByRecipeId(string recipeId);
        Task<Comment> Create(Comment comment);
        Task Update(string id, Comment commentIn);
        Task Delete(string id);
    }
}
