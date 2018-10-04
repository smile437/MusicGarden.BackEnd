using System.Threading.Tasks;

namespace MSS.Services.Interfaces
{
    public interface IYouTubeApi
    {
        Task<string> GetVideoIdAsync(string searchTitle, int maxResults);
    }
}
