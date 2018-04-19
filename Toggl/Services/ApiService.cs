using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Toggl.DataObjects;
using Toggl.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Toggl.Services
{
    public class ApiService : IApiService
    {
        private string ApiToken { get; set; }

        public Session Session { get; set; }

        public ApiService(string apiToken)
        {
            ApiToken = apiToken;
        }

        public void Initialize()
        {
            if (Session != null && !string.IsNullOrEmpty(Session.ApiToken))
            {
                return;
            }

            GetSession();
        }

        public Session GetSession()
        {

            var args = new List<KeyValuePair<string, string>>();

            Session = Get(ApiRoutes.Session.Me, args).GetData<Session>();

            ApiToken = Session.ApiToken;

            return Session;
        }

        public ApiResponse Get(string url)
        {
            return Get(new ApiRequest
            {
                Url = url
            });
        }

        public ApiResponse Get(string url, List<KeyValuePair<string, string>> args)
        {
            return Get(new ApiRequest
            {
                Url = url,
                Args = args
            });
        }

        public TResponse Get<TResponse>(string url)
        {
            return Get<TResponse>(new ApiRequest()
            {
                Url = url
            });
        }

        public TResponse Get<TResponse>(string url, List<KeyValuePair<string, string>> args)
        {
            return Get<TResponse>(new ApiRequest()
                                  {
                                      Url = url, Args = args
                                  });
        }

        public ApiResponse Delete(string url)
        {
            return Get(new ApiRequest
            {
                Url = url,
                Method = HttpMethod.Delete
            });
        }

        public ApiResponse Delete(string url, List<KeyValuePair<string, string>> args)
        {
            return Get(new ApiRequest
            {
                Url = url,
                Method = HttpMethod.Delete,
                Args = args
            });
        }
        public ApiResponse Post(string url, string data)
        {
            return Get(
                new ApiRequest
                {
                    Url = url,
                    Method = HttpMethod.Post,
                    ContentType = "application/json",
                    Data = data
                });
        }

        public ApiResponse Post(string url, List<KeyValuePair<string, string>> args, string data = "")
        {
            return Get(
                new ApiRequest
                {
                    Url = url,
                    Args = args,
                    Method = HttpMethod.Post,
                    ContentType = "application/json",
                    Data = data
                });
        }

        public ApiResponse Put(string url, string data)
        {
            return Get(
                new ApiRequest
                {
                    Url = url,
                    Method = HttpMethod.Put,
                    ContentType = "application/json",
                    Data = data
                });
        }

        public ApiResponse Put(string url, List<KeyValuePair<string, string>> args, string data = "")
        {
            return Get(
                new ApiRequest
                {
                    Url = url,
                    Args = args,
                    Method = HttpMethod.Put,
                    ContentType = "application/json",
                    Data = data
                });
        }

        private TResponse Get<TResponse>(ApiRequest apiRequest) 
        {
            string value = "";

            if (apiRequest.Args != null && apiRequest.Args.Count > 0)
            {
                apiRequest.Args.ForEach(e => value += e.Key + "=" + System.Uri.EscapeDataString(e.Value) + "&");
                value = value.Trim('&');
            }

            if (apiRequest.Method == HttpMethod.Get && !string.IsNullOrEmpty(value))
            {
                apiRequest.Url += "?" + value;
            }

            var client = new HttpClient();

            //var authRequest = (HttpWebRequest)HttpWebRequest.Create(apiRequest.Url);
            var authRequest = new HttpRequestMessage();
            authRequest.RequestUri = new Uri(apiRequest.Url, UriKind.RelativeOrAbsolute);
            authRequest.Method = apiRequest.Method;

            //authRequest.ContentType = apiRequest.ContentType;

            //authRequest.Credentials = CredentialCache.DefaultNetworkCredentials;

            //authRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(GetAuthHeader()));
            AddAuthorizationHeader(authRequest);

            var authResponse = client.SendAsync(authRequest).Result;
            string content = authResponse.Content.ReadAsStringAsync().Result;
            //using (var reader = new StreamReader(authResponse.GetResponseStream(), Encoding.UTF8))
            //{
            //    content = reader.ReadToEnd();
            //}

            var rsp = JsonConvert.DeserializeObject<TResponse>(content);              
            return rsp;
        }

        private void AddAuthorizationHeader(HttpRequestMessage authRequest)
        {
            var authorizationHeader = GetAuthHeader();
            authRequest.Headers.Add(authorizationHeader.Item1, authorizationHeader.Item2);
        }

        private HttpRequestMessage ManufactureRequest(ApiRequest apiRequest)
        {
            string value = "";

            if (apiRequest.Args != null && apiRequest.Args.Count > 0)
            {
                apiRequest.Args.ForEach(e => value += e.Key + "=" + System.Uri.EscapeDataString(e.Value) + "&");
                value = value.Trim('&');
            }

            if (apiRequest.Method == HttpMethod.Get && !string.IsNullOrEmpty(value))
            {
                apiRequest.Url += "?" + value;
            }

            //var authRequest = (HttpWebRequest)HttpWebRequest.Create(apiRequest.Url);

            var authRequest = new HttpRequestMessage();
            authRequest.RequestUri = new Uri(apiRequest.Url, UriKind.RelativeOrAbsolute);
            authRequest.Method = apiRequest.Method;
            //authRequest.ContentType = apiRequest.ContentType;
            //authRequest.Credentials = CredentialCache.DefaultNetworkCredentials;

            AddAuthorizationHeader(authRequest);


            if (apiRequest.Method == HttpMethod.Post || apiRequest.Method == HttpMethod.Put)
            {
                var utd8WithoutBom = new UTF8Encoding(false);

                authRequest.Content = new StringContent(apiRequest.Data, utd8WithoutBom, apiRequest.ContentType);
                //value += apiRequest.Data;
                //authRequest.ContentLength = utd8WithoutBom.GetByteCount(value);
                //using (var writer = new StreamWriter(authRequest.GetRequestStream(), utd8WithoutBom))
                //{
                //    writer.Write(value);
                //}
            }

            return authRequest;
        }

        private ApiResponse Get(ApiRequest apiRequest)
        {
            HttpResponseMessage authResponse = null;

            while (true)
            {
                try
                {
                    var authRequest = ManufactureRequest(apiRequest);
                    var client = new HttpClient();
                    //authResponse = (HttpWebResponse)authRequest.GetResponse();
                    authResponse = client.SendAsync(authRequest).Result;
                    Console.WriteLine(((int)authResponse.StatusCode).ToString());
                    break;
                }
                catch (Exception ex)
                {
                    throw;
                    // Pay attention to HTTP 429 responses to work with the Leaky bucket
                    // mentioned at https://github.com/toggl/toggl_api_docs  
                    // retry as necessary

                    //if (ex.Status == WebExceptionStatus.ProtocolError)
                    //{
                    //    var response = ex.Response as HttpWebResponse;
                    //    if (response != null)
                    //    {
                    //        int statusCode = (int)response.StatusCode;
                    //        Console.WriteLine(statusCode.ToString());
                    //        if (statusCode == 429)
                    //        {
                    //            Thread.Sleep(1500); // 1500ms based on cursory testing
                    //        }
                    //        else
                    //        {
                    //            throw;
                    //        }
                    //    }
                    //    else {
                    //        throw;
                    //    }
                    //}
                    //else {
                    //    throw;
                    //}
                }
            }
            string content = authResponse.Content.ReadAsStringAsync().Result;
            //using (var reader = new StreamReader(authResponse.GetResponseStream(), Encoding.UTF8))
            //{
            //    content = reader.ReadToEnd();
            //}

            if ((string.IsNullOrEmpty(content)
                || content.ToLower() == "null")
                && authResponse.StatusCode == HttpStatusCode.OK
                && apiRequest.Method == HttpMethod.Delete)
            {
                var rsp = new ApiResponse();
                rsp.Data = new JObject();
                rsp.related_data_updated_at = DateTime.Now;
                rsp.StatusCode = authResponse.StatusCode;
                rsp.Method = apiRequest.Method;

                return rsp;
            }

            try
            {
	            ApiResponse rsp = content.ToLower() == "null" 
					? new ApiResponse { Data = null } 
					: JsonConvert.DeserializeObject<ApiResponse>(content);
	            
                rsp.StatusCode = authResponse.StatusCode;
                rsp.Method = apiRequest.Method;
                return rsp;
            }
            catch (Exception)
            {
                var token = JToken.Parse(content);
                var rsp = new ApiResponse()
                    {
                        Data = token,
                        related_data_updated_at = DateTime.Now,
                        StatusCode = authResponse.StatusCode,
                        Method = apiRequest.Method
                    };
                return rsp;
            }

        }

        private Tuple<string, string> GetAuthHeader()
        {
            var encodedApiKey = Convert.ToBase64String(Encoding.ASCII.GetBytes(ApiToken + ":api_token"));
            return new Tuple<string, string>("Authorization", "Basic " + encodedApiKey);
        }
    }

}
