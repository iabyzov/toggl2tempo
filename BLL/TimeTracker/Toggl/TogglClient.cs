using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;

using Common;

using Toggl;
using Toggl.DataObjects;
using Toggl.QueryObjects;
using Toggl.Services;

namespace BLL.TimeTracker.Toggl
{
    public class TogglClient : ITogglClient
    {
        private readonly IApplicationUserService _applicationUserService;

        public TogglClient(IApplicationUserService applicationUserService)
        {
            _applicationUserService = applicationUserService;
        }

        private User GetTogglUser()
        {
            var token = _applicationUserService.GetTogglTokenForCurrentUser();
           
                var userService = new UserService(token);
                return userService.GetCurrent();
        }

        public void AddWorklog(TogglWorklog workLog)
        {
            throw new NotImplementedException();
        }

        public void DeleteWorklog(long workLogId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TogglWorklog> GetTimeSheet(DateTime start, DateTime end)
        {
            var token = _applicationUserService.GetTogglTokenForCurrentUser();
            var togglUser = GetTogglUser();

            var reportService = new ReportService(token);

            int pageNumber = 1;

            List<TogglWorklog> _worklogs = new List<TogglWorklog>();

            while (true)
            {
                var reportParams = new DetailedReportParams
                {
                    Since = start.Date.ToIsoDateTimeStrWithoutTimeZone(),
                    Until = end.Date.ToIsoDateTimeStrWithoutTimeZone(),
                    Page = pageNumber,

                    UserAgent = togglUser.Email,
                    WorkspaceId = togglUser.DefaultWorkspaceId ?? 0, // TODO: FIX
                };

                var detailedReport = reportService.Detailed(reportParams);

                var worklogsonPage = detailedReport.Data.Select(MapTogglWorklog);
                _worklogs.AddRange(worklogsonPage);

                var totalCount = detailedReport.TotalCount;
                var perPage = detailedReport.PerPage;

                if ((perPage * pageNumber) >= totalCount)
                    break;

                pageNumber++;
            }

            return _worklogs;
        }

        private TogglWorklog MapTogglWorklog(ReportTimeEntry entry)
        {
            // We should store and compare updateOn in UTC.
            if (!string.IsNullOrEmpty(entry.Updated) && DateTime.TryParse(entry.Updated, out DateTime updatedOn))
                updatedOn = updatedOn.ToUniversalTime();
            else
                updatedOn = DateTime.Now.ToUniversalTime();

            return new TogglWorklog
            {
                Id = entry.Id,
                Description = entry.Description,
                StartTime = DateTime.Parse(entry.Start),
                EndTime = DateTime.Parse(entry.Stop),
                Project = entry.ProjectName,
                UpdatedOn = updatedOn,
                Tags = entry.TagNames
            };
        }
    }
}