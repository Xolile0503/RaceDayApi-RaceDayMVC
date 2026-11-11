using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiClient _api;
        public AccountController(ApiClient api) => _api = api;

        [HttpGet] public IActionResult Login() => View();
        [HttpGet] public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginVm vm)
        {
            var result = await _api.LoginAsync(vm);
            if (result == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(vm);
            }

            // Save user state in session
            HttpContext.Session.SetString("Token", result.Token ?? "dummy-token-for-now");
            HttpContext.Session.SetInt32("UserId", result.UserId);
            HttpContext.Session.SetInt32("ProfileId", result.ProfileId ?? 0);
            HttpContext.Session.SetString("Role", result.Role);
            HttpContext.Session.SetString("Name", $"{result.FirstName} {result.LastName}");

            // Redirect based on role
            if (result.Role == "Organiser")
                return RedirectToAction("Index", "Organiser");
            else
                return RedirectToAction("Index", "Participant");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVm vm)
        {
            var (ok, error) = await _api.RegisterAsync(vm);
            if (!ok)
            {
                // Displays the exact error from the API (e.g. "The Password field is required.")
                ModelState.AddModelError("", error ?? "Registration failed. Check your inputs.");
                return View(vm);
            }

            TempData["Success"] = "Registration successful. Please log in.";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}