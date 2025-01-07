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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Recipe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create([Bind("RecipeName,Instructions,Ingredients")] ModelRecipe recipe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recipe);
                _context.SaveChanges();
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




        //public IActionResult Search(string[] ingredients)
        //{
        //    var recipes = _context.Recipes
        //                          .Where(r => r.Ingredients
        //                          .Any(i => ingredients.Contains(i.Name)))
        //                          .ToList();

        //    return View(recipes);
        //}
    }
}
