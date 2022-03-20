using System.Collections.Generic;

namespace coreproject.Model
{
    public class RecipesReturn
    {
        public List<Recipe> Recipes { get; set; }
        public int CurrentPage { get; set; }
        public int NumberOfPages { get; set; }
    }
}