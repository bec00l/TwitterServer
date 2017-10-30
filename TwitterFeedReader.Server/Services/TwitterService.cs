using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using TwitterFeedReader.Server.Models;

namespace TwitterFeedReader.Server.Services
{
    public static class TwitterService
    {
        private static readonly String OAuthBaseUrl = "https://api.twitter.com/oauth2/token";



        private static readonly String OAuthAPIKey = "zIb9cf1iMIs6jC3ZWa0kwe0Eu";



        private static readonly String OAuthAPISecret = "3jJs8MxpdDzpDg1Vp9iojNNTxirlIZp5sATk3VPfjFDvU1NLhD";



        private static String UserTimelineBaseUrl = "https://api.twitter.com/1.1/statuses/user_timeline.json?count={0}&screen_name={1}&trim_user={2}&exclude_replies={3}";


        public static async Task<string> GetAccessToken()

        {

            using (HttpClient httpClient = new HttpClient())

            {

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, OAuthBaseUrl))

                {

                    var customerInfo = Convert.ToBase64String(new UTF8Encoding()

                                         .GetBytes(OAuthAPIKey + ":" + OAuthAPISecret));

                    request.Headers.Add("Authorization", "Basic " + customerInfo);

                    request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8,

                                                                              "application/x-www-form-urlencoded");



                    HttpResponseMessage response = await httpClient.SendAsync(request);

                    string jsonResponseMessage = await response.Content.ReadAsStringAsync();

                    dynamic parsedResponse = new JavaScriptSerializer().Deserialize<object>(jsonResponseMessage);

                    return parsedResponse["access_token"];

                }



            }

        }



        public static async Task<IEnumerable<Tweet>> GetTweets(string userName, int count, string filter = null, string accessToken = null)

        {

            if (String.IsNullOrWhiteSpace(accessToken))

            {

                accessToken = await GetAccessToken();

            }



            using (HttpClient httpClient = new HttpClient())

            {
                using (HttpRequestMessage requestUserTimeline = new HttpRequestMessage(HttpMethod.Get, string.Format(UserTimelineBaseUrl, count, userName, 0, 1)))

                {



                    requestUserTimeline.Headers.Add("Authorization", "Bearer " + accessToken);



                    HttpResponseMessage responseUserTimeLine = await httpClient.SendAsync(requestUserTimeline);



                    dynamic jsonResponse = new JavaScriptSerializer().Deserialize<object>(await responseUserTimeLine.Content.ReadAsStringAsync());



                    var tweetCollection = (jsonResponse as IEnumerable<dynamic>);



                    if (tweetCollection == null)

                    {

                        return null;

                    }

                    if (!String.IsNullOrWhiteSpace(filter))

                    {

                        tweetCollection = tweetCollection.Where(t => t["text"] != null && t["text"].ToString().Contains(filter));

                    }

                    return tweetCollection.Select(t => new Tweet()

                    {
                        User = new User()
                        {
                            Name = t["user"]["name"],
                            Handle = t["user"]["screen_name"],
                            ProfileImage = t["user"]["profile_image_url"]
                        },
                        TweetId = t["id"]
                    });

                }



            }
        }
    }
}