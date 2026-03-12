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

            var apiKey = _configuration["OpenAI:ApiKey"];
            var model = _configuration["OpenAI:Model"] ?? "gpt-4o-mini";

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
                model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.2,
                max_tokens = 200
            };

            var json = JsonSerializer.Serialize(requestBody);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseJson);

                var rawText = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "[]";

                rawText = rawText.Trim();
                if (rawText.StartsWith("```")) rawText = rawText.Split('\n', 2)[1];
                if (rawText.EndsWith("```")) rawText = rawText[..rawText.LastIndexOf("```")];

                var extractedTags = JsonSerializer.Deserialize<List<string>>(rawText.Trim()) ?? new List<string>();

                return extractedTags
                    .Where(t => availableTags.Any(a => string.Equals(a, t, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI tagging failed for content. Returning empty tag list.");
                return new List<string>();
            }
        }
    }
}
