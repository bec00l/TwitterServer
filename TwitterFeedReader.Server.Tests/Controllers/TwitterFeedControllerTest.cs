using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterFeedReader.Server.Controllers;
using TwitterFeedReader.Server.Models;

namespace TwitterFeedReader.Server.Tests.Controllers
{
    [TestClass]
    public class TwitterFeedControllerTest
    {
        [TestMethod]
        public async Task GetTenTweets()
        {
            TwitterFeedController controller = new TwitterFeedController();
            IEnumerable<Tweet> result = await controller.Get(10, "salesforce", "");
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Count());
        }

        [TestMethod]
        public async Task GetWithBadFilter()
        { 
            TwitterFeedController controller = new TwitterFeedController();
            IEnumerable<Tweet> result = await controller.Get(10, "salesforce", "david hurd is awesome");
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }
    }
}
