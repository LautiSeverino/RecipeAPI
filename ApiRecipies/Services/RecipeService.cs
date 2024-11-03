using MongoDB.Bson;
using MongoDB.Driver;
using RecipeAPI.Models;
using RecipeAPI.Services.Interface;

namespace RecipeAPI.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IMongoCollection<Recipe> _recipes;
        public RecipeService(MongoDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.Database);
            _recipes = database.GetCollection<Recipe>(settings.RecipesCollection);
        }
        public async Task<List<Recipe>> GetRecipes()
        {
            return await _recipes.Find(recipe => true).ToListAsync();
        }
        public async Task<Recipe> GetRecipeById(string id)
        {
            return await _recipes.Find(recipe => recipe.Id == id).FirstOrDefaultAsync();
        }
        public async Task<Recipe> Create(Recipe recipe)
        {
            await _recipes.InsertOneAsync(recipe);
            return recipe;
        }
        public async Task Update(string id, Recipe recipeIn)
        {
            await _recipes.ReplaceOneAsync(recipe => recipe.Id == id, recipeIn);
        }
        public async Task Delete(string id)
        {
            await _recipes.DeleteOneAsync(recipe => recipe.Id == id);
        }
        public async Task<List<Recipe>> SearchByName(string name)
        {
            return await _recipes.Find(recipe => recipe.Name.ToLower().Contains(name.ToLower())).ToListAsync();
        }
        public async Task<List<Recipe>> FilterByCategory(string category)
        {
            return await _recipes.Find(recipe => recipe.Category.ToLower().Contains(category.ToLower())).ToListAsync();
        }
        public async Task<List<Recipe>> AdvancedSearch(string? name, string? category, int? prepTime, int? cookTime)
        {
            var filterBuilder = Builders<Recipe>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(name))
            {
                filter &= filterBuilder.Regex("name", new BsonRegularExpression(name, "i"));
            }

            if (!string.IsNullOrEmpty(category))
            {
                filter &= filterBuilder.Regex("category", new BsonRegularExpression($"^{category}$", "i"));
            }

            if (prepTime.HasValue)
            {
                filter &= filterBuilder.Lte(r => r.PreparationTime, prepTime.Value);
            }

            if (cookTime.HasValue)
            {
                filter &= filterBuilder.Lte(r => r.CookingTime, cookTime.Value);
            }

            // 1. Obtén las recetas filtradas
            var recipes = await _recipes.Find(filter).ToListAsync();

            // 2. Obtén los IDs de usuarios únicos de las recetas
            var userIds = recipes.Where(r => !string.IsNullOrEmpty(r.CreatedBy))
                                 .Select(r => r.CreatedBy)
                                 .Distinct()
                                 .ToList();

            // 3. Consulta los usuarios en la base de datos
            var usersCollection = _recipes.Database.GetCollection<User>("UsersCollection"); // Cambia "UsersCollection" por el nombre correcto de la colección de usuarios
            var users = await usersCollection.Find(Builders<User>.Filter.In(u => u.Id, userIds)).ToListAsync();

            // 4. Crear un diccionario de ID de usuario a nombre para acceso rápido
            var userNames = users.ToDictionary(u => u.Id, u => u.Username);

            // 5. Asigna los nombres de usuario a las recetas
            foreach (var recipe in recipes)
            {
                if (userNames.TryGetValue(recipe.CreatedBy, out var userName))
                {
                    recipe.CreatedByName = userName;
                }
            }

            return recipes;
        }

        public async Task AddComment(string recipeId, Comment comment)
        {
            var update = Builders<Recipe>.Update.Push(r => r.Comments, comment);
            await _recipes.UpdateOneAsync(r => r.Id == recipeId, update);
        }

        public async Task DeleteComment(string recipeId, string commentId)
        {
            var update = Builders<Recipe>.Update.PullFilter(r => r.Comments, c => c.Id == commentId);
            await _recipes.UpdateOneAsync(r => r.Id == recipeId, update);
        }

        public async Task<List<Recipe>> GetRecipesWithComments()
        {
            var lookupComments = new BsonDocument
    {
        { "$lookup", new BsonDocument
            {
                { "from", "CommentsCollection" },
                { "localField", "_id" },
                { "foreignField", "recipeId" },
                { "as", "comments" }
            }
        }
    };

            var lookupUsers = new BsonDocument
    {
        { "$lookup", new BsonDocument
            {
                { "from", "UsersCollection" },
                { "localField", "createdBy" }, // Relación con el creador de la receta
                { "foreignField", "_id" },     // Relación con el ID del usuario
                { "as", "userInfo" }
            }
        }
    };

            var addUserNameField = new BsonDocument
    {
        { "$addFields", new BsonDocument
            {
                { "createdBy", "$createdBy" }, // Mantiene el ID original del usuario creador
                { "createdByName", new BsonDocument
                    {
                        { "$arrayElemAt", new BsonArray { "$userInfo.userName", 0 } }
                    }
                }
            }
        }
    };

            var projectFields = new BsonDocument
    {
        { "$project", new BsonDocument
            {
                { "userInfo", 0 } // Excluir el campo "userInfo" del resultado final
            }
        }
    };

            var pipeline = new[] { lookupComments, lookupUsers, addUserNameField, projectFields };

            var recipesWithDocuments = await _recipes.Aggregate<Recipe>(pipeline).ToListAsync();
            return recipesWithDocuments;
        }

    }
}
