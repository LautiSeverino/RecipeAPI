using RecipeAPI.Models;

namespace RecipeAPI.Services.Interface
{
    public interface IUserService
    {
        Task<List<User>> GetUsers();
        Task<User> GetUserById(string id);
        Task<User> GetUserByUsername(string userName);
        Task<User> GetUserByEmail(string email);
        Task<User> Create(User user);
        Task Update(string id,User userIn);
        Task Delete(string id);
    }
}
