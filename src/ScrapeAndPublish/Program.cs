using ScrapeAndPublish;

// シナリオファイルを読み込む
var scenario = ScenarioLoader.Load();

// コンテキストを作成
var context = new Context();

// スクレイピング
foreach (var scrapingScenario in scenario.ScrapingScenario)
{
    var result = await Scraper.ScrapeAsync(context, scrapingScenario);

    if (!result)
    {
        Logger.Info("Scraping failed: {0}", scrapingScenario.Url);
        break;
    }
}

foreach (var pair in context.Variables)
{
    Logger.Info("{0}: {1}", pair.Key, pair.Value);
}

// パブリッシング
foreach (var publishingScenario in scenario.PublishingScenario)
{
    var result = await Publisher.Publish(context, publishingScenario);

    if (!result)
    {
        Logger.Info("Publishing failed: {0}", publishingScenario.Type);
        break;
    }
}
