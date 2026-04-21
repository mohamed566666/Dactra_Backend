namespace Dactra.DTOs.ComplaintsDTOs
{
    public class CreateComplaintDTO
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public ComplaintAgainst Against { get; set; }
    }
}
