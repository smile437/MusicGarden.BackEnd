using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using MSS.Services.Interfaces;

namespace MSS.Services
{
    public class YouTubeApi : IYouTubeApi
    {
        private YouTubeService youtubeService;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string appName;

        public YouTubeApi(string clientId, string clientSecret, string appName)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.appName = appName;
        }

        public async Task<string> GetVideoIdAsync(string searchTitle, int maxResults)
        {
            if (ReferenceEquals(this.youtubeService, null))
            {
                this.youtubeService = new YouTubeService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = await this.AuthorizeAsync(),
                    ApplicationName = this.appName
                });
            }

            if (searchTitle.Contains('('))
            {
                var openBracketIndex = 0;
                var isBracketWasFound = false;

                foreach (var letter in searchTitle)
                {
                    if (letter.Equals('('))
                    {
                        openBracketIndex = searchTitle.IndexOf(letter);
                        isBracketWasFound = true;
                    }

                    if (letter.Equals(')') && isBracketWasFound)
                    {
                        var length = searchTitle.IndexOf(letter) - openBracketIndex;
                        searchTitle = searchTitle.Remove(openBracketIndex, length + 1);

                        isBracketWasFound = false;
                        openBracketIndex = 0;
                    }
                }
            }

            var listRequest = this.youtubeService.Search.List("snippet");
            listRequest.Q = searchTitle;
            listRequest.MaxResults = maxResults;
            listRequest.Type = "video";
            var resp = await listRequest.ExecuteAsync();
            return resp.Items.Any() ? resp.Items[0].Id.VideoId : "";
        }

        private async Task<UserCredential> AuthorizeAsync()
        {
            return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = this.clientId,
                    ClientSecret = this.clientSecret,
                },
                null,
                "user",
                CancellationToken.None,
                new FileDataStore("YouTube.Auth.Store"));
        }

    }
}