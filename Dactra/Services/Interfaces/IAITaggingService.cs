namespace Dactra.Services.Interfaces
{
    public interface IAITaggingService
    {
        Task<List<string>> ExtractTagsFromContentAsync(string content, List<string> availableTags);
    }
}
