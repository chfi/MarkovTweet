using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;

using LinqToTwitter;
using Microsoft.Extensions.OptionsModel;

namespace MarkovTweet.Controllers
{
    public class HomeController : Controller
    {
        private readonly string consumerKey;
        private readonly string consumerSecret;
        private readonly string accessToken;
        private readonly string accessTokenSecret;

        public HomeController(IOptions<TwitterAuth> authOptions)
        {
            consumerKey = authOptions.Value.ConsumerKey;
            consumerSecret = authOptions.Value.ConsumerSecret;
            accessToken = authOptions.Value.AccessToken;
            accessTokenSecret = authOptions.Value.AccessTokenSecret;
        }

        public IActionResult Index(string screenname)
        {
            int markovOrder = 2;
            int tweetCount = 100;
            var generator = new MarkovGenerator(markovOrder);
            
            ViewData["Screenname"] = screenname;
            
            // read from secrets.json
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret,
                    AccessToken = accessToken,
                    AccessTokenSecret = accessTokenSecret
                }
            };

            var twitterCtx = new TwitterContext(auth);
            var timeline =
                (from tweet in twitterCtx.Status
                    where tweet.Type == StatusType.User &&
                          tweet.ScreenName == screenname &&
                          tweet.Count == tweetCount
                    select tweet)
                    .ToList();
          
            foreach (var tweet in timeline)
            {
                generator.ReadInput(tweet.Text);
            }

            int outputCount = 10;
            List<string> outputList = new List<string>();
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
