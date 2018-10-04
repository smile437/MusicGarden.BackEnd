using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using MSS.Helpers;
using MSS.Services;
using MSS.Services.Interfaces;
using MSS.Services.Interfaces.Models;

namespace MSS.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly IGeniusApi geniusApi;

        public ValuesController()
        {
            var youTubeApi = new YouTubeApi(AppConsts.OAuthGoogleId, AppConsts.OAuthGoogleSecret, AppConsts.AppName);
            var lastFmApi = new LastFmApi(AppConsts.LastfmApiKey, AppConsts.LastfmApiSecret);
            this.geniusApi = new GeniusApi(AppConsts.GeniusAccessToken, youTubeApi, lastFmApi, AppConsts.GeniusClientId, AppConsts.GeniusClientSecret);
        }

        // GET api/values/
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }

        // GET api/values/5
        public async Task<JsonResult<List<ISongsData>>> Get(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return this.Json(new List<ISongsData>());
            }

            if (id.Contains(")("))
            {
                id = id.Replace(")(", ".");
            }

            var result = await this.geniusApi.RetrieveSongs(id);

            return this.Json(result);
        }


    }
}
