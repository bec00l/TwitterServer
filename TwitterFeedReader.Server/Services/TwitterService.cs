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
        private static String UserTimelineBaseUrl = "https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name={0}&count={1}";



        public static async Task<string> GetAccessToken()
        {
            const string AUTHORIZATION_STRING = "Authorization";
            const string ACCESS_TOKEN_STRING = "access_token";
            const string BASIC_STRING = "Basic ";
            const string ENCODING_STRING = "application/x-www-form-urlencoded";
            const string GRANT_TYPE_STRING = "grant_type=client_credentials";
            const string SEPARATOR_STRING = ":";
            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, OAuthBaseUrl))
                {

                    var customerInfo = Convert.ToBase64String(new UTF8Encoding().GetBytes(OAuthAPIKey + SEPARATOR_STRING + OAuthAPISecret));

                    request.Headers.Add(AUTHORIZATION_STRING, BASIC_STRING + customerInfo);

                    request.Content = new StringContent(GRANT_TYPE_STRING, Encoding.UTF8, ENCODING_STRING);

                    HttpResponseMessage response = await httpClient.SendAsync(request);

                    string jsonResponseMessage = await response.Content.ReadAsStringAsync();

                    dynamic parsedResponse = new JavaScriptSerializer().Deserialize<object>(jsonResponseMessage);

                    return parsedResponse[ACCESS_TOKEN_STRING];
                }
            }

        }



        public static async Task<IEnumerable<Tweet>> GetTweets(string userName, int count, string filter = null, string accessToken = null)
        {
            const string AUTHORIZATION_STRING = "Authorization";
            const string TEXT_STRING = "text";
            const string USER_STRING = "user";
            const string NAME_STRING = "name";
            const string SCREENNAME_STRING = "screen_name";
            const string ID_STRING = "id";
            const string BEARER_STRING = "Bearer ";

            if (String.IsNullOrWhiteSpace(accessToken))
            {
                accessToken = await GetAccessToken();
            }
            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpRequestMessage requestUserTimeline = new HttpRequestMessage(HttpMethod.Get, string.Format(UserTimelineBaseUrl, userName, count)))
                {
                    requestUserTimeline.Headers.Add(AUTHORIZATION_STRING, BEARER_STRING + accessToken);

                    HttpResponseMessage responseUserTimeLine = await httpClient.SendAsync(requestUserTimeline);

                    dynamic jsonResponse = new JavaScriptSerializer().Deserialize<object>(await responseUserTimeLine.Content.ReadAsStringAsync());

                    var tweetCollection = (jsonResponse as IEnumerable<dynamic>);

                    if (tweetCollection == null)
                    {
                        return null;
                    }

                    if (!String.IsNullOrWhiteSpace(filter))
                    {
                        tweetCollection = tweetCollection.Where(t => t[TEXT_STRING] != null && t[TEXT_STRING].ToString().Contains(filter));
                    }

                    return tweetCollection.Select(t => new Tweet()
                    {
                        User = new User()
                        {
                            Name = t[USER_STRING][NAME_STRING],
                            Handle = t[USER_STRING][SCREENNAME_STRING]
                        },
                        TweetId = t[ID_STRING]
                    });
                }
            }
        }
    }
}