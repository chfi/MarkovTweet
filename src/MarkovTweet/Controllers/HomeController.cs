using System.Collections.Generic;
using System.Linq;
using LinqToTwitter;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;

namespace MarkovTweet.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _accessToken;
        private readonly string _accessTokenSecret;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;

        public HomeController(IOptions<TwitterAuth> authOptions)
        {
            _consumerKey = authOptions.Value.ConsumerKey;
            _consumerSecret = authOptions.Value.ConsumerSecret;
            _accessToken = authOptions.Value.AccessToken;
            _accessTokenSecret = authOptions.Value.AccessTokenSecret;
        }

        public IActionResult Index(string screenname, string order)
        {
            // TODO handle hashtags separately
            int markovOrder;
            // default order is 2, if the parse failed
            if (!int.TryParse(order, out markovOrder))
                markovOrder = 2;

            // hack to avoid null pointer exception
            if (markovOrder > 5)
                markovOrder = 5;
            int tweetCount = 200;
            var generator = new MarkovGenerator(markovOrder);

            ViewData["Screenname"] = screenname;

            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = _consumerKey,
                    ConsumerSecret = _consumerSecret,
                    AccessToken = _accessToken,
                    AccessTokenSecret = _accessTokenSecret
                }
            };

            var twitterCtx = new TwitterContext(auth);
            var timeline =
                (twitterCtx.Status.Where(tweet => tweet.Type == StatusType.User &&
                                                  tweet.ScreenName == screenname &&
                                                  tweet.Count == tweetCount))
                    .ToList();

            foreach (var tweet in timeline)
            {
                generator.ReadInput(tweet.Text);
            }

            int outputCount = 20;
            var outputList = new List<string>();
            for (int i = 0; i < outputCount; i++)
            {
                outputList.Add(generator.GenerateOutput());
            }

            return View(outputList);
        }
        
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}