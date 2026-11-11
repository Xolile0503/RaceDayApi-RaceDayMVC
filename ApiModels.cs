using Microsoft.AspNetCore.Http;

namespace RaceDayMVC.Models
{
    public class LoginVm
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class RegisterVm
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "Participant";
    }

    public class LoginResult
    {
        public string Token { get; set; } = "";
        public int UserId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public int? ProfileId { get; set; }

    }

    public class EventListVm
    {
        public int RaceEventId { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string Location { get; set; } = "";
        public DateTime EventDate { get; set; }
        public int OrganiserId { get; set; }
        public int RegistrationCount { get; set; }
        public double Distance { get; set; }
        public string EventType { get; set; } = "";
        public string? BannerUrl { get; set; }
    }

    public class CreateEventVm
    {
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string Location { get; set; } = "";
        public DateTime EventDate { get; set; } = DateTime.Now.AddDays(7);
        public double Distance { get; set; }
        public string EventType { get; set; } = "Road";
        public IFormFile? Banner { get; set; }
    }
    public class EventDetailVm : EventListVm
    {
        
    }

    public class RegistrationVm
    {
        public int EventRegistrationId { get; set; }
        public int ParticipantId { get; set; }
        public int RaceEventId { get; set; }
        public int? EventCategoryId { get; set; }
        public DateTime RegistrationDate { get; set; }

        public string? ParticipantName { get; set; }
        public string? EventName { get; set; }
        public DateTime? EventDate { get; set; }
        public string? CategoryName { get; set; }

        public string? FinishTime { get; set; }
        public int? FinishingPosition { get; set; }
    }
    public class ParticipantProfileVm
    {
        public int ParticipantId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? EmergencyContact { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public IFormFile? PictureFile { get; set; }
    }
}