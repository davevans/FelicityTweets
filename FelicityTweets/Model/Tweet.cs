using System.Text.Json.Serialization;

namespace FelicityTweets.Model
{
    public class Tweet
    {
        public string? Id { get; set; }
        public string? Text { get; set; }
        public TweetEntities? Entities { get; set; }
    }

    public class TweetEntities
    {
        [JsonPropertyName("hashtags")]
        public TweetHashTag[]? HashTags { get; set; }

        public TweetEntities()
        {
            HashTags = Enumerable.Empty<TweetHashTag>().ToArray();
        }

    }

    public class TweetMentions
    {
        public string? Username { get; set; }
    }

    public class TweetHashTag
    {
        public string? Tag { get; set; }
    }

    public class TweetResponseMetaData
    {
        [JsonPropertyName("results_count")]
        public int ResultsCount { get; set; }

        [JsonPropertyName("newest_id")]
        public string? NewestId { get; set; }

        [JsonPropertyName("oldest_id")]
        public string? OldestId { get; set; }

        [JsonPropertyName("next_token")]
        public string? NextToken { get; set; }
    }

    public class TweetsResponse
    {
        public List<Tweet>? Data { get; set; }

        [JsonPropertyName("meta")]
        public TweetResponseMetaData? MetaData{ get; set; }
    }


    /// <summary>
    /// A consise tweet
    /// </summary>
    public class MinimumTweet
    {
        public string? Id { get; set; }
        public string? Text { get; set; }
        public string[]? HashTags { get; set; }
    }
}
