using Newtonsoft.Json;

namespace Tempo.DataObjects
{
    [JsonObject]
    public class Author
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "self")]
        public string Self { get; set; }
    }
}
