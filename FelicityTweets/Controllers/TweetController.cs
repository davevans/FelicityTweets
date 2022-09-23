using Microsoft.AspNetCore.Mvc;

namespace FelicityTweets.Controllers
{
    [ApiController]
    [Route("")]
    public class TweetController : ControllerBase
    {
        private readonly TweetRepository _tweetRepository;

        public TweetController(TweetRepository tweetRepository)
        {
            _tweetRepository = tweetRepository;
        }

        [Route("")]
        [HttpGet]
        public IActionResult Get()
        {
            var queryHashTag = Request.Query["hashtag"];
            var hashTag = queryHashTag.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(hashTag))
            {
                return Ok(_tweetRepository.Tweets);
            }

            var filtered = _tweetRepository.Tweets.Where(t => t.HashTags != null && t.HashTags.Contains(hashTag, StringComparer.OrdinalIgnoreCase)).ToList();
            return Ok(filtered);
        }
    }
}
