using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Configuration;
using BLL.TimeTracker.Tempo;
using BLL.TimeTracker.Toggl;
using Data;
using Data.Entities;

namespace BLL
{
    public class Toggl2TempoSynchronizer : IToggl2TempoSynchronizer
    {
        private IConfiguration _configuration;

        private ITogglClient _togglClient;
        private ITempoClient _tempoClient;

        private SyncDbContext _context;

        private static Regex JiraKeyRegex = new Regex(@"\w+-\d+");

        private static Dictionary<string, string> ActivitiesMap = new Dictionary<string, string> {
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

        public Toggl2TempoSynchronizer(
            IConfiguration configuration,
            SyncDbContext context,
            ITogglClient togglClient,
            ITempoClient tempoClient)
        {
            _configuration = configuration;
            _context = context;

            _togglClient = togglClient;
            _tempoClient = tempoClient;
        }

        public SyncResult Sync(DateTime startTime, DateTime endTime)
        {
            var togglTimesheet = _togglClient.GetTimeSheet(startTime, endTime);

            var worklogs = new Dictionary<TogglWorklog, TempoWorklog>();

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
                var togglWorklog = wl.Key;
                var tempoWorklog = wl.Value;

                // Check if worklog already synced
                var workLogEntity = _context.Worklogs.FirstOrDefault(x => x.MasterId == togglWorklog.Id);
                if (workLogEntity != null)
                {
                    // Skip worklog if it doesn't change in Toggl
                    if (togglWorklog.UpdatedOn <= workLogEntity.UpdatedOn)
                        continue;

                    tempoWorklog.Id = workLogEntity.SecondaryId;
                }

                // Save worklog to Tempo
                _tempoClient.AddWorklog(tempoWorklog);
                tempoAmount++;

                if (!togglWorklog.Id.HasValue || !tempoWorklog.Id.HasValue)
                    throw new InvalidOperationException("Both worklogs must have identifiers to save.");

                // Insert or update
                if (workLogEntity == null)
                {
                    workLogEntity = new WorklogEntity
                    {
                        MasterId = togglWorklog.Id.Value,
                        SecondaryId = tempoWorklog.Id.Value,
                        UpdatedOn = togglWorklog.UpdatedOn,
                    };

                    _context.Add(workLogEntity);
                }
                else
                {
                    workLogEntity.UpdatedOn = togglWorklog.UpdatedOn;
                    _context.Update(workLogEntity);
                }
            }

            _context.SaveChanges();

            //CleanRemovedWorklogsFromTempo(startTime, endTime);

            var result = new SyncResult()
            {
                TogglAmount = worklogs.Count,
                TempoSentAmount = tempoAmount
            };
            
            return result;
        }

        private void CleanRemovedWorklogsFromTempo(DateTime startTime, DateTime endTime)
        {
            // Search deleted worklogs
            var worklogEntities = _context.Worklogs.ToList();
            var tempoTimesheet = _tempoClient.GetTimeSheet(startTime, endTime);
            var togglTimesheet = _togglClient.GetTimeSheet(startTime, endTime);

            var tempoWorklogsToDelete = worklogEntities
                .Where(x => tempoTimesheet.Any(t => t.Id == x.SecondaryId))
                .Where(x => !togglTimesheet.Any(t => t.Id == x.MasterId))
                .ToList();

            foreach (var worklogEntity in tempoWorklogsToDelete)
            {
                _tempoClient.DeleteWorklog(worklogEntity.SecondaryId);
                _context.Remove(worklogEntity);
            }

            _context.SaveChanges();
        }

        private TempoWorklog Convert(TogglWorklog sourceWorklog)
        {
            var wl = new TempoWorklog
            {
                StartTime = sourceWorklog.StartTime,
                EndTime = sourceWorklog.EndTime,
                Description = sourceWorklog.Description,
            };

            CalculateTicketKey(sourceWorklog, wl);
            CalculateActivity(sourceWorklog, wl);

            return wl;
        }

        private void CalculateTicketKey(TogglWorklog sourceWorklog, TempoWorklog worklog)
        {
            // TODO: What occurs if we add more than one tag to worklog?
            var key_tag = sourceWorklog.Tags.FirstOrDefault(x => x.StartsWith("key_"));
            if (!string.IsNullOrEmpty(key_tag))
            {
                var index = key_tag.LastIndexOf('_') + 1;
                if (index > 0 && index < key_tag.Length)
                {
                    var parsedKey = key_tag.Substring(index);
                    if (JiraKeyIsValid(parsedKey))
                    {
                        worklog.TicketKey = parsedKey;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(sourceWorklog.Description))
            {
                var dot_index = sourceWorklog.Description.IndexOf(".");
                if (dot_index >= 0)
                {
                    var parsedKey = sourceWorklog.Description.Substring(0, dot_index);
                    if (JiraKeyIsValid(parsedKey))
                    {
                        worklog.TicketKey = parsedKey;
                        worklog.Description = sourceWorklog.Description.Substring(dot_index + 1).Trim();
                    }
                }
            }
        }

        private void CalculateActivity(TogglWorklog sourceWorklog, TempoWorklog worklog)
        {
            var activity = "Other";

            if (!string.IsNullOrEmpty(sourceWorklog.Project) && ActivitiesMap.Keys.Contains(sourceWorklog.Project))
            {
                activity = ActivitiesMap[sourceWorklog.Project];
            }

            worklog.Activity = activity;
        }

        private bool JiraKeyIsValid(string key)
        {
            return JiraKeyRegex.IsMatch(key);
        }
    }
}
