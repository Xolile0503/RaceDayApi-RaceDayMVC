using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly ApiClient _api;
        public ParticipantController(ApiClient api) => _api = api;

        private bool IsParticipant => HttpContext.Session.GetString("Role") == "Participant";

        public async Task<IActionResult> Index()
        {
            if (!IsParticipant) return RedirectToAction("Login", "Account");
            var events = await _api.GetEventsAsync();
            return View(events);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!IsParticipant) return RedirectToAction("Login", "Account");
            var ev = await _api.GetEventAsync(id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost]
        public async Task<IActionResult> Enrol(int eventId)
        {
            if (!IsParticipant) return RedirectToAction("Login", "Account");

            int participantId = HttpContext.Session.GetInt32("ProfileId") ?? 0;
            var (ok, error) = await _api.EnrolAsync(eventId, participantId);

            if (ok)
                TempData["Success"] = "Successfully enrolled!";
            else
                TempData["Error"] = error ?? "Failed to enrol.";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MyEvents()
        {
            if (!IsParticipant) return RedirectToAction("Login", "Account");

            int participantId = HttpContext.Session.GetInt32("ProfileId") ?? 0;
            var regs = await _api.GetMyRegistrationsAsync(participantId);
            return View(regs);
        }
    }
}