using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using TwitterFeedReader.Server.Models;
using TwitterFeedReader.Server.Services;

namespace TwitterFeedReader.Server.Controllers
{
    [RoutePrefix("api/feed")]
    [EnableCors(origins: "http://twitterfeedreaderclient20171030123542.azurewebsites.net", headers: "*", methods: "*")]
    public class TwitterFeedController : ApiController

    {

        [Route("{count:int}/{userName:alpha}/{filter:alpha?}")]

        public async Task<IEnumerable<Tweet>> Get(int count, string userName = "salesforce", string filter = "")

        {

            var tweets = await TwitterService.GetTweets(userName, count, filter, string.Empty);

            return tweets;

        }



    }
}
