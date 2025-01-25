using System.ComponentModel.DataAnnotations;

namespace Licenta2.Models
{
    public class ModelRecipe
    {
        [Key]
        public int RecipeId { get; set; }
        [Required]
        public string RecipeName { get; set; }
        public List<ModelIngredient> Ingredients { get; set; }
        public string Instructions { get; set; }

        public string? ImagePath { get; set; }


        public ModelRecipe()
        {
            Ingredients = new List<ModelIngredient>();
        }
    }

    
}
