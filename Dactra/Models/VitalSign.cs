namespace Dactra.Models
{
    public class VitalSign
    {
        public int Id { get; set; }
        public int  Value { get; set; }
        public string  Type { get; set; } = string.Empty;
        public string data { get; set; }=string.Empty;
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

    }
}
