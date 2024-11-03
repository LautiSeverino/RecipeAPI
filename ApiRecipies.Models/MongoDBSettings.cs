using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeAPI.Models
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public string RecipesCollection { get; set; }
        public string CommentsCollection { get; set; }
        public string UsersCollection { get; set; }
    }
}
