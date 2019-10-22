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

        [JsonProperty(PropertyName = "startDate")]
        public string StartDate { get; set; }

        [JsonProperty(PropertyName = "startTime")]
        public string StartTime { get; set; }

        [JsonProperty(PropertyName = "timeSpentSeconds")]
        public int TimeSpentSeconds { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "issueKey")]
        public string IssueKey { get; set; }

        [JsonProperty(PropertyName = "authorAccountId")]
        public string AuthorAccountId { get; set; }

        [JsonProperty(PropertyName = "attributes")]
        public List<WorklogAttribute> WorklogAttributes { get; set; }
    }
}