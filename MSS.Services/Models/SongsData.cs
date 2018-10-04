using MSS.Services.Interfaces.Models;

namespace MSS.Services.Models
{
    public class SongsData : ISongsData
    {
        public string Title { get; set; }
        public string HeaderImageUrl { get; set; }
        public string Url { get; set; }
        public string TypeAlbumName { get; set; }
        public string PrimaryArtistName { get; set; }
        public string Lyrics { get; set; }
        public string Youtube {get; set;}
        public string Genre { get; set; }
    }
}