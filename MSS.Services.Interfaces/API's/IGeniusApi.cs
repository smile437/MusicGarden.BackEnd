using System.Collections.Generic;
using System.Threading.Tasks;
using MSS.Services.Interfaces.Models;

namespace MSS.Services.Interfaces
{
    public interface IGeniusApi
    {
        Task<List<ISongsData>> RetrieveSongs(string id);
    }
}
