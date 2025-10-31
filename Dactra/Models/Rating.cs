namespace Dactra.Models
{
    public class Rating
    {
        public int Rate_Id { get; set; }

        public int Patient_ID { get; set; }
        public PatientProfile Patient { get; set; }
        
        public int Provider_ID { get; set; }
        public ServiceProvider Provider { get; set; }
        
        public int Score { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime Rated_At { get; set; } = DateTime.Now;
    }
}
