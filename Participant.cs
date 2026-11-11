namespace RaceDayAPI.Models
{
    public class Participant
    {
        public int ParticipantId { get; set; }

        public int UserId { get; set; }

        public string? EmergencyContact { get; set; }

        public User? User { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}