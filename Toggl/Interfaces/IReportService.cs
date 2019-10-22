using Toggl.DataObjects;
using Toggl.QueryObjects;

namespace Toggl.Interfaces
{
    public interface IReportService
    {
        IApiService ToggleSrv { get; set; }

        DetailedReport Detailed(DetailedReportParams requestParameters);
    }
}
