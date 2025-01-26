using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Licenta2.Models;
using Licenta2.Data;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Licenta2.Controllers
{
    public class RecipeController: Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ModelUser> _userManager;

        public RecipeController(ApplicationDbContext context)
        {
            _context = context;
        }
        private static readonly List<string> HardcodedCategories = new()
        {
        "Breakfast",
        "Lunch",
        "Dessert",
        "Beverage"
        };

        // GET: Recipe
        public IActionResult Index()
        {
            return View(_context.Recipes.ToList());
        }



        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            ViewBag.Categories = HardcodedCategories;
            return View(new ModelRecipe());
        }
        [HttpPost]
        [Authorize]

        [Authorize]
        // POST: Recipe/Create
        public async Task<IActionResult> Create([Bind("RecipeName,Instructions,Ingredients, Category")] ModelRecipe recipe, IFormFile imageFile)
        {
            if (User.Identity.IsAuthenticated)
            {
                recipe.CreatedBy = User.Identity.Name;
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = HardcodedCategories;
                return View(recipe);
            }
            // Assign the email address of the logged-in user
            
            if (!ModelState.IsValid)
            {
                // Log validation errors
                foreach (var state in ModelState)
                {
                    Console.WriteLine($"Key: {state.Key}");
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Error: {error.ErrorMessage}");
                    }
                }
                // Return the view with the current model to display validation messages
                return View(recipe);
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {

                    Console.WriteLine("File received: " + imageFile.FileName);
                    Console.WriteLine("File size: " + imageFile.Length);
                    // Save the image to wwwroot/images/recipes
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/recipes");
                    Directory.CreateDirectory(uploads); // Ensure the directory exists
                    var filePath = Path.Combine(uploads, Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName));

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    recipe.ImagePath = "/images/recipes/" + Path.GetFileName(filePath);
                    Console.WriteLine("Image saved to: " + filePath); // Log where it's saved
                }

                _context.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(recipe);
        }
        public IActionResult Search(string query)
        {
            var recipes = from r in _context.Recipes
                          select r;

            if (!string.IsNullOrEmpty(query))
            {
                recipes = recipes.Where(r => r.RecipeName.Contains(query) || r.Instructions.Contains(query));
            }

            return View(recipes.ToList());
        }
        [HttpGet]
        public IActionResult SearchByIngredients(string[] ingredients)
        {
            var recipes = _context.Recipes.Include(r => r.Ingredients).AsQueryable();

            if (ingredients != null && ingredients.Length > 0)
            {
                recipes = recipes.Where(r => r.Ingredients.Any(i => ingredients.Contains(i.Name)));
            }

            return View(recipes.ToList());
        }
        // GET: Recipe/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients) // Include ingredients if needed
                .FirstOrDefaultAsync(r => r.RecipeId == id); // Assuming RecipeId is the primary key

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }
        // POST: Recipe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipes
        .Include(r => r.Ingredients)
        .FirstOrDefaultAsync(r => r.RecipeId == id);

            if (recipe == null)
            {
                return NotFound();
            }

            var currentUser = User.Identity.Name; // Email of the logged-in user
            var isAdmin = User.IsInRole("Admin"); // Check if the user is in the Admin role

            // Allow deletion only if the user is the creator or an admin
            if (recipe.CreatedBy != currentUser && !isAdmin)
            {
                return Forbid(); // Return 403 Forbidden
            }

            // Remove associated ingredients first
            _context.Ingredients.RemoveRange(recipe.Ingredients);

            // Remove the recipe
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // GET: Recipe/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients) // Include related ingredients
                .FirstOrDefaultAsync(r => r.RecipeId == id);

            if (recipe == null)
            {
                return NotFound();
            }
            ViewBag.Categories = HardcodedCategories;

            return View(recipe);
        }


        // POST: Recipe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(
    int id,
    [Bind("RecipeId,RecipeName,Instructions,Ingredients,Category,ImagePath,CreatedBy")] ModelRecipe recipe,
    IFormFile imageFile)
        {
            if (id != recipe.RecipeId)
            {
                return NotFound();
            }


            // Retrieve the existing recipe from the database
            var existingRecipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.RecipeId == id);

            if (existingRecipe == null)
            {
                return NotFound();
            }
            // Check if the user is authorized to edit
            var isAdmin = User.IsInRole("Administrator");
            var isOwner = existingRecipe.CreatedBy == User.Identity.Name;

            if (!isOwner && !isAdmin)
            {
                return Forbid(); // Return 403 Forbidden if the user is not authorized
            }

            ModelState.Remove("imageFile");
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = HardcodedCategories; // Ensure dropdown is repopulated
                return View(recipe); // Return view with validation errors
            }

            // Update the recipe details
            existingRecipe.RecipeName = recipe.RecipeName;
            existingRecipe.Instructions = recipe.Instructions;
            existingRecipe.Category = recipe.Category;

            // Handle optional image replacement
            if (imageFile != null && imageFile.Length > 0)
            {
                // Save the new image
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/recipes");
                Directory.CreateDirectory(uploads); // Ensure the directory exists
                var filePath = Path.Combine(uploads, Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName));

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                // Delete the old image file if it exists
                if (!string.IsNullOrEmpty(existingRecipe.ImagePath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingRecipe.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Update the image path
                existingRecipe.ImagePath = "/images/recipes/" + Path.GetFileName(filePath);
            }

            // Update ingredients
            _context.Ingredients.RemoveRange(existingRecipe.Ingredients); // Remove old ingredients
            if (recipe.Ingredients != null && recipe.Ingredients.Any())
            {
                foreach (var ingredient in recipe.Ingredients)
                {
                    existingRecipe.Ingredients.Add(new ModelIngredient
                    {
                        Name = ingredient.Name,
                        Quantity = ingredient.Quantity,
                        Weight = ingredient.Weight
                    });
                }
            }

            // Save the changes
            try
            {
                _context.Update(existingRecipe);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Recipes.Any(r => r.RecipeId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: MealPlanner/DayView?date=yyyy-MM-dd
        public async Task<IActionResult> DayView(DateTime date)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            // Get all recipes for the specified day
            var entries = await _context.ModelMealPlannerEntries
                .Include(e => e.Recipe)
                .Where(e => e.MealPlanner.UserId == user.Id && e.Date.Date == date.Date)
                .ToListAsync();

            ViewBag.Date = date;
            return View(entries);
        }

        // GET: MealPlanner/AddRecipe?date=yyyy-MM-dd
        public async Task<IActionResult> AddRecipe(DateTime date)
        {
            ViewBag.Date = date;
            return View();
        }

        // POST: MealPlanner/AddRecipe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRecipe(DateTime date, string recipeName)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            // Find the recipe by name
            var recipe = await _context.Recipes.FirstOrDefaultAsync(r => r.RecipeName == recipeName);

            if (recipe == null)
            {
                ModelState.AddModelError("", "Recipe not found.");
                ViewBag.Date = date;
                return View();
            }

            // Get or create a meal planner for the user
            var mealPlanner = await _context.MealPlanner
                .FirstOrDefaultAsync(mp => mp.UserId == user.Id);

            if (mealPlanner == null)
            {
                mealPlanner = new ModelMealPlanner { UserId = user.Id };
                _context.MealPlanner.Add(mealPlanner);
                await _context.SaveChangesAsync();
            }

            // Add the recipe to the day
            var mealPlannerEntry = new ModelMealPlannerEntry
            {
                MealPlannerId = mealPlanner.Id,
                RecipeId = recipe.RecipeId,
                Date = date,
                MealType = "Any" // Default meal type
            };

            _context.ModelMealPlannerEntries.Add(mealPlannerEntry);
            await _context.SaveChangesAsync();

            return RedirectToAction("DayView", new { date = date.ToString("yyyy-MM-dd") });
        }



        public async Task<IActionResult> Details(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients) // Ensure Ingredients are included
                .FirstOrDefaultAsync(r => r.RecipeId == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }




    }
}
