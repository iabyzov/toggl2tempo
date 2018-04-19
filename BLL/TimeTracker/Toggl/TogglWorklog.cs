using System;
using System.Collections.Generic;

using BLL.Domain;

namespace BLL.TimeTracker.Toggl
{
    public class TogglWorklog : Worklog
    {
        public TogglWorklog()
        {
            Tags = new List<string>();
        }

        // TODO: Implement correct check
        public override bool IsCorrect => true;

        public string Project { get; set; }

        public DateTime UpdatedOn { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
