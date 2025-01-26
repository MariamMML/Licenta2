using System;
using System.ComponentModel.DataAnnotations;

namespace Licenta2.Models
{
    public class ModelMealPlannerEntry
    {
        [Key]
        public int Id { get; set; }  // Primary Key

        [Required]
        public int MealPlannerId { get; set; }  // Foreign Key
        public ModelMealPlanner MealPlanner { get; set; }  // Navigation property

        [Required]
        public int RecipeId { get; set; }  // Foreign Key
        public ModelRecipe Recipe { get; set; }  // Navigation property

        [Required]
        public DateTime Date { get; set; }  // When the recipe is planned

        [Required]
        [StringLength(50)] // Limit meal types to a reasonable length
        public string MealType { get; set; }  // E.g., Breakfast, Lunch, Dinner
    }
}
