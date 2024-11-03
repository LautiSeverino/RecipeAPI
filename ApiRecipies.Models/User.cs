using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeAPI.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("userName")]
        public string Username { get; set; }
        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}
