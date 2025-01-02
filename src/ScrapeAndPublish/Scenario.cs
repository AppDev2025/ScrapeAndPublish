using System.Text.Json;

namespace ScrapeAndPublish;

public class Scenario
{
    public required string Description { get; init; }
    public required List<ScrapingScenarioItem> ScrapingScenario { get; init; }
    public required List<PublishingScenarioItem> PublishingScenario { get; init; }

    public class ScrapingScenarioItem
    {
        public required string Description { get; init; }
        public required string Url { get; init; }
        public required string Type { get; init; }
        public required List<ScrapedItem> Items { get; init; }

        public class ScrapedItem
        {
            public required string Name { get; init; }
            public required string Selector { get; init; }
            public string? Target { get; init; }
            public string? AttributeName { get; init; }
            public string? Regex { get; init; }
            public int? RegexCaptureIndex { get; init; }
        }
    }

    public class PublishingScenarioItem
    {
        public required string Description { get; init; }
        public required string Type { get; init; }
        public required string Text { get; init; }
        public string? XConsumerKey { get; init; }
        public string? XConsumerSecret { get; init; }
        public string? XAccessToken { get; init; }
        public string? XAccessTokenSecret { get; init; }
        public int? XTextSizeLimit { get; init; }
        public string? XTextSizeLimitExceededAlternativeText { get; init; }
    }
}

public class ScenarioLoader
{
    private const string ConfigFileName = "Scenario.json";

    private static readonly JsonSerializerOptions JsonDeserializeOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    public static Scenario Load()
    {
        var scenarioJson = File.ReadAllText(ConfigFileName);
        var scenario = JsonSerializer.Deserialize<Scenario>(scenarioJson, JsonDeserializeOptions) ?? throw new JsonException("Invalid scenario json.");
        Logger.Info("Scenario loaded successfully.");
        return scenario;
    }
}
