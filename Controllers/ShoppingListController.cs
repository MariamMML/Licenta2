using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Licenta2.Models;
using Licenta2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Licenta2.Controllers
{
    public class ShoppingListController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ModelUser> _userManager;

        public ShoppingListController(ApplicationDbContext context, UserManager<ModelUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        public IActionResult RenderShoppingList()
        {
            var user = _userManager.GetUserAsync(User).Result;

            if (user == null)
            {
                return Unauthorized();
            }

            var startDate = DateTime.Today;
            var endDate = startDate.AddDays(7);

            // Fetch meal planner entries for the user within the date range
            var mealPlannerEntries = _context.ModelMealPlannerEntries
                .Where(entry => entry.MealPlanner.UserId == user.Id && entry.Date >= startDate && entry.Date <= endDate)
                .Include(entry => entry.Recipe)
                    .ThenInclude(recipe => recipe.Ingredients)
                .ToList();

            // Aggregate ingredients
            var aggregatedIngredients = mealPlannerEntries
    .SelectMany(entry => entry.Recipe.Ingredients)
    .GroupBy(ingredient => ingredient.Name)
    .Select(group => new
    {
        Name = group.Key,
        TotalQuantity = group.Sum(ingredient => ingredient.Quantity),
        TotalWeight = group.Sum(ingredient => ingredient.Weight) // Ensure Weight is correctly referenced
    })
    .ToList();

            return View("ShoppingList", aggregatedIngredients); // Render a dedicated ShoppingList view
        }


        //[HttpGet]
        //public async Task<IActionResult> GenerateShoppingList()
        //{
        //    var user = await _userManager.GetUserAsync(User); // Fetch the logged-in user
        //    if (user == null)
        //    {
        //        return Unauthorized();
        //    }

        //    var startDate = DateTime.Today; // Start from today
        //    var endDate = startDate.AddDays(7); // Up to 7 days later

        //    // Fetch meal planner entries for the user within the date range
        //    var mealPlannerEntries = await _context.ModelMealPlannerEntries
        //        .Where(entry => entry.MealPlanner.UserId == user.Id && entry.Date >= startDate && entry.Date <= endDate)
        //        .Include(entry => entry.Recipe) // Include the Recipe navigation property
        //            .ThenInclude(recipe => recipe.Ingredients) // Include Ingredients of each Recipe
        //        .ToListAsync();
        //    Console.WriteLine($"StartDate: {startDate}, EndDate: {endDate}, Entries: {mealPlannerEntries.Count}");

        //    if (mealPlannerEntries == null || mealPlannerEntries.Count == 0)
        //    {
        //        return Json(new { Message = "No meals found in the selected period.", Data = new List<object>() });
        //    }

        //    // Aggregate ingredients from the recipes
        //    var aggregatedIngredients = mealPlannerEntries
        //        .SelectMany(entry => entry.Recipe.Ingredients) // Access ingredients through Recipe navigation
        //        .GroupBy(ingredient => ingredient.Name) // Group by ingredient name
        //        .Select(group => new
        //        {
        //            Name = group.Key,
        //            TotalQuantity = group.Sum(ingredient => ingredient.Quantity),
        //            Unit = group.First().Weight // Assuming each ingredient has a unit (e.g., grams, cups)
        //        })
        //        .ToList();

        //    if (!aggregatedIngredients.Any())
        //    {
        //        return Json(new { Message = "No ingredients found for the selected period.", Data = aggregatedIngredients });
        //    }

        //    return Json(new { Message = "Success", Data = aggregatedIngredients });
        //}
    }
}
