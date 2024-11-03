using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeAPI.DTO.Comment
{
    public class CommentCreateRequest
    {
        public string RecipeId { get; set; }
        public string Content { get; set;}
    }
}
