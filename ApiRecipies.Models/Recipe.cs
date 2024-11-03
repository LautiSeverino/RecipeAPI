using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeAPI.Models
{
    public class Recipe
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("description")]
        public string Description { get; set; }
        [BsonElement("ingredients")]
        public List<string> Ingredients { get; set; }
        [BsonElement("steps")]
        public List<string> Steps { get; set; }
        [BsonElement("preparationTime")]
        public int PreparationTime { get; set; } //minutes
        [BsonElement("cookingTime")]
        public int CookingTime { get; set;} // minutes
        [BsonElement("servings")]
        public int Servings { get; set; }
        [BsonElement("category")]
        public string Category { get; set; }
        [BsonElement("comments")]
        public List<Comment> Comments { get; set; }
        [BsonElement("createdBy")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CreatedBy { get; set; } // User ID

        [BsonElement("createdByName")]
        public string CreatedByName { get; set; }

        [BsonElement("imageBase64")]
        public string ImageBase64 { get; set; }
       


    }
}
