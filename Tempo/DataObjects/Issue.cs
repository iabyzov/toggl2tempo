using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Tempo.DataObjects
{
    [JsonObject]
    public class Issue
    {
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }
    }
}
