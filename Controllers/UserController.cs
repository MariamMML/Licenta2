using Microsoft.AspNetCore.Mvc;
using Licenta2.Models;

namespace Licenta2.Controllers
{
    public class UserController : Controller
    {
        private readonly AuthenticationService _authenticationService;

        public UserController()
        {
            _authenticationService = new AuthenticationService();
        }

        // GET: User/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string email, string password)
        {
            if (ModelState.IsValid)
            {
                _authenticationService.RegisterUser(email, password);
                return RedirectToAction(nameof(Login));
            }
            return View();
        }

        // GET: User/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            if (_authenticationService.Authenticate(email, password))
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }
    }
}
