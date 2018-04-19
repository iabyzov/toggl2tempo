using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Tempo.DataObjects
{
    [JsonObject]
    public class WorklogAttribute
    {
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}
