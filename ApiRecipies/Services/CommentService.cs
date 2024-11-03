using MongoDB.Driver;
using RecipeAPI.Models;
using RecipeAPI.Services.Interface;

namespace RecipeAPI.Services
{
    public class CommentService : ICommentService
    {
        private readonly IMongoCollection<Comment> _comments;
        public CommentService(MongoDBSettings settings)
        {
            if (settings.CommentsCollection == null)
            {
                Console.WriteLine("El nombre de la colección de comentarios no está configurado.");
            }
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.Database);
            _comments = database.GetCollection<Comment>(settings.CommentsCollection ?? throw new ArgumentNullException(nameof(settings.CommentsCollection)));
        }

        public async Task<List<Comment>> GetComments()
        {
            return await _comments.Find(comment => true).ToListAsync();
        }
        public async Task<Comment> GetCommentById(string id)
        {
            return await _comments.Find(comment => comment.Id == id).FirstOrDefaultAsync();
        }
        public async Task<List<Comment>> GetByRecipeId(string recipeId)
        {
            return await _comments.Find(comment => comment.RecipeId == recipeId).ToListAsync();
        }
        public async Task<Comment> Create(Comment comment)
        {
            await _comments.InsertOneAsync(comment);
            return comment;
        }
        public async Task Update(string id, Comment commentIn)
        {
            await _comments.ReplaceOneAsync(comment => comment.Id == id, commentIn);
        }
        public async Task Delete(string id)
        {
            await _comments.DeleteOneAsync(comment => comment.Id == id);   
        }
    }
}
