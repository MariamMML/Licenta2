using Licenta2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Licenta2.Controllers
{
    [Authorize(Roles = "Administrator")] // Restrict access to administrators
    public class AdminManagementController : Controller
    {
        private readonly UserManager<ModelUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminManagementController(UserManager<ModelUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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
    }
}
