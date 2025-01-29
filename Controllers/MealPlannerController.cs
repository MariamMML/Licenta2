using Licenta2.Data;
using Licenta2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Licenta2.Controllers
{
    public class MealPlannerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ModelUser> _userManager;

        public MealPlannerController(ApplicationDbContext context, UserManager<ModelUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MealPlanner
        // GET: MealPlanner
        public async Task<IActionResult> Index(int? year, int? month)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            // Determine the current year and month
            var currentDate = DateTime.Now;
            var selectedYear = year ?? currentDate.Year;
            var selectedMonth = month ?? currentDate.Month;

            // Get all meal entries for the user in the selected month
            var mealEntries = await _context.ModelMealPlannerEntries
                .Include(e => e.Recipe)
                .Where(e => e.MealPlanner.UserId == user.Id &&
                            e.Date.Year == selectedYear &&
                            e.Date.Month == selectedMonth)
                .ToListAsync();

            // Pass necessary data to the view
            ViewBag.SelectedYear = selectedYear;
            ViewBag.SelectedMonth = selectedMonth;
            ViewBag.MealEntries = mealEntries;

            return View();
        }





        //..............................................................








        // GET: MealPlanner/AddRecipe?date=yyyy-MM-dd
        public async Task<IActionResult> AddRecipe(DateTime date)
        {
            ViewBag.Date = date;
            ViewBag.Recipes = await _context.Recipes.ToListAsync(); // Pass all recipes
            ViewBag.MealTypes = new List<string> { "Breakfast", "Lunch", "Dessert", "Beverage" }; // Predefined meal types
            return View();
        }



        // POST: MealPlanner/AddRecipe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRecipe(DateTime date, int recipeId, string mealType)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            // Validate input
            if (recipeId <= 0 || string.IsNullOrEmpty(mealType))
            {
                ModelState.AddModelError("", "Invalid input. Please select a recipe and provide a meal type.");
                ViewBag.Date = date;
                ViewBag.Recipes = await _context.Recipes.ToListAsync();
                return View();
            }

            // Get or create a meal planner for the user
            var mealPlanner = await _context.MealPlanner
                .FirstOrDefaultAsync(mp => mp.UserId == user.Id);

            if (mealPlanner == null)
            {
                mealPlanner = new ModelMealPlanner
                {
                    UserId = user.Id
                };
                _context.MealPlanner.Add(mealPlanner);
                await _context.SaveChangesAsync();
            }

            // Add the recipe to the meal planner for the specified date
            var mealPlannerEntry = new ModelMealPlannerEntry
            {
                MealPlannerId = mealPlanner.Id,
                RecipeId = recipeId,
                Date = date,
                MealType = mealType
            };

            _context.ModelMealPlannerEntries.Add(mealPlannerEntry);
            await _context.SaveChangesAsync();

            return RedirectToAction("DayView", new { date = date.ToString("yyyy-MM-dd") });
        }




        //.............................................................. 



        public IActionResult Next7DaysRecipes()
        {
            var user = _userManager.GetUserAsync(User).Result;

            if (user == null)
            {
                return Unauthorized();
            }

            var startDate = DateTime.Today;
            var endDate = startDate.AddDays(7);

            // Fetch meal planner entries for the next 7 days for the logged-in user
            var mealPlannerEntries = _context.ModelMealPlannerEntries
                .Where(entry => entry.MealPlanner.UserId == user.Id && entry.Date >= startDate && entry.Date <= endDate)
                .Include(entry => entry.Recipe) // Ensure recipes are loaded
                .ToList();

            return View("Next7DaysRecipes", mealPlannerEntries);
        }


        //.............................................................. 



        // GET: MealPlanner/DayView?date=yyyy-MM-dd
        public async Task<IActionResult> DayView(DateTime date)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            // Get all entries for the specified day
            var entries = await _context.ModelMealPlannerEntries
                .Include(e => e.Recipe)
                .Where(e => e.MealPlanner.UserId == user.Id && e.Date.Date == date.Date)
                .ToListAsync();

            ViewBag.Date = date;
            return View(entries);
        }


        //..............................................................


        // GET: MealPlanner/EditEntries?date=yyyy-MM-dd
        public async Task<IActionResult> EditEntries(DateTime date)
        {
            var user = await _userManager.GetUserAsync(User);

            // Get all entries for the specified date
            var entries = await _context.ModelMealPlannerEntries
                .Include(e => e.Recipe)
                .Where(e => e.MealPlanner.UserId == user.Id && e.Date.Date == date.Date)
                .ToListAsync();

            ViewBag.Date = date;
            ViewBag.Recipes = await _context.Recipes.ToListAsync();
            return View(entries);
        }

        //// POST: MealPlanner/EditEntries
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EditEntries(DateTime date, List<int> entryIds, List<int> recipeIds, List<string> mealTypes)
        //{
        //    var user = await _userManager.GetUserAsync(User);

        //    // Validate the input
        //    if (entryIds.Count != recipeIds.Count || recipeIds.Count != mealTypes.Count)
        //    {
        //        ModelState.AddModelError("", "Invalid input data.");
        //        return RedirectToAction(nameof(EditEntries), new { date });
        //    }

        //    // Update existing entries
        //    for (int i = 0; i < entryIds.Count; i++)
        //    {
        //        var entry = await _context.ModelMealPlannerEntries
        //            .FirstOrDefaultAsync(e => e.Id == entryIds[i] && e.MealPlanner.UserId == user.Id);

        //        if (entry != null)
        //        {
        //            entry.RecipeId = recipeIds[i];
        //            entry.MealType = mealTypes[i];
        //        }
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        // POST: MealPlanner/DeleteEntry/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEntry(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var entry = await _context.ModelMealPlannerEntries
                .FirstOrDefaultAsync(e => e.Id == id && e.MealPlanner.UserId == user.Id);

            if (entry == null)
            {
                return NotFound();
            }

            _context.ModelMealPlannerEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
