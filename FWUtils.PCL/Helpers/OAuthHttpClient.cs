//using FWUtils.PCL.CustomClasses;
//using FWUtils.PCL.Enumerations;
//using FWUtils.PCL.Interfaces;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;

//namespace FWUtils.PCL.Helpers
//{
//    public class OAuthHttpClient<TReturnType> : IHttpClient<TReturnType>
//    {
//        protected readonly string _endpoint = AppConfiguration.WebAPIEndPoint;  // Set on Application.Start()

//        JsonSerializerSettings settings;

//        #region -- Response --
//        public HttpResponseHeaders ResponseHeader { get; set; }
//        public TReturnType Result { get; set; }
//        public HttpStatusCode ResponseCode { get; set; }
//        public bool Succeed { get; set; } = false;
//        public string BadRequestResult { get; set; }
//        public APIResponse<TReturnType> Response
//        {
//            get
//            {
//                return new APIResponse<TReturnType>()
//                {
//                    Message = BadRequestResult,
//                    Result = Result,
//                    Succeed = Succeed
//                };
//            }
//        }
//        #endregion

//        private Dictionary<string, string> _Parameters;
//        /// <summary>
//        /// Parameters of query string with api request
//        /// </summary>
//        public Dictionary<string, string> Parameters
//        {
//            get
//            {
//                if (_Parameters == null)
//                    _Parameters = new Dictionary<string, string>();
//                return _Parameters;
//            }
//            set
//            {
//                _Parameters = value;
//            }
//        }

//        /// <summary>
//        /// Attaches authorizaion headers
//        /// </summary>
//        /// <param name="httpClient"></param>
//        public void AttachAthorization(HttpClient httpClient)
//        {
//            try
//            {
//                if (IdentityModel.Current != null && IdentityModel.Current.SelectedHierarchyToken != null && !String.IsNullOrEmpty(IdentityModel.Current.SelectedHierarchyToken.access_token))
//                {
//                    if (IdentityModel.Current.SelectedHierarchyToken.HasAccessTokenExpired)
//                    {
//                        var service = new AccountService();
//                        var res = service.RequestRefreshToken().Result;
//                        if (res != null)
//                        {
//                            if (IdentityModel.Current.IsDescendantSelected())
//                                IdentityModel.Current.SetDescendant(res);
//                            else
//                                IdentityModel.Current.SetToken(res);
//                            //IdentityModel.Current.Token = res;
//                            //IdentityModel.Current.Save();                            
//                        }
//                    }

//                    if (IdentityModel.Current != null && IdentityModel.Current.SelectedHierarchyToken != null)
//                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AppConfiguration.Auth.AuthorizationSchema, IdentityModel.Current.AccessToken);
//                }
//            }
//            catch (Exception ex)
//            {
//                DeviceUtilities.LogError(new Exception("AttachAuthorization", ex));
//                throw ex;
//            }
//        }

//        public HttpResponseStatusEnum ResponseStatus { get; set; }

//        public OAuthHttpClient()
//        {
//            settings = new JsonSerializerSettings()
//            {
//                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
//                DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
//            };
//            ResponseStatus = HttpResponseStatusEnum.NONE;
//        }

//        public TReturnType Get(string controller, string action, bool isAPI = true)
//        {
//            String url = controller + "/" + action;
//            try
//            {
//                if (!ConnectivityObserver.Current.IsInternetAvailable || !ConnectivityObserver.Current.IsServiceRunning)
//                    return Result = default(TReturnType);

//                using (var httpClient = new HttpClient())
//                {
//                    httpClient.BaseAddress = new Uri(_endpoint);

//                    AttachAthorization(httpClient);
//                    //if (IdentityModel.Current != null && !String.IsNullOrEmpty(IdentityModel.Current.AccessToken))
//                    //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AppConfiguration.Auth.AuthorizationSchema, IdentityModel.Current.AccessToken);
//                    httpClient.DefaultRequestHeaders.Add("apiv", AppConfiguration.WebAPIVersion);
//                    var timeout = httpClient.Timeout;
//                    httpClient.DefaultRequestHeaders.Accept.Add(
//                    new MediaTypeWithQualityHeaderValue("application/json"));

//                    var str = "";
//                    if (Parameters != null && Parameters.Count > 0)
//                    {
//                        foreach (var item in Parameters)
//                            str += String.Format("{0}={1}{2}", item.Key, System.Net.WebUtility.UrlEncode(item.Value), (Parameters.Last().Key == item.Key) ? "" : "&");
//                    }

//                    url = !String.IsNullOrEmpty(str) ? String.Format("{3}{0}/{1}?{2}", controller, action, str, (isAPI) ? "api/" : "") : string.Format("{2}{0}/{1}", controller, action, (isAPI) ? "api/" : "");

//                    HttpResponseMessage response = httpClient.GetAsync(url).Result;
//                    ResponseHeader = response.Headers;

//                    return RenderRequestResult(response, url, () =>
//                    {
//                        LoginHelper.Logout(false);
//                        return Result = default(TReturnType);
//                    });
//                }
//            }
//            catch (CustomException ex)
//            {
//                DeviceUtilities.LogError(ex);
//            }
//            catch (AggregateException ex)
//            {
//                if (ex.InnerException != null && ex.InnerException.GetType() == typeof(TaskCanceledException))
//                {
//                    ResponseStatus = BaseClientResponseStatus.TIME_OUT;
//                    DeviceUtilities.LogError(new Exception("POST timeout expired", new Exception("Request URI: " + url)));
//                }
//            }
//            catch (Exception ex)
//            {
//                DeviceUtilities.LogError(new Exception("GET Error -> FAILED", new Exception("Request URI: " + url, ex)));
//            }
//            return Result = default(TReturnType);
//        }

//        public TReturnType Post<tdata>(string controller, string action, tdata data = default(tdata), bool containsBytesToUpload = false)
//        {
//            String url = controller + (String.IsNullOrEmpty(action) ? "" : "/" + action);

//            try
//            {
//                if (!ConnectivityObserver.Current.IsInternetAvailable || !ConnectivityObserver.Current.IsServiceRunning)
//                    return Result = default(TReturnType);

//                using (var httpClient = new HttpClient())
//                {
//                    httpClient.BaseAddress = new Uri(_endpoint);

//                    if (containsBytesToUpload)
//                        httpClient.Timeout = new TimeSpan(0, 5, 0);

//                    AttachAthorization(httpClient);

//                    httpClient.DefaultRequestHeaders.Add("apiv", AppConfiguration.WebAPIVersion);
//                    httpClient.DefaultRequestHeaders.Accept.Add(
//                    new MediaTypeWithQualityHeaderValue("application/json"));

//                    var str = "";
//                    if (Parameters != null && Parameters.Count > 0)
//                    {
//                        foreach (var item in Parameters)
//                            str += String.Format("{0}={1}{2}", item.Key, System.Net.WebUtility.UrlEncode(item.Value), (Parameters.Last().Key == item.Key) ? "" : "&");
//                    }

//                    url = !String.IsNullOrEmpty(str) ? String.Format("api/{0}/{1}?{2}", controller, action, str) : string.Format(((!String.IsNullOrEmpty(action)) ? "api/{0}/{1}" : "api/{0}{1}"), controller, action);
//                    HttpContent content;


//                    //if (containsBytesToUpload)
//                    //{
//                    //    var bytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, settings));
//                    //    content = new ByteArrayContent(bytes, 0, bytes.Count());
//                    //}

//                    //else
//                    //{
//                    var jsonString = JsonConvert.SerializeObject(data, settings);
//                    content = new StringContent(jsonString, Encoding.UTF8, "application/json");
//                    //}


//                    HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
//                    ResponseHeader = response.Headers;

//                    return RenderRequestResult(response, url, () =>
//                    {
//                        LoginHelper.Logout(false);
//                        return Result = default(TReturnType);
//                    }, action);
//                }
//            }
//            catch (CustomException ex)
//            {

//                if (!MethodsToSkipLogs.Contains(action))
//                    DeviceUtilities.LogError(ex);
//            }
//            catch (AggregateException ex)
//            {

//                if (ex.InnerException != null && ex.InnerException.GetType() == typeof(TaskCanceledException) && !MethodsToSkipLogs.Contains(action))
//                {
//                    ResponseStatus = BaseClientResponseStatus.TIME_OUT;
//                    DeviceUtilities.LogError(new Exception("POST timeout expired", new Exception("Request URI: " + url)));
//                }
//            }
//            catch (Exception ex)
//            {

//                if (!MethodsToSkipLogs.Contains(action))
//                    DeviceUtilities.LogError(new Exception("POST Error -> FAILED", new Exception("Request URI: " + url, ex)));
//            }
//            return Result = default(TReturnType);
//        }

//        public TReturnType Post(string controller, string action, bool containsBytesToUpload = false)
//        {
//            return Post<object>(controller, action);
//        }

//        public TReturnType PostBinaryAsync<tData>(string controller, string action, tData data) where tData : IMultiContentRequestFile
//        {
//            String url = controller + (String.IsNullOrEmpty(action) ? "" : "/" + action);
//            try
//            {
//                if (!ConnectivityObserver.Current.IsInternetAvailable || !ConnectivityObserver.Current.IsServiceRunning)
//                    return Result = default(TReturnType);

//                using (var httpClient = new HttpClient())
//                {
//                    httpClient.BaseAddress = new Uri(_endpoint);
//                    httpClient.Timeout = new TimeSpan(1, 0, 0);

//                    AttachAthorization(httpClient);

//                    httpClient.DefaultRequestHeaders.Add("apiv", AppConfiguration.WebAPIVersion);

//                    var str = "";
//                    if (Parameters != null && Parameters.Count > 0)
//                    {
//                        foreach (var item in Parameters)
//                            str += String.Format("{0}={1}{2}", item.Key, System.Net.WebUtility.UrlEncode(item.Value), (Parameters.Last().Key == item.Key) ? "" : "&");
//                    }

//                    url = !String.IsNullOrEmpty(str) ? String.Format("api/{0}/{1}?{2}", controller, action, str) : string.Format(((!String.IsNullOrEmpty(action)) ? "api/{0}/{1}" : "api/{0}{1}"), controller, action);

//                    var jsonString = JsonConvert.SerializeObject(data, settings);
//                    HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

//                    var multiPartContent = new MultipartFormDataContent();
//                    multiPartContent.Add(content, "data");

//                    if (data != null && data.files != null)
//                    {
//                        foreach (var item in data.files.ToList())
//                        {
//                            multiPartContent.Add(new ByteArrayContent(item.FileBytes, 0, item.FileBytes.Length), item.Key, String.Format("{0}.{1}", item.ImageName, item.Extension));
//                        }
//                    }

//                    HttpResponseMessage response = httpClient.PostAsync(url, multiPartContent).Result;
//                    ResponseHeader = response.Headers;

//                    return RenderRequestResult(response, url, null);
//                }
//            }
//            catch (CustomException ex)
//            {
//                if (!MethodsToSkipLogs.Contains(action))
//                    DeviceUtilities.LogError(ex);
//            }
//            catch (AggregateException ex)
//            {
//                if (ex.InnerException != null && ex.InnerException.GetType() == typeof(TaskCanceledException) && !MethodsToSkipLogs.Contains(action))
//                    DeviceUtilities.LogError(new Exception("POST timeout expired", new Exception("Request URI: " + url)));
//            }
//            catch (Exception ex)
//            {
//                if (!MethodsToSkipLogs.Contains(action))
//                    DeviceUtilities.LogError(new Exception("POST Error -> FAILED", new Exception("Request URI: " + url, ex)));
//            }
//            return Result = default(TReturnType);
//        }

//        public TReturnType RenderRequestResult(HttpResponseMessage Response, String url, Func<TReturnType> OnUnauthorized, string action = "")
//        {
//            Succeed = Response.IsSuccessStatusCode;

//            if (Response.IsSuccessStatusCode)
//            {
//                try
//                {
//                    var result = JsonConvert.DeserializeObject<TReturnType>(Response.Content.ReadAsStringAsync().Result, settings);

//                    return Result = result;
//                }
//                catch (Exception ex)
//                {
//                    DeviceUtilities.LogError(new Exception("Error -> API Request -> SUCCESS -> Deserialization", new Exception("Request URI: " + url, ex)));
//                }
//            }
//            else if (Response.StatusCode == HttpStatusCode.Unauthorized)
//            {
//                if (OnUnauthorized != null)
//                    return Result = OnUnauthorized.Invoke();
//            }
//            else if (Response.StatusCode == HttpStatusCode.BadRequest)
//            {
//                BadRequestResult = Response.Content.ReadAsStringAsync().Result;
//                if (String.IsNullOrEmpty(BadRequestResult))
//                    BadRequestResult = Response.ReasonPhrase;
//            }
//            else
//            {
//                throw new CustomException("Error -> API Request -> FAILED -> Status Code: " + Response.StatusCode.ToString(), new Exception("Request URI: " + url));
//            }
//            return Result = default(TReturnType);
//        }

//        public TReturnType Delete(string controller, string action)
//        {
//            if (!ConnectivityObserver.Current.IsInternetAvailable || !ConnectivityObserver.Current.IsServiceRunning)
//                return Result = default(TReturnType);

//            using (var httpClient = new HttpClient())
//            {
//                httpClient.BaseAddress = new Uri(_endpoint);

//                AttachAthorization(httpClient);

//                httpClient.DefaultRequestHeaders.Add("apiv", AppConfiguration.WebAPIVersion);
//                httpClient.DefaultRequestHeaders.Accept.Add(
//                new MediaTypeWithQualityHeaderValue("application/json"));

//                var str = "";
//                if (Parameters != null && Parameters.Count > 0)
//                {
//                    foreach (var item in Parameters)
//                        str += String.Format("{0}={1}{2}", item.Key, System.Net.WebUtility.UrlEncode(item.Value), (Parameters.Last().Key == item.Key) ? "" : "&");
//                }

//                var url = !String.IsNullOrEmpty(str) ? String.Format("api/{0}/{1}?{2}", controller, action, str) : string.Format("api/{0}/{1}", controller, action);


//                HttpResponseMessage response = httpClient.DeleteAsync(url).Result;
//                ResponseHeader = response.Headers;


//                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
//                {
//                    return Result = default(TReturnType);
//                }

//                return Result = JsonConvert.DeserializeObject<TReturnType>(response.Content.ReadAsStringAsync().Result);
//            }
//            return Result = default(TReturnType);
//        }

//        public TReturnType Ping(TimeSpan ts, string controller = "associate", string action = "ping", bool isAPI = true)
//        {
//            String url = controller + "/" + action;
//            try
//            {
//                if (!ConnectivityObserver.Current.IsInternetAvailable)
//                    return Result = default(TReturnType);

//                using (var httpClient = new HttpClient())
//                {
//                    httpClient.BaseAddress = new Uri(_endpoint);
//                    AttachAthorization(httpClient);

//                    httpClient.DefaultRequestHeaders.Add("apiv", AppConfiguration.WebAPIVersion);
//                    httpClient.Timeout = ts;
//                    httpClient.DefaultRequestHeaders.Accept.Add(
//                    new MediaTypeWithQualityHeaderValue("application/json"));

//                    url = string.Format("{2}{0}/{1}", controller, action, (isAPI) ? "api/" : "");

//                    HttpResponseMessage response = httpClient.GetAsync(url).Result;
//                    ResponseHeader = response.Headers;

//                    return RenderRequestResult(response, url, () =>
//                    {
//                        LoginHelper.Logout(false);
//                        return Result = default(TReturnType);
//                    });
//                }
//            }
//            catch (CustomException ex)
//            {
//                DeviceUtilities.LogError(ex);
//            }
//            catch (AggregateException ex)
//            {
//                if (ex.InnerException != null && ex.InnerException.GetType() == typeof(TaskCanceledException))
//                {
//                    ResponseStatus = BaseClientResponseStatus.TIME_OUT;
//                    //DeviceUtilities.LogError(new Exception("POST timeout expired", new Exception("Request URI: " + url)));
//                }
//            }
//            catch (Exception ex)
//            {
//                DeviceUtilities.LogError(new Exception("GET Error -> FAILED", new Exception("Request URI: " + url, ex)));
//            }
//            return Result = default(TReturnType);
//        }

//        public async Task<TReturnType> TokenPost()
//        {
//            String url = AppConfiguration.Auth.TokenEndpointPath;

//            try
//            {
//                using (var httpClient = new HttpClient())
//                {
//                    httpClient.BaseAddress = new Uri(_endpoint);
//                    httpClient.DefaultRequestHeaders.Accept.Add(
//                    new MediaTypeWithQualityHeaderValue("application/json"));
//                    HttpContent content = new FormUrlEncodedContent(Parameters);
//                    HttpResponseMessage response = await httpClient.PostAsync(url, content);
//                    ResponseHeader = response.Headers;

//                    return RenderRequestResult(response, url, () =>
//                    {
//                        response = httpClient.PostAsync(url, content).Result;
//                        return RenderRequestResult(response, url, null);
//                    });
//                }
//            }
//            catch (Exception ex)
//            {
//                await DeviceUtilities.LogError(new Exception("TokenPost Error -> FAILED", new Exception("Request URI: " + url, ex)));
//            }

//            return Result = default(TReturnType);
//        }

//        public string[] MethodsToSkipLogs
//        {
//            get
//            {
//                return new string[] { "LogError", "LogErrorCollection" };
//            }
//        }

//        //public bool
//    }
//}
