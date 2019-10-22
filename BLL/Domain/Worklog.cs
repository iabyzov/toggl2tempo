using System;

namespace BLL.Domain
{
    public abstract class Worklog : IWorklog
    {
        public long? Id { get; set; }

        public string Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

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
