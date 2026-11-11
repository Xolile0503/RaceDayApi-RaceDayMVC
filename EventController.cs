using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;
using RaceDayAPI.Services;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayContext _context;
        private readonly IBlobService _blobService;

        public EventsController(RaceDayContext context, IBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.RaceEvents
                .Select(e => new
                {
                    e.RaceEventId,
                    e.Name,
                    e.Description,
                    e.Location,
                    e.EventDate,
                    e.OrganiserId,
                    e.Distance,
                    e.EventType,
                    e.BannerUrl,
                    RegistrationCount = _context.EventRegistrations.Count(r => r.RaceEventId == e.RaceEventId)
                })
                .ToListAsync();

            return Ok(events);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var raceEvent = await _context.RaceEvents
                .Where(e => e.RaceEventId == id)
                .Select(e => new
                {
                    e.RaceEventId,
                    e.Name,
                    e.Description,
                    e.Location,
                    e.EventDate,
                    e.OrganiserId,
                    e.Distance,
                    e.EventType,
                    e.BannerUrl,
                    RegistrationCount = _context.EventRegistrations.Count(r => r.RaceEventId == e.RaceEventId)
                })
                .SingleOrDefaultAsync();

            if (raceEvent == null)
                return NotFound("Race event not found.");

            return Ok(raceEvent);
        }

        [HttpGet("organiser/{organiserId:int}")]
        public async Task<IActionResult> GetEventsByOrganiser(int organiserId)
        {
            var events = await _context.RaceEvents
                .Where(e => e.OrganiserId == organiserId)
                .Select(e => new
                {
                    e.RaceEventId,
                    e.Name,
                    e.Description,
                    e.Location,
                    e.EventDate,
                    e.OrganiserId,
                    e.Distance,
                    e.EventType,
                    e.BannerUrl,
                    RegistrationCount = _context.EventRegistrations.Count(r => r.RaceEventId == e.RaceEventId)
                })
                .ToListAsync();

            return Ok(events);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromForm] CreateEventRequest request, IFormFile? banner)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool organiserExists = await _context.Organisers
                .AnyAsync(o => o.OrganiserId == request.OrganiserId);
            if (!organiserExists)
                return BadRequest("Organiser does not exist.");

            if (request.EventDate <= DateTime.UtcNow)
                return BadRequest("Event date must be in the future.");

            string? bannerUrl = null;
            if (banner != null && banner.Length > 0)
            {
                bannerUrl = await _blobService.UploadAsync(banner);
            }

            var raceEvent = new RaceEvent
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                Location = request.Location.Trim(),
                EventDate = request.EventDate,
                Distance = request.Distance,
                EventType = request.EventType,
                BannerUrl = bannerUrl,
                OrganiserId = request.OrganiserId
            };

            _context.RaceEvents.Add(raceEvent);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Event created successfully!", raceEvent.RaceEventId });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEvent(int id, CreateEventRequest request)
        {
            var raceEvent = await _context.RaceEvents.FindAsync(id);
            if (raceEvent == null) return NotFound("Race event not found.");

            raceEvent.Name = request.Name.Trim();
            raceEvent.Description = request.Description?.Trim();
            raceEvent.Location = request.Location.Trim();
            raceEvent.EventDate = request.EventDate;
            raceEvent.Distance = request.Distance;
            raceEvent.EventType = request.EventType;

            await _context.SaveChangesAsync();
            return Ok(raceEvent);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var raceEvent = await _context.RaceEvents.FindAsync(id);
            if (raceEvent == null) return NotFound("Race event not found.");

            var regs = await _context.EventRegistrations.Where(r => r.RaceEventId == id).ToListAsync();
            _context.EventRegistrations.RemoveRange(regs);
            _context.RaceEvents.Remove(raceEvent);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}