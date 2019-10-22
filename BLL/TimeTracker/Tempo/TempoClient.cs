using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Common.Extensions;
using Microsoft.AspNetCore.Http;
using Tempo.Services;

using InternalTempoWorklog = Tempo.DataObjects.Worklog;
using InternalWorklogAttribute = Tempo.DataObjects.WorklogAttribute;

namespace BLL.TimeTracker.Tempo
{
    public class TempoClient : ITempoClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IApplicationUserService _userService;

        private IConfigurationSection _jiraConfig;
        private IWorklogService _worklogService;

        private const string ActivityAttributeKey = "_Activity_";

        public TempoClient(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IApplicationUserService userService )
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        private void Init()
        {
            if (_jiraConfig == null)
            {
                _jiraConfig = _configuration.GetSection("Jira");
            }

            if (_worklogService == null)
            {
                var jiraHostUri = new Uri(_jiraConfig["Host"]);

                _worklogService = new WorklogService(jiraHostUri);
                _worklogService.SetBearer(_userService.GetTempoTokenForCurrentUser());
            }
        }

        public void AddWorklog(TempoWorklog workLog)
        {
            Init();

            var internalWorklog = MapToInternalTempoWorklog(workLog);
            _worklogService.Tempo.CreateAsync(internalWorklog).Wait();
        }

        public IEnumerable<TempoWorklog> GetTimeSheet(DateTime start, DateTime end)
        {
            Init();

            var worklogs = _worklogService.Tempo.GetWorklogsAsync(start, end).Result;

            return worklogs.Select(MapToTempoWorklog)
                .ToList();
        }

        private TempoWorklog MapToTempoWorklog(InternalTempoWorklog internalWorkLog)
        {
            var worklog = new TempoWorklog
            {
                Id = internalWorkLog.Id,
                Description = internalWorkLog.Description,
                StartTime = DateTime.ParseExact($"{internalWorkLog.StartDate} {internalWorkLog.StartTime}",
                    "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                TicketKey = internalWorkLog.IssueKey
            };

            worklog.EndTime = worklog.StartTime.AddSeconds(internalWorkLog.TimeSpentSeconds);
            worklog.Activity = internalWorkLog.WorklogAttributes.FirstOrDefault(x => x.Key == ActivityAttributeKey)?.Value;

            return worklog;
        }

        private InternalTempoWorklog MapToInternalTempoWorklog(TempoWorklog workLog)
        {
            var userIdentity = _httpContextAccessor.HttpContext.User.Identity;
            var nameId = userIdentity.GetNameId();
            var selfAccountId = userIdentity.GetSelfAccountId();

            if (string.IsNullOrEmpty(nameId) || string.IsNullOrEmpty(selfAccountId))
            {
                throw new InvalidOperationException("Can't find necessary claims");
            }

            var internalWorklog = new InternalTempoWorklog()
            {
                StartDate = workLog.StartTime.ToIsoDateStr(),
                StartTime = workLog.StartTime.ToIsoTimeStr(),
                TimeSpentSeconds = workLog.Duration,
                Description = workLog.Description,
                IssueKey = workLog.TicketKey,
                AuthorAccountId = selfAccountId
            };

            internalWorklog.WorklogAttributes.Add(
                new InternalWorklogAttribute
                {
                    Key = ActivityAttributeKey,
                    Value = Uri.EscapeDataString(workLog.Activity)
                });

            internalWorklog.Id = workLog.Id;

            return internalWorklog;
        }


    }
}
