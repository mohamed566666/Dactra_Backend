namespace Dactra.DTOs
{
    public class ReportCardDTO
    {
         public int Id { get; set; }

    public string UserEmail { get; set; } = null!;

    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;

    public string Type { get; set; } = null!;

    public int? RelatedEntityId { get; set; }

    public string RedirectUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    }
}
