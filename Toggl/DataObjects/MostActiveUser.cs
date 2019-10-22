﻿using Newtonsoft.Json;

namespace Toggl.DataObjects
{
    public class MostActiveUser : BaseDataObject
    {
        [JsonProperty(PropertyName = "user_id")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public int Duration { get; set; }
    }
}
