using System.Threading.Tasks;

namespace MSS.Services.Interfaces
{
    public interface ILastFmApi
    {
        Task<string> GetLyrics(string track, string artist);
        Task<string> GetGenre(string track, string artist);
        Task<string> GetAlbumName(string track, string artist);
    }
}
