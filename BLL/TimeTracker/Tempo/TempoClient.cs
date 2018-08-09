using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

using Common;
using Dapplo.Jira;
using Dapplo.Jira.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Tempo.Services;

using InternalTempoWorklog = Tempo.DataObjects.Worklog;
using InternalTempoIssue = Tempo.DataObjects.Issue;
using InternalTempoAuthor = Tempo.DataObjects.Author;
using InternalWorklogAttribute = Tempo.DataObjects.WorklogAttribute;

namespace BLL.TimeTracker.Tempo
{
    public class TempoClient : ITempoClient
    {
        private IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IApplicationUserService _userService;

        private IConfigurationSection _jiraConfig = null;
        private IWorklogService _worklogService = null;

        private User _jiraUser = null;

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

            InternalTempoWorklog result;
            if (internalWorklog.Id == null)
            {
                result = _worklogService.Tempo.CreateAsync(internalWorklog).Result;
                workLog.Id = result.Id;
            }
            else
            {
                result = _worklogService.Tempo.UpdateAsync(internalWorklog).Result;
            }
        }

        public void DeleteWorklog(long workLogId)
        {
            Init();

            _worklogService.Tempo.DeleteAsync(workLogId);
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
                Description = internalWorkLog.Comment,
                StartTime = DateTime.Parse(internalWorkLog.DateStarted),
                TicketKey = internalWorkLog.Issue.Key,
            };

            worklog.EndTime = worklog.StartTime.AddSeconds(internalWorkLog.TimeSpentSeconds);
            worklog.Activity = internalWorkLog.WorklogAttributes.FirstOrDefault(x => x.Key == ActivityAttributeKey)?.Value;

            return worklog;
        }

        private InternalTempoWorklog MapToInternalTempoWorklog(TempoWorklog workLog)
        {
            var userIdentity = _httpContextAccessor.HttpContext.User.Identity;
            var nameId = userIdentity.GetNameId();
            var self = userIdentity.GetSelf();

            if (string.IsNullOrEmpty(nameId) || string.IsNullOrEmpty(self))
            {
                throw new InvalidOperationException("Can't find neccessary claims");
            }

            var internalWorklog = new InternalTempoWorklog()
            {
                DateStarted = workLog.StartTime.ToIsoDateTimeStrWithoutTimeZone(),
                TimeSpentSeconds = workLog.Duration,
                Comment = workLog.Description,
                Issue = new InternalTempoIssue
                {
                    Key = workLog.TicketKey,
                },
                Author = new InternalTempoAuthor
                {
                    Name = nameId,
                    Self = self,
                }
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
