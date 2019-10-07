using System;
using System.Collections.Generic;
using Dapplo.Jira;
using Tempo.DataObjects;
using Tempo.Services;

namespace TempoTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var tempoClient = new WorklogService(new Uri(""));
            tempoClient.SetBasicAuthentication("", "");
            
            var searchResult = tempoClient.Issue.GetAsync("21127").Result;
            var myselfAsync = tempoClient.User.GetMyselfAsync().Result;
            var worklog = new Worklog()
            {
                AuthorAccountId = myselfAsync.Name,
                IssueKey = "18652",
                StartDate = DateTime.Now.ToString("yyyy-MM-dd"),
                StartTime = DateTime.Now.ToString("HH:mm:ss"),
                Description = "Tested toggl2tempo",
                TimeSpentSeconds = 2600,
                WorklogAttributes = new List<WorklogAttribute>() { new WorklogAttribute() { Key = "_Activity_", Value = "Other"} }
            };

            tempoClient.Tempo.CreateAsync(worklog).Wait();
        }
    }
}