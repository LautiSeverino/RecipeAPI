using MongoDB.Driver;
using RecipeAPI.Models;
using RecipeAPI.Services.Interface;

namespace RecipeAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;
        public UserService(MongoDBSettings settings)
        {
            if (string.IsNullOrEmpty(settings.UsersCollection))
            {
                throw new ArgumentNullException(nameof(settings.UsersCollection), "UsersCollection no está configurada.");
            }
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.Database);
            _users = database.GetCollection<User>(settings.UsersCollection
         ?? throw new ArgumentNullException(nameof(settings.UsersCollection)));
        }
        public async Task<List<User>> GetUsers()
        {
            return await _users.Find(User => true).ToListAsync();
        }
        public async Task<User> GetUserById(string id)
        {
            return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByUsername(string userName)
        {
            return await _users.Find(user => user.Username == userName).FirstOrDefaultAsync();
        }
        public async Task<User> GetUserByEmail(string email)
        {
            return await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User> Create(User user)
        {
            user.CreatedAt = DateTime.Now;
            await _users.InsertOneAsync(user);
            return user;
        }
        public async Task Update(string id, User userIn)
        {
            await _users.ReplaceOneAsync(user => user.Id == id, userIn);
        }
        public async Task Delete(string id)
        {
            await _users.DeleteOneAsync(user => user.Id == id);
        }

    }
}
