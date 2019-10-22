using System;

namespace BLL.Domain
{
    public interface IWorklog
    {
        long? Id { get; set; }

        string Description { get; set; }

        DateTime StartTime { get; set; }

        DateTime EndTime { get; set; }

        int Duration { get; }

        bool IsCorrect { get; }
    }
}
