namespace Dactra.Models
{
    public class MedicineReminder
    {
        public int Id { get; set; }
        public string PatientId { get; set; } = string.Empty;
        public string MedicineName { get; set; } = string.Empty;
        public string Dose { get; set; } = string.Empty;
        public WhenToTake? MealRelation { get; set; }  
        public TimesPerDay? Frequency { get; set; } 
        public string ScheduledTimes { get; set; } = string.Empty;
        public DateTime? LastSentAt { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

