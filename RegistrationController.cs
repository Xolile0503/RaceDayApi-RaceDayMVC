using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public RegistrationsController(RaceDayContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterForEvent(RegisterForEventRequest request)
        {
            bool participantExists = await _context.Participants
                .AnyAsync(p => p.ParticipantId == request.ParticipantId);
            if (!participantExists)
                return BadRequest("Participant does not exist.");

            bool eventExists = await _context.RaceEvents
                .AnyAsync(e => e.RaceEventId == request.RaceEventId);
            if (!eventExists)
                return BadRequest("Race event does not exist.");

            bool alreadyRegistered = await _context.EventRegistrations
                .AnyAsync(r => r.ParticipantId == request.ParticipantId
                            && r.RaceEventId == request.RaceEventId);
            if (alreadyRegistered)
                return Conflict("Participant is already registered.");

            var registration = new EventRegistration
            {
                ParticipantId = request.ParticipantId,
                RaceEventId = request.RaceEventId,
                RegistrationDate = DateTime.UtcNow
            };

            _context.EventRegistrations.Add(registration);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Registration completed successfully.",
                registration.EventRegistrationId,
                registration.ParticipantId,
                registration.RaceEventId,
                registration.RegistrationDate
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetRegistrations()
        {
            var registrations = await _context.EventRegistrations
                .Select(r => new
                {
                    r.EventRegistrationId,
                    r.ParticipantId,
                    r.RaceEventId,
                    r.RegistrationDate
                })
                .ToListAsync();

            return Ok(registrations);
        }

        [HttpGet("participant/{participantId:int}")]
        public async Task<IActionResult> GetRegistrationsByParticipant(int participantId)
        {
            bool exists = await _context.Participants.AnyAsync(p => p.ParticipantId == participantId);
            if (!exists)
                return NotFound("Participant not found.");

            var regs = await _context.EventRegistrations
                .Where(r => r.ParticipantId == participantId)
                .Select(r => new
                {
                    r.EventRegistrationId,
                    r.RaceEventId,
                    r.RegistrationDate,
                    EventName = r.RaceEvent != null ? r.RaceEvent.Name : null,
                    EventDate = r.RaceEvent != null ? r.RaceEvent.EventDate : (DateTime?)null
                })
                .ToListAsync();

            return Ok(regs);
        }

        [HttpGet("event/{eventId:int}")]
        public async Task<IActionResult> GetRegistrationsByEvent(int eventId)
        {
            bool exists = await _context.RaceEvents.AnyAsync(e => e.RaceEventId == eventId);
            if (!exists)
                return NotFound("Race event not found.");

            var regs = await _context.EventRegistrations
                .Where(r => r.RaceEventId == eventId)
                .Select(r => new
                {
                    r.EventRegistrationId,
                    r.ParticipantId,
                    r.RegistrationDate,
                    ParticipantName = r.Participant != null && r.Participant.User != null
                        ? r.Participant.User.FirstName + " " + r.Participant.User.LastName
                        : null
                })
                .ToListAsync();

            return Ok(regs);
        }
    }
}