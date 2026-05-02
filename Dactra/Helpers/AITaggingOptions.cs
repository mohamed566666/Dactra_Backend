namespace Dactra.Helpers
{
    public class AITaggingOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public List<string> PriorityModels { get; set; } = new();
    }
}
