using System.ComponentModel.DataAnnotations;

namespace coreproject.Model
{
    public class RecipeRequest
    {
        public int AccountId { get; set; }
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ListOfIngredients { get; set; }
    }
}