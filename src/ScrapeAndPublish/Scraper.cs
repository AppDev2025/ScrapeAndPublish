using AngleSharp;
using Newtonsoft.Json.Linq;

namespace ScrapeAndPublish;

public partial class Scraper
{
    private static readonly HttpClient httpClient = new();


    public static async Task<bool> ScrapeAsync(Context context, Scenario.ScrapingScenarioItem scenarioItem)
    {
        // URLに含まれる変数を解決
        var url = Util.ResolveVariable(scenarioItem.Url, context.Variables);

        switch (scenarioItem.Type)
        {
            case "html":
                // HTMLスクレイピング

                // AngleSharpを初期化
                var configuration = Configuration.Default.WithDefaultLoader();
                var browsingContext = BrowsingContext.New(configuration);
                // URLにアクセスしドキュメントを読み込み
                var document = await browsingContext.OpenAsync(url);
                // セレクタを使用してテキストを抽出
                foreach (var item in scenarioItem.Items)
                {
                    var rawText = item.Target switch
                    {
                        // target=attributeの場合、属性値
                        "attribute" => document.QuerySelector(item.Selector)?.GetAttribute(item.AttributeName!) ?? String.Empty,
                        _ => document.QuerySelector(item.Selector)?.TextContent ?? String.Empty,
                    };
                    context.Variables[item.Name] = Util.ResolveRegex(rawText, item.Regex, item.RegexCaptureIndex);
                }
                break;
            case "json":
                // JSONスクレイピング

                // URLにアクセスしJSONを取得
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Accept", "application/json");
                var response = await httpClient.SendAsync(request);
                // JSONオブジェクトに変換
                var jsonObject = JObject.Parse(await response.Content.ReadAsStringAsync());
                // セレクタを使用してテキストを抽出
                foreach (var item in scenarioItem.Items)
                {
                    var rawText = jsonObject.SelectToken(item.Selector)?.Value<string>() ?? String.Empty;
                    context.Variables[item.Name] = Util.ResolveRegex(rawText, item.Regex, item.RegexCaptureIndex);
                }
                break;
        }

        return true;
    }
}
