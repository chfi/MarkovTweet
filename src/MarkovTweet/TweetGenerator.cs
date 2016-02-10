using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.Extensions.OptionsModel;

namespace MarkovTweet
{
    public class TweetGenerator
    {
        private MarkovGenerator generator;
        private int markovOrder;

        private string screenname;
        private int tweetCount;

        private SingleUserAuthorizer auth;

        private List<string> hashtagList;

        private List<string> mentionList; 

        //private int tweetCount;

        //var generator = new MarkovGenerator(markovOrder, false);

        public TweetGenerator(SingleUserAuthorizer auth, string screenname, int count, int order)
        {
            this.auth = auth;
            this.generator = new MarkovGenerator(order, false);
            this.screenname = screenname;
            this.tweetCount = count;
        }

        //TODO all of this shit
        // this will be instantiated every time the page is refreshed.
        // that is, there exists one of these per twitter user (nominally)

        // this needs a screenname,
        // a generator,
        // a list of hashtags
        // a hashtag probability distribution
        // a list of mentions
        // a mention probability distribution

        // this will be given a twitter screenname, and pull everything down on its own.
        // so this needs one method for extracting hashtags and hashtag probabilities,
        // one for extracting mentions and mention probabilities,
        // one for removing hashtags and mentions from the twitter data, so the data can be shoved into the markov chain.

        //TODO create a class for twitter auth and save it somewhere

    }
}
