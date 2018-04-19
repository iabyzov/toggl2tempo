using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tempo.DataObjects
{
    [JsonObject]
    public class Worklog
    {
        public Worklog()
        {
            WorklogAttributes = new List<WorklogAttribute>();
        }

        [JsonProperty(PropertyName = "id")]
        public long? Id { get; set; }

        [JsonProperty(PropertyName = "dateStarted")]
        public string DateStarted { get; set; }

        [JsonProperty(PropertyName = "timeSpentSeconds")]
        public int TimeSpentSeconds { get; set; }

        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }

        [JsonProperty(PropertyName = "issue")]
        public Issue Issue { get; set; }

        [JsonProperty(PropertyName = "author")]
        public Author Author { get; set; }

        [JsonProperty(PropertyName = "worklogAttributes")]
        public List<WorklogAttribute> WorklogAttributes { get; set; }
    }
}