using System.Text;
using Newtonsoft.Json;
using Tweetinvi;
using Tweetinvi.Core.Web;
using Tweetinvi.Models;

namespace ScrapeAndPublish;

public class Publisher
{

    public static async Task<bool> Publish(Context context, Scenario.PublishingScenarioItem scenarioItem)
    {
        switch (scenarioItem.Type)
        {
            case "x":
                // Xへポスト

                var text = Util.ResolveVariable(scenarioItem.Text, context.Variables);
                if (scenarioItem.XTextSizeLimit is not null && scenarioItem.XTextSizeLimitExceededAlternativeText is not null &&
                    scenarioItem.XTextSizeLimit < text.Length)
                {
                    // 最大文字数超過時の代替テキストで置き換え
                    text = scenarioItem.XTextSizeLimitExceededAlternativeText;
                }
                var client = new TwitterClient(
                    scenarioItem.XConsumerKey, scenarioItem.XConsumerSecret, scenarioItem.XAccessToken, scenarioItem.XAccessTokenSecret);
                var poster = new TweetsV2Poster(client);
                var result = await poster.PostTweet(new TweetV2PostRequest { Text = text });
                Logger.Info("Posted successfully. {0}", result.Content);
                break;
        }
        return true;
    }

    private class TweetsV2Poster(ITwitterClient client)
    {
        private readonly ITwitterClient client = client;

        public Task<ITwitterResult> PostTweet(TweetV2PostRequest request)
        {
            return client.Execute.AdvanceRequestAsync((ITwitterRequest twitterRequest) =>
            {
                var json = client.Json.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                twitterRequest.Query.Url = "https://api.twitter.com/2/tweets";
                twitterRequest.Query.HttpMethod = Tweetinvi.Models.HttpMethod.POST;
                twitterRequest.Query.HttpContent = content;
            });
        }
    }

    private class TweetV2PostRequest
    {
        [JsonProperty("text")]
        public required string Text { get; init; }
    }
}
