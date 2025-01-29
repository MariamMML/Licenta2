using System.ComponentModel.DataAnnotations;

namespace Licenta2.Models
{
    public class ModelIngredient
    {
        [Key]
        public int IngredientId { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public double Weight { get; set; } 

        // Additional properties as needed

        public ModelIngredient()
        {
        }
        public ModelIngredient(int ingredientId, string name, decimal quantity, double weight)
        {
            IngredientId = ingredientId;
            Name = name;
            Quantity = quantity;
            Weight = weight;
        }
    }
}
