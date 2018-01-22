using FWUtils.PCL.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FWUtils.PCL.Interfaces
{
    public interface IHttpClient<TReturnType>
    {
        #region -- Response --
        HttpResponseHeaders ResponseHeader { get; set; }
        TReturnType Result { get; set; }
        HttpStatusCode ResponseCode { get; set; }
        bool Succeed { get; set; }
        string BadRequestResult { get; set; }
        HttpResponseStatusEnum ResponseStatus { get; set; }
        #endregion

        Dictionary<string, string> Parameters { get; set; }

        //void AttachAthorization(HttpClient httpClient);

        TReturnType Get(string controller, string action, bool isAPI = true);

        TReturnType Post<tdata>(string controller, string action, tdata data = default(tdata), bool containsBytesToUpload = false);

        TReturnType Delete(string controller, string action);

        TReturnType Ping(TimeSpan ts, string controller = "associate", string action = "ping", bool isAPI = true);

        Task<TReturnType> TokenPost();

        //TReturnType RenderRequestResult(HttpResponseMessage Response, String url, Func<TReturnType> OnUnauthorized, string action = "");
    }
}
