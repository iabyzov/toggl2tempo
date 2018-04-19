using System;

namespace BLL.Domain
{
    public abstract class Worklog : IWorklog
    {
        public Worklog()
        {
            State = WorklogState.Unprocessed;
        }

        public long? Id { get; set; }

        public string Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public WorklogState State { get; set; }

        public int Duration
        {
            get
            {
                return (int)(EndTime - StartTime).TotalSeconds;
            }
        }

        public abstract bool IsCorrect { get; }
    }
}
