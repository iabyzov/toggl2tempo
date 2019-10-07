using System.Threading.Tasks;
using Toggl.DataObjects;
using Toggl.QueryObjects;

namespace Toggl.Interfaces
{
    public interface IReportServiceAsync
    {
        IApiServiceAsync ToggleSrv { get; set; }

        Task<DetailedReport> Detailed(DetailedReportParams requestParameters);
    }
}
