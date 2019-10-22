﻿using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Toggl
{
    public class ApiRequest
    {
        public string Url { get; set; }
        public List<KeyValuePair<string, string>> Args { get; set; }
        public CookieContainer Container { get; set; }
        public HttpMethod Method { get; set; }
        public string Data { get; set; }
        public string ContentType { get; set; }
        public NetworkCredential Credential { get; set; }

        public ApiRequest()
        {
            Method = HttpMethod.Get;
            ContentType = "application/json";
        }

    }
}
