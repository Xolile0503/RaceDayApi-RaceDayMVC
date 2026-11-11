using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly ApiClient _api;
        public OrganiserController(ApiClient api) => _api = api;

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser") return RedirectToAction("Login", "Account");

            int organiserId = HttpContext.Session.GetInt32("ProfileId") ?? 0;
            var events = await _api.GetMyEventsAsync(organiserId);
            return View(events);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            return View(new CreateEventVm());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEventVm vm)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            int organiserId = HttpContext.Session.GetInt32("ProfileId") ?? 0;

            var (ok, error) = await _api.CreateEventAsync(vm, organiserId);

            if (!ok)
            {
                ModelState.AddModelError("", error ?? "Failed to create event.");
                return View(vm);
            }

            TempData["Success"] = "Event created successfully!";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Enrolments(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            var regs = await _api.GetEventRegistrationsAsync(id);
            return View(regs);
        }
    }
}