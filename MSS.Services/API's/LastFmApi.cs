using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using IF.Lastfm.Core.Api;
using MSS.Services.Interfaces;

namespace MSS.Services
{
    public class LastFmApi : ILastFmApi
    {
        private readonly LastfmClient lastFmClient;

        public LastFmApi(string key, string secret)
        {
            //todo: check for invalid params
            this.lastFmClient = new LastfmClient(key, secret);
        }

        public async Task<string> GetLyrics(string track, string artist)
        {
            var response = await this.lastFmClient.Track.GetInfoAsync(track, artist);

            if (response.Success || !ReferenceEquals(response.Content, null))
            {
                var page = new HtmlWeb().Load(response.Content.Url.OriginalString)
                    .DocumentNode
                    .Descendants("a")
                    .Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Equals("external-link"));
                var htmlNodes = page.ToList();
                if (htmlNodes.Any())
                {
                    page = new HtmlWeb().Load(htmlNodes.First()
                        .GetAttributeValue("href", "empty"))
                        .DocumentNode.Descendants("p")
                        .Where(d => d.Attributes.Contains("class") &&
                        d.Attributes["class"].Value.Contains("verse"));
                    var lyricsCollection = page.Select(x => x.InnerText);
                    var res = lyricsCollection.Aggregate("", (current, row) => current + "\n" + row).Replace("\"", "\'");
                    return res;
                }
            }

            return "";
        }

        public async Task<string> GetGenre(string track, string artist)
        {
            var response = await this.lastFmClient.Track.GetInfoAsync(track, artist);
            if (response.Success || !ReferenceEquals(response.Content, null))
            {
                return response.Content.TopTags.ToList().Any() ? response.Content.TopTags.ToList()[0].Name : "";
            }

            return "";
        }

        public async Task<string> GetAlbumName(string track, string artist)
        {
            var response = await this.lastFmClient.Track.GetInfoAsync(track, artist);
            if (response.Success || !ReferenceEquals(response.Content, null))
            {
                var page = new HtmlWeb().Load(response.Content.Url)
                    .DocumentNode
                    .Descendants("h3")
                    .Where(d => d.Attributes.Contains("class") &&
                                d.Attributes["class"].Value.Equals("featured-item-name"))
                    .ToList();

                if (page.Any())
                {
                    return page[0].InnerText;
                }
            }

            return "no info";
        }
    }
}