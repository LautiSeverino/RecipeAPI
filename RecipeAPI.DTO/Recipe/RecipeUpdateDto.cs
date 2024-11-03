using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeAPI.DTO.Recipe
{
    public class RecipeUpdateDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public int PreparationTime { get; set; } // in minutes

        public int CookingTime { get; set; } // in minutes

        public int Servings { get; set; }

        public List<string> Ingredients { get; set; }

        public List<string> Steps { get; set; }
        public string ImageBase64 { get; set; }
    }
}
