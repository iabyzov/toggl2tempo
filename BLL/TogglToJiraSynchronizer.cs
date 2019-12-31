using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BLL.TimeTracker.Jira;
using BLL.TimeTracker.Tempo;
using BLL.TimeTracker.Toggl;

namespace BLL
{
    public interface ITogglToJiraSynchronizer
    {
        SyncResult Sync(DateTime startTime, DateTime endTime);
    }

    public class TogglToJiraSynchronizer : ITogglToJiraSynchronizer
    {
        private readonly ITogglClient _togglClient;
        private readonly IJiraTracker _jiraTracker;

        private static readonly Regex JiraKeyRegex = new Regex(@"^\w+-\d+");

        private static readonly Dictionary<string, string> ActivitiesMap = new Dictionary<string, string> {
            { "Analysis", "Design/Analysis" },
            { "BugFixing", "Bugfixing" },
            { "CodeReview", "Code Review" },
            { "Development", "Development" },
            { "Interview", "Other" },
            { "Meetings", "Development" },
            { "SystemAdministration", "Environment Setup" },
            { "Team Activities", "Other" },
            { "Estimation", "Estimation" },
            { "Bug fixing", "Bugfixing"},
            { "Code Review Fixes", "Code Review Fixes"},
            { "Test Automation", "AutomationPerformanceTesting" },
            { "Testing", "Testing" },
            { "PM", "PM" }
        };

        public TogglToJiraSynchronizer(
            ITogglClient togglClient,
            IJiraTracker jiraTracker)
        {
            _togglClient = togglClient;
            _jiraTracker = jiraTracker;
        }

        public SyncResult Sync(DateTime startTime, DateTime endTime)
        {
            var togglTimesheet = _togglClient.GetTimeSheet(startTime, endTime);

            var worklogs = new Dictionary<TogglWorklog, JiraWorklog>();

            // Convert and check
            foreach (var togglWorklog in togglTimesheet)
            {
                var tempoWorklog = Convert(togglWorklog);
                if (!tempoWorklog.IsCorrect)
                    continue;

                worklogs.Add(togglWorklog, tempoWorklog);
            }

            var tempoAmount = 0;
            // Sync
            foreach (var wl in worklogs)
            {
                var tempoWorklog = wl.Value;

                // Save worklog to Tempo
                _jiraTracker.AddWorklog(tempoWorklog);
                tempoAmount++;
            }

            var result = new SyncResult()
            {
                TogglAmount = worklogs.Count,
                TempoSentAmount = tempoAmount
            };

            return result;
        }

        private JiraWorklog Convert(TogglWorklog sourceWorklog)
        {
            var wl = new JiraWorklog
            {
                StartTime = sourceWorklog.StartTime,
                EndTime = sourceWorklog.EndTime,
                Description = sourceWorklog.Description,
            };

            CalculateTicketKey(sourceWorklog, wl);
            CalculateActivity(sourceWorklog, wl);

            return wl;
        }

        private void CalculateTicketKey(TogglWorklog toggleWorklog, JiraWorklog jiraWorklog)
        {
            if (!string.IsNullOrEmpty(toggleWorklog.Project) && JiraKeyIsValid(toggleWorklog.Project))
            {
                var match = JiraKeyRegex.Match(toggleWorklog.Project);
                jiraWorklog.TicketKey = match.Groups[0].Value;
            }
            else if (!string.IsNullOrEmpty(toggleWorklog.Description) && JiraKeyIsValid(toggleWorklog.Description))
            {
                var match = JiraKeyRegex.Match(toggleWorklog.Description);
                var ticket = match.Groups[0].Value;
                jiraWorklog.TicketKey = ticket;

                var length = ticket.Length;
                jiraWorklog.Description = toggleWorklog.Description.Substring(length).Trim();
            }
            else
            {
                throw new ApplicationException(
                    "Jira issue key not found! " +
                    $"{nameof(toggleWorklog.Project)}: {toggleWorklog.Project}; " +
                    $"{nameof(toggleWorklog.Description)}: {toggleWorklog.Description}");
            }
        }

        private void CalculateActivity(TogglWorklog sourceWorklog, JiraWorklog worklog)
        {
            var activity = "Other";

            if (!string.IsNullOrEmpty(sourceWorklog.Project) && ActivitiesMap.Keys.Contains(sourceWorklog.Project))
            {
                activity = ActivitiesMap[sourceWorklog.Project];
            }

            //worklog.Activity = activity;
        }

        private bool JiraKeyIsValid(string key)
        {
            return JiraKeyRegex.IsMatch(key);
        }

    }
}