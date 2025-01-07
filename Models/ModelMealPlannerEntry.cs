using System.ComponentModel.DataAnnotations;

namespace Licenta2.Models
{
    public class ModelMealPlannerEntry
    {
        [Key]
        public int Id { get; set; }  // Primary Key
        public int MealPlannerId { get; set; }  // Foreign Key
        public ModelMealPlanner MealPlanner { get; set; }
        public int RecipeId { get; set; }  // Foreign Key
        public ModelRecipe Recipe { get; set; }
        public DateTime Date { get; set; }  // When the recipe is planned
        public string MealType { get; set; }  // Breakfast, Lunch, Dinner, etc.
    }
}
