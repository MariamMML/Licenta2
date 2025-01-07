using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Licenta2.Models;

namespace Licenta2.Data
{
    public class ApplicationDbContext : IdentityDbContext<ModelUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Custom DbSets for your application
        public DbSet<ModelRecipe> Recipes { get; set; }
        public DbSet<ModelIngredient> Ingredients { get; set; }
        public DbSet<ModelMealPlanner> MealPlanner { get; set; }
       
        public DbSet<ModelMealPlannerEntry> ModelMealPlannerEntries { get; set; }
    }
}
