using System;
using System.Collections.Generic;
using Dapplo.Jira;
using Dapplo.Jira.Query;
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
                Author = new Author()
                {
                    Name = myselfAsync.Name,
                    Self = myselfAsync.Self.ToString()
                },
                Issue = new Issue()
                {
                    Key = "18652"
                },
                DateStarted = DateTime.Now.ToString("o"),
                Comment = "Tested toggl2tempo",
                TimeSpentSeconds = 2600,
                WorklogAttributes = new List<WorklogAttribute>() { new WorklogAttribute() { Key = "_Activity_", Value = "Other"} }
            };

            var result = tempoClient.Tempo.CreateAsync(worklog).Result;
        }
    }
}