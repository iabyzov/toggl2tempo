using System;
using System.Collections.Generic;
using System.Text;

using BLL.Domain;

namespace BLL.TimeTracker.Toggl
{
    public interface ITogglClient: ITimeTracker<TogglWorklog>
    {
    }
}
