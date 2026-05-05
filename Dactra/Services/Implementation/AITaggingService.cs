using Azure;

namespace Dactra.Services.Implementation
{
    public class AITaggingService : IAITaggingService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly AITaggingOptions _options;
        private readonly ILogger<AITaggingService> _logger;

        public AITaggingService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IOptions<AITaggingOptions> options,
            ILogger<AITaggingService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("OpenAI");
            _configuration = configuration;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<List<string>> ExtractTagsFromContentAsync(string content, List<string> availableTags)
        {
            if (!availableTags.Any()) return new List<string>();

            var apiKey = _configuration["AITagging:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("AITagging: API key is missing.");
                return new List<string>();
            }

            var tagsJson = JsonSerializer.Serialize(availableTags);
            var prompt = $"""
            You are a medical content tagging assistant.
            Given the following medical post content, (if it's in any other language translate it to English first), and select the most relevant tags from the predefined list below.
            Return ONLY a JSON array of tag names that match. Return an empty array [] if none match.
            Do not invent new tags

            Available Tags:
            {tagsJson}

            Post Content:
            {content}

            Respond ONLY with a JSON array, e.g.: ["Cardiology", "Hypertension"]
            """;

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);

            foreach (var model in _options.PriorityModels)
            {
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await _httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("AITagging: Model {Model} failed with status {StatusCode}", model, response.StatusCode);
                        continue;
                    }

                    var responseJson = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("AITagging: Raw Gemini response = {Response}", responseJson);

                    using var doc = JsonDocument.Parse(responseJson);
                    var rawText = doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString() ?? "[]";

                    _logger.LogInformation("AITagging: AI returned text = '{RawText}'", rawText);

                    rawText = rawText.Trim();
                    if (rawText.StartsWith("```")) rawText = rawText.Split('\n', 2)[1];
                    if (rawText.EndsWith("```")) rawText = rawText[..rawText.LastIndexOf("```")];

                    var extractedTags = JsonSerializer.Deserialize<List<string>>(rawText.Trim()) ?? new List<string>();

                    var matchedTags = extractedTags
                        .Where(t => availableTags.Any(a => string.Equals(a, t, StringComparison.OrdinalIgnoreCase)))
                        .ToList();

                    _logger.LogInformation("AITagging: Final matched tags = [{Tags}]", string.Join(", ", matchedTags));

                    return matchedTags;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "AITagging: Exception occurred with model {Model}, trying next...", model);
                    continue;
                }
            }

            _logger.LogWarning("AITagging: All models failed.");
            return new List<string>();
        }
    }
}