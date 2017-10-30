using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwitterFeedReader.Server.Models
{
    public class Tweet
    {
        public User User { get; set; }
        public long TweetId { get; set; }
        public string TweetUrl
        {
            get
            {
                string formattedUrl = String.Format("https://twitter.com/{0}/status/{1}", Uri.EscapeDataString(User.Name), TweetId);
                return Uri.EscapeUriString(formattedUrl);
            }
        }
    }
}