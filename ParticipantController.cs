using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Services;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantsController : ControllerBase
    {
        private readonly RaceDayContext _context;
        private readonly IBlobService _blob;

        public ParticipantsController(RaceDayContext context, IBlobService blob)
        {
            _context = context;
            _blob = blob;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var p = await _context.Participants
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.ParticipantId == id);

            if (p == null) return NotFound();

            return Ok(new
            {
                p.ParticipantId,
                p.UserId,
                p.User!.FirstName,
                p.User.LastName,
                p.User.Email,
                p.EmergencyContact,
                p.ProfilePictureUrl
            });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromForm] string? emergencyContact, IFormFile? profilePicture)
        {
            var p = await _context.Participants.FirstOrDefaultAsync(x => x.ParticipantId == id);
            if (p == null) return NotFound();

            p.EmergencyContact = emergencyContact;

            if (profilePicture != null && profilePicture.Length > 0)
            {
                p.ProfilePictureUrl = await _blob.UploadAsync(profilePicture);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Profile updated successfully!", p.ProfilePictureUrl });
        }
    }
}