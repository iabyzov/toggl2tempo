using System;
using System.Collections.Generic;
using System.Text;
using Dapplo.Jira;
using Dapplo.Jira.Entities;
using Microsoft.Extensions.Configuration;
using Tempo.Services;

namespace BLL.TimeTracker.Jira
{
    public class JiraTracker : IJiraTracker
    {
        private readonly IConfiguration _configuration;
        private readonly IApplicationUserService _userService;

        private IConfigurationSection _jiraConfig;
        private IJiraClient _jiraClient;

        public JiraTracker(IConfiguration configuration, IApplicationUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        private void Init()
        {
            if (_jiraConfig == null)
            {
                _jiraConfig = _configuration.GetSection("Jira");
            }

            if (_jiraClient == null)
            {
                var jiraHostUri = new Uri(_jiraConfig["Host"]);

                _jiraClient = JiraClient.Create(jiraHostUri);
                //_jiraClient.SetBearer(_userService.GetTempoTokenForCurrentUser());
            }
        }
        public IEnumerable<JiraWorklog> GetTimeSheet(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public void AddWorklog(JiraWorklog workLog)
        {
            Init();

            var user = _userService.Get("abyzily");
            _jiraClient.SetBasicAuthentication(user.Email, user.Password);
            var duration = workLog.EndTime - workLog.StartTime;
            _jiraClient.Work.CreateAsync(workLog.TicketKey, new Worklog(duration)).Wait();
        }
    }
}
