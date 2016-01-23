using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarkovTweet
{
    public class TwitterAuth
    {
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
    }
}
