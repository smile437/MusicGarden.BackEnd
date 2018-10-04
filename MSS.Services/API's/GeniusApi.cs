using System.Collections.Generic;
using System.Threading.Tasks;
using Genius;
using MSS.Services.Interfaces;
using MSS.Services.Interfaces.Models;
using MSS.Services.Models;

namespace MSS.Services
{
    public class GeniusApi : IGeniusApi
    {
        #region FIELDS

        private readonly string accessToken;
        private readonly ILastFmApi lastFmApi;
        private readonly IYouTubeApi youTubeApi;

        #endregion

        #region CONSTRUCTORS

        public GeniusApi(string accessToken, IYouTubeApi youTube, ILastFmApi lastFm, string clientId, string clientSecret)
        {
            this.accessToken = accessToken;
            this.youTubeApi = youTube;
            this.lastFmApi = lastFm;

            Authenticator.ClientId = clientId;
            Authenticator.ClientSecret = clientSecret;
            Authenticator.RedirectUri = "";
            Authenticator.Scope = "me create_annotation manage_annotation vote";
            Authenticator.State = "default_state";
            ContentRetriever.AuthorizationToken = accessToken;
        }

        #endregion

        #region METHODS

        public async Task<List<ISongsData>> RetrieveSongs(string id)
        {
            Search.AuthenticationToken = this.accessToken;
            Search.SearchTerm = id;

            var hits = await Search.DoSearch();
            var result = new List<ISongsData>();

            foreach (var hit in hits)
            {
                var hr = hit.Result;
                hr.Album = new Album { Name = await this.lastFmApi.GetAlbumName(hr.Title, hr.PrimaryArtist.Name) };
                var lyric = await this.lastFmApi.GetLyrics(hr.Title, hr.PrimaryArtist.Name);
                var video = await this.youTubeApi.GetVideoIdAsync(hr.FullTitle, 1);
                var genre = await this.lastFmApi.GetGenre(hr.Title, hr.PrimaryArtist.Name);
                var data = new SongsData
                {
                    Title = hr.FullTitle.Replace("\"", "\'"),
                    HeaderImageUrl = hr.HeaderImageUrl,
                    Url = hr.Url,
                    TypeAlbumName = hr.Album.Name,
                    PrimaryArtistName = hr.PrimaryArtist.Name,
                    Lyrics = lyric,
                    Youtube = video,
                    Genre = genre
                };
                result.Add(data);
            }
            return result;
        }

        #endregion
    }
}