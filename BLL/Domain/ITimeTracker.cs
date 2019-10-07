using System;
using System.Collections.Generic;

namespace BLL.Domain
{
    public interface ITimeTracker<WLType>
        where WLType : IWorklog
    {
        IEnumerable<WLType> GetTimeSheet(DateTime start, DateTime end);

        void AddWorklog(WLType workLog);
    }
}
