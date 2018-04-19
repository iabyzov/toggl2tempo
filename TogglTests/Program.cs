using System;

using Common;
using Toggl.QueryObjects;
using Toggl.Services;

namespace TogglTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var apiKey = "";
            var usrSrv = new UserService(apiKey);
            var c = usrSrv.GetCurrent();
            //usrSrv.

            var workspaceId = c.DefaultWorkspaceId ?? 0;
            var standardParams = new DetailedReportParams()
            {
                UserAgent = "",
                WorkspaceId = workspaceId,
                Since = DateTime.Now.AddDays(-10).ToIsoDateTimeStr()
            };

            var reportService = new ReportService(apiKey);
            var res = reportService.Detailed(standardParams);
        }
    }
}