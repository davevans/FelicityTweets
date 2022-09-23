using FelicityTweets.Model;

namespace FelicityTweets
{
    public class TweetRepository : BackgroundService
    {
        private readonly HttpClient _httpClient;
        private List<MinimumTweet>? _tweets;
        private readonly TimeSpan _waitTime = TimeSpan.FromHours(1);

        private string _userId;

        public TweetRepository(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("twitter");
            _userId = configuration["USER_ID"];
        }

        public List<MinimumTweet> Tweets => _tweets ?? (_tweets = new List<MinimumTweet>());

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                _tweets = await GetTweets();

                await Task.Delay(_waitTime, stoppingToken);
            }
        }

        private async Task<List<MinimumTweet>> GetTweets()
        {
            var results = new List<Tweet>();
            var hasMore = true;
            var maxPages = 10;
            var currentPage = 0;
            var pageToken = string.Empty;

            while (hasMore && currentPage < maxPages)
            {
                currentPage++;

                var baseUrl = $"2/users/{_userId}/tweets?exclude=retweets&max_results=50&tweet.fields=entities";

                var url = string.IsNullOrEmpty(pageToken)
                    ? baseUrl
                    : $"{baseUrl}&pagination_token={pageToken}";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    throw new Exception(body);
                }

                response.EnsureSuccessStatusCode();
                var tweetResponse = await response.Content.ReadFromJsonAsync<TweetsResponse>();
                if (tweetResponse?.Data != null)
                {
                    results.AddRange(tweetResponse.Data);
                }

                hasMore = !string.IsNullOrWhiteSpace(tweetResponse?.MetaData?.NextToken);
                pageToken = tweetResponse?.MetaData?.NextToken;
            }

            return results.Select(t =>
            {

                var tags = t.Entities?.HashTags?.Select(y => y.Tag).ToArray();

                return new MinimumTweet
                {
                    Id = t.Id,
                    Text = t.Text,
                    HashTags = tags
                };

            }).ToList();
        }
    }
}
