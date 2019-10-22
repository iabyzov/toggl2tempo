using Newtonsoft.Json;

namespace Toggl.QueryObjects
{
    public class DetailedReportParams : ReportParams
    {        
        [JsonProperty(PropertyName = "page")]
        public int? Page { get; set; }        
    }
}
