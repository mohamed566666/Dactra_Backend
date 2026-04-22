namespace Dactra.DTOs
{
    public class ReportCreateDTO
    {

        public ReportType Type { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int? RelatedEntityId { get; set; }
    }
}
