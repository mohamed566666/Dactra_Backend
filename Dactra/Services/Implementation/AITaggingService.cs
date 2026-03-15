namespace Dactra.Services.Implementation
{
    public class AITaggingService : IAITaggingService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AITaggingService> _logger;

        public AITaggingService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<AITaggingService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("OpenAI");
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<List<string>> ExtractTagsFromContentAsync(string content, List<string> availableTags)
        {
            if (!availableTags.Any()) return new List<string>();

            var apiKey = _configuration["AITagging:ApiKey"];
            var model = _configuration["AITagging:Model"] ?? "gemini-2.0-flash";

            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("AITagging: API key is missing.");
                return new List<string>();
            }

            var tagsJson = JsonSerializer.Serialize(availableTags);
            var prompt = $"""
        You are a medical content tagging assistant.
        Given the following medical post content, select the most relevant tags from the predefined list below.
        Return ONLY a JSON array of tag names that match. Return an empty array [] if none match.
        Do not invent new tags. Only use tags from the provided list.
        Select between 1 and 5 tags maximum.

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
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("AITagging: Gemini request failed. Status={Status}, Error={Error}",
                        response.StatusCode, errorBody);
                    return new List<string>();
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
                _logger.LogError(ex, "AITagging: Exception occurred.");
                return new List<string>();
            }
        }
    }
}
