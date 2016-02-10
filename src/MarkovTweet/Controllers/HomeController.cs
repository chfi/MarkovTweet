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
            var generator = new MarkovGenerator(markovOrder, false);
            
            // how to handle hashtags? just keep a list of them along with a probability for each,
            // i.e. a single-prefix markov chain?
            // would also need a no-hashtag probability
            // no, that won't work, since there wouldn't be an end point, really. at least not one that makes sense.
            
            // what might work is keeping track of individual hashtag probabilities,
            // as well as a probability distribution of number of hashtags. (fun to get some stats on that, see if it's poisson-distributed)

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

            //TODO add form
            //TODO use form data
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