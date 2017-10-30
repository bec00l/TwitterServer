using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwitterFeedReader.Server.Models
{
    public class User
    {
        public string Name { get; set; }
        public string Handle { get; set; }
        public string ProfileImage { get; set; }
    }
}