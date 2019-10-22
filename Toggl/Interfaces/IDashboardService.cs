using Toggl.DataObjects;

namespace Toggl.Interfaces
{
    public interface IDashboardService
    {
        IApiService ToggleSrv { get; set; }

        Dashboard Get(int workspaceID);
    }
}
