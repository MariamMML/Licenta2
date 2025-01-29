using Licenta2.Data;
using Licenta2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Licenta2.Controllers
{
    [Authorize(Roles = "Administrator")] // Restrict access to administrators
    public class AdminManagementController : Controller
    {
        private readonly UserManager<ModelUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminManagementController(UserManager<ModelUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // Display all users
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // Assign the Administrator role to a user
        [HttpPost]
        public async Task<IActionResult> AssignAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            var roleExists = await _roleManager.RoleExistsAsync("Administrator");
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole("Administrator"));
            }

            await _userManager.AddToRoleAsync(user, "Administrator");
            TempData["Success"] = $"{user.Email} has been assigned as an Administrator.";
            return RedirectToAction("Index");
        }

        // Delete a user
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = $"{user.Email} has been deleted.";
            }
            else
            {
                TempData["Error"] = $"Failed to delete {user.Email}.";
            }

            return RedirectToAction("Index");
        }



        public IActionResult RecipeStatistics()
        {
            var ingredientCounts = _context.Recipes
                .Include(r => r.Ingredients)
                .SelectMany(r => r.Ingredients)
                .GroupBy(i => i.Name)
                .Select(g => new
                {
                    Ingredient = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .ToList();

            ViewBag.MostPopularIngredient = ingredientCounts.FirstOrDefault();
            return View(ingredientCounts);
        }


        public IActionResult UserActivityStatistics()
        {
            // Fetch users with the number of recipes they created
            var userActivity = _context.Users
                .Select(user => new
                {
                    UserName = user.Email,
                    RecipeCount = _context.Recipes.Count(r => r.CreatedBy == user.Email) // Count recipes by user
                })
                .OrderByDescending(u => u.RecipeCount) // Sort by highest number of recipes
                .ToList();

            return View("UserActivityStatistics", userActivity);
        }


    }
}

