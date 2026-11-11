namespace RaceDayAPI.Models
{
    public class EventRegistration
    {
        public int EventRegistrationId { get; set; }

        public int ParticipantId { get; set; }

        public int RaceEventId { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        public Participant? Participant { get; set; }

        public RaceEvent? RaceEvent { get; set; }
    }
}