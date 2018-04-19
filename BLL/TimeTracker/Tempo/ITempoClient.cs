using System;
using System.Collections.Generic;
using System.Text;

using BLL.Domain;

namespace BLL.TimeTracker.Tempo
{
    public interface ITempoClient : ITimeTracker<TempoWorklog>
    {
    }
}
