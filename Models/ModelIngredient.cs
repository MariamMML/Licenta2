using System.ComponentModel.DataAnnotations;

namespace Licenta2.Models
{
    public class ModelIngredient
    {
        [Key]
        public int IngredientId { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public string Weight { get; set; } // Updated from 'Unit' to 'Weight'

        // Additional properties as needed

        public ModelIngredient()
        {
        }
        public ModelIngredient(int ingredientId, string name, decimal quantity, string weight)
        {
            IngredientId = ingredientId;
            Name = name;
            Quantity = quantity;
            Weight = weight;
        }
    }
}
