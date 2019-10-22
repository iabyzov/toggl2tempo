﻿using Newtonsoft.Json;
using System;

namespace Toggl.DataObjects
{
    public class Activity : BaseDataObject
    {
        [JsonProperty(PropertyName = "user_id")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "project_id")]
        public int ProjectId { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public int Duration { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "stop")]
        public DateTime Stop { get; set; }
    }
}
