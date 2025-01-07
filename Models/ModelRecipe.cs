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
        // Alte proprietăți după nevoie

        // Constructor pentru a inițializa lista de ingrediente
        public ModelRecipe()
        {
            Ingredients = new List<ModelIngredient>();
        }
    }

    
}
