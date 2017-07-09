using InstaGramLikeTake2;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using Microsoft.Azure.Graphs.Elements;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TestMobileAppForAuth
{
    public partial class authenticateinstagram : System.Web.UI.Page
    {
        static string code = string.Empty;
        DocumentClient client;
        protected void Page_Load(object sender, EventArgs e)
        {
            string endpoint = ConfigurationManager.AppSettings["Graph.Endpoint"];
            string authKey = ConfigurationManager.AppSettings["Graph.AuthKey"];

            client = new DocumentClient(
                new Uri(endpoint),
                authKey,
                new ConnectionPolicy { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Tcp });

            if (!String.IsNullOrEmpty(Request["code"]) && !Page.IsPostBack)
            {
                code = Request["code"].ToString();
                GetDataInstagramToken();
            }
        }

        //Function used to get instagram user id and access token  
        public void GetDataInstagramToken()
        {
            var json = "";
            try
            {
                NameValueCollection parameters = new NameValueCollection();
                parameters.Add("client_id", ConfigurationManager.AppSettings["instagram.clientid"].ToString());
                parameters.Add("client_secret", ConfigurationManager.AppSettings["instagram.clientsecret"].ToString());
                parameters.Add("grant_type", "authorization_code");
                parameters.Add("redirect_uri", ConfigurationManager.AppSettings["instagram.redirecturi"].ToString());
                parameters.Add("code", code);

                WebClient client = new WebClient();
                var result = client.UploadValues("https://api.instagram.com/oauth/access_token", "POST", parameters);
                var response = System.Text.Encoding.Default.GetString(result);

                // deserializing nested JSON string to object  
                var jsResult = (JObject)JsonConvert.DeserializeObject(response);
                string accessToken = (string)jsResult["access_token"];
                ConfigurationManager.AppSettings.Set(Constants.InstagramAccessToken, accessToken);
                long id = (long)jsResult["user"]["id"];

                //This code register id and access token to get on client side  
                Page.ClientScript.RegisterStartupScript(this.GetType(), "GetToken", "<script>var instagramaccessid=\"" + @"" + id + "" + "\"; var instagramaccesstoken=\"" + @"" + accessToken + "" + "\";</script>");

                string resultString = string.Empty;
                // Shows an example how you get information of the current user
                Task getUserSelf = new Task(() =>
                {
                    resultString = ExecuteTestCode().Result;
                });
                getUserSelf.Start();
                getUserSelf.Wait();
                    Response.Write(resultString);
                
            }
            catch (Exception ex)
            {
                throw;

            }
        }

        public async Task ExecuteGraphQuery(string graphQuery)
        {
            // The CreateGremlinQuery method extensions allow you to execute Gremlin queries and iterate
            // results asychronously
            IDocumentQuery<dynamic> query = client.CreateGremlinQuery<dynamic>(graph, graphQuery);

            while (query.HasMoreResults)
            {
                foreach (dynamic result in await query.ExecuteNextAsync())
                {
                    Console.WriteLine($"\t {JsonConvert.SerializeObject(result)}");
                }
            }
        }

        public async Task CreateGraphUser(InstagramUser user)
        {
            await CreateGraphUser(user.Data.Id, user.Data.Full_name, user.Data.Counts.Follows, user.Data.Counts.Followed_by);
        }

        public async Task CreateGraphUser(string id, string fullName, int follows, int followedBy)
        {
            string query = string.Format("g.addV('User').property('id', '{0}').property('FullName', '{1}').property('Follows', '{2}').property('FollowedBy', '{3}')",
                    id, fullName, follows, followedBy);

            await ExecuteGraphQuery(query);
        }

        public async Task CreateGraphMediaItem(MediaContainer mediaContainer, string userId)
        {
            string query = string.Format("g.addV('MediaItem').property('id', '{0}').property('OwnerId', {1}).property('UserId', {2})", mediaContainer.Id, mediaContainer.User.Id, userId);
            await ExecuteGraphQuery(query);
        }

        public async Task CreateLikeGraphEdge(string UserId, string mediaId)
        {
            string query = string.Format("g.V('{0}').addE('Likes').to(g.V('{1}'))", UserId, mediaId);
            await ExecuteGraphQuery(query);
        }

        public async Task CreateNewGraphDb()
        {
            Database database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "graphdb" });

            graph = client.CreateDocumentCollectionIfNotExistsAsync(
            UriFactory.CreateDatabaseUri("graphdb"),
            new DocumentCollection { Id = "instagram" },
            new RequestOptions { OfferThroughput = 1000 }).Result;

            await ExecuteGraphQuery("g.V().drop()");
        }

        public async Task<InstagramMediaList> GetTaggedMedia(string tag)
        {
            InstagramMediaList media = null;
            try
            {
                string jsonResponse = await InstagramAPI.GetTagMedia(tag);
                media = InstagramMediaList.CreateFromJsonResponse(jsonResponse);

                Debug.WriteLine("Filtered Media : {0}", media.Data.Count);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failure getting recent tags {0}", ex.Message);
            }

            return media;
        }

        public async Task<InstagramMediaList> GetMediaSelfLiked()
        {
            InstagramMediaList media = null;
            try
            {
                string jsonResponse = await InstagramAPI.GetUserMediaSelfLiked();
                media = InstagramMediaList.CreateFromJsonResponse(jsonResponse);

                Debug.WriteLine("Filtered Media : {0}", media.Data.Count);
                foreach (var item in media.Data)
                {
                    Debug.WriteLine("UserName {0}", item.User.Username);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failure getting recent tags {0}", ex.Message);
            }

            return media;
        }

        DocumentCollection graph = null;

        public async Task<string> ExecuteTestCode()
        {
            StringBuilder message = new StringBuilder();

            // Shows an example how you get information of the current user
            await CreateNewGraphDb();

            string jsonResponse = await InstagramAPI.GetUserSelf();
            InstagramUser userSelf = InstagramUser.CreateFromJsonResponse(jsonResponse);
            await CreateGraphUser(userSelf);

            var myLikes = await GetMediaSelfLiked();

            var mediaTaggedItems = await GetTaggedMedia("beer");
            Debug.WriteLine("Have Found : {0} tagged Items", mediaTaggedItems.Data.Count);
            foreach(var item in mediaTaggedItems.Data)
            {
                Debug.WriteLine("Created By {0} : Has {1} likes : Has {2} comments", item.User.Full_name, item.Likes.Count, item.Comments.Count);
                await CreateGraphMediaItem(item, userSelf.Data.Id);
                await CreateLikeGraphEdge(userSelf.Data.Id, item.Id);

                if (userSelf.Data.Id == item.User.Id)
                {
                    Debug.WriteLine("This is Me!");
                }
                else
                {
                    await CreateGraphUser(item.User.Id, item.User.Full_name, -1, -1);
                }

                if(item.User_has_liked)
                {
                    Debug.WriteLine("Have already liked this");
                }
                else
                {
                    Debug.WriteLine("Potential item to like");
                }
            }

            return message.ToString();

        }
    }
}