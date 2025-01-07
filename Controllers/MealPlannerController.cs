using Licenta2.Data;
using Licenta2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var mealPlanners = _context.MealPlanner
                .Include(mp => mp.Entries)
                .ThenInclude(e => e.Recipe)
                .Where(mp => mp.UserId == user.Id)
                .ToList();
            return View(mealPlanners);
        }

        // GET: MealPlanner/Create
        public IActionResult Create()
        {
            ViewBag.Recipes = _context.Recipes.ToList();  // Pass recipes to the view for selection
            return View();
        }

        // POST: MealPlanner/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<DateTime> dates, List<int> recipeIds, List<string> mealTypes)
        {
            var user = await _userManager.GetUserAsync(User);

            if (dates.Count != recipeIds.Count || recipeIds.Count != mealTypes.Count)
            {
                ModelState.AddModelError("", "Mismatch in dates, recipes, or meal types.");
                ViewBag.Recipes = _context.Recipes.ToList();
                return View();
            }

            if (ModelState.IsValid)
            {
                var mealPlanner = new ModelMealPlanner
                {
                    UserId = user.Id,
                    Entries = new List<ModelMealPlannerEntry>()
                };

                for (int i = 0; i < dates.Count; i++)
                {
                    mealPlanner.Entries.Add(new ModelMealPlannerEntry
                    {
                        RecipeId = recipeIds[i],
                        Date = dates[i],
                        MealType = mealTypes[i]
                    });
                }

                _context.Add(mealPlanner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Recipes = _context.Recipes.ToList();
            return View();
        }

        // GET: MealPlanner/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var mealPlanner = _context.MealPlanner
                .Include(mp => mp.Entries)
                .FirstOrDefault(mp => mp.Id == id && mp.UserId == user.Id);

            if (mealPlanner == null)
            {
                return NotFound();
            }

            ViewBag.Recipes = _context.Recipes.ToList();
            return View(mealPlanner);
        }

        // POST: MealPlanner/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id")] ModelMealPlanner mealPlanner, List<DateTime> dates, List<int> recipeIds, List<string> mealTypes)
        {
            if (id != mealPlanner.Id)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var existingPlanner = _context.MealPlanner
                .Include(mp => mp.Entries)
                .FirstOrDefault(mp => mp.Id == id && mp.UserId == user.Id);

            if (existingPlanner == null)
            {
                return NotFound();
            }

            if (dates.Count != recipeIds.Count || recipeIds.Count != mealTypes.Count)
            {
                ModelState.AddModelError("", "Mismatch in dates, recipes, or meal types.");
                ViewBag.Recipes = _context.Recipes.ToList();
                return View(mealPlanner);
            }

            if (ModelState.IsValid)
            {
                // Clear existing entries
                _context.ModelMealPlannerEntries.RemoveRange(existingPlanner.Entries);

                // Add updated entries
                for (int i = 0; i < dates.Count; i++)
                {
                    existingPlanner.Entries.Add(new ModelMealPlannerEntry
                    {
                        RecipeId = recipeIds[i],
                        Date = dates[i],
                        MealType = mealTypes[i]
                    });
                }

                _context.Update(existingPlanner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Recipes = _context.Recipes.ToList();
            return View(mealPlanner);
        }

        // GET: MealPlanner/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var mealPlanner = _context.MealPlanner
                .FirstOrDefault(mp => mp.Id == id && mp.UserId == user.Id);

            if (mealPlanner == null)
            {
                return NotFound();
            }

            return View(mealPlanner);
        }

        // POST: MealPlanner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var mealPlanner = _context.MealPlanner
                .FirstOrDefault(mp => mp.Id == id && mp.UserId == user.Id);

            if (mealPlanner == null)
            {
                return NotFound();
            }

            _context.MealPlanner.Remove(mealPlanner);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
