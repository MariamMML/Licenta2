using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Licenta2.Models;
using Licenta2.Data;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Licenta2.Controllers
{
    public class RecipeController: Controller
    {
        private readonly ApplicationDbContext _context;

        public RecipeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Recipe
        public IActionResult Index()
        {
            return View(_context.Recipes.ToList());
        }

        [Authorize]
        

        // POST: Recipe/Create
        public async Task<IActionResult> Create([Bind("RecipeName,Instructions,Ingredients")] ModelRecipe recipe, IFormFile imageFile)
        {
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
            // Retrieve the recipe with its ingredients
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients) // Ensure you load the ingredients
                .FirstOrDefaultAsync(r => r.RecipeId == id);

            if (recipe != null)
            {
                // Remove associated ingredients first
                _context.Ingredients.RemoveRange(recipe.Ingredients);

                // Now remove the recipe
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Recipe/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var recipe = await _context.Recipes
         .Include(r => r.Ingredients) // Include Ingredients here
         .FirstOrDefaultAsync(r => r.RecipeId == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Recipe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecipeId,RecipeName,Instructions,Ingredients")] ModelRecipe recipe)
        {
            if (id != recipe.RecipeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
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

            return View(recipe);
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
