using Microsoft.AspNetCore.Mvc;
using Licenta2.Data;
using Licenta2.Models;
using System.Linq;

namespace Licenta2.Controllers
{
    public class IngredientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IngredientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ingredient
        public IActionResult Index()
        {
            return View(_context.Ingredients.ToList());
        }

        // GET: Ingredient/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ingredient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Quantity,Weight")] ModelIngredient ingredient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ingredient);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(ingredient);
        }

        // GET: Ingredient/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = _context.Ingredients.Find(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            return View(ingredient);
        }

        // POST: Ingredient/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("IngredientId,Name,Quantity,Weight")] ModelIngredient ingredient)
        {
            if (id != ingredient.IngredientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(ingredient);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(ingredient);
        }

        // GET: Ingredient/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = _context.Ingredients
                .FirstOrDefault(m => m.IngredientId == id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        // POST: Ingredient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var ingredient = _context.Ingredients.Find(id);
            _context.Ingredients.Remove(ingredient);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
