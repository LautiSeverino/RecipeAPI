using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecipeAPI.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]  // Agrega este atributo
        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("recipeId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string RecipeId { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }
    }


}