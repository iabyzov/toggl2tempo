using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.Jira.Internal;
using Dapplo.Jira.Entities;
using Dapplo.HttpExtensions;

using Worklog = Tempo.DataObjects.Worklog;
using Common;

namespace Tempo.Services
{
    public static class TempoExtensions
    {
        public static async Task<List<Worklog>> GetWorklogsAsync(this ITempoDomain jiraClient,
            DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default(CancellationToken))
        {
            jiraClient.Behaviour.MakeCurrent();
            var res = await jiraClient.JiraTempoUri
                .AppendSegments("worklogs")
                .ExtendQuery("dateFrom", dateFrom.ToIsoDateStr())
                .ExtendQuery("dateTo", dateTo.ToIsoDateStr())
                .GetAsAsync<HttpResponse<List<Worklog>, Error>>(cancellationToken)
                .ConfigureAwait(false);

            //return res.HandleErrors(new HttpStatusCode?(HttpStatusCode.OK));
            return res.Response;
        }

        public static async Task<Worklog> CreateAsync(this ITempoDomain jiraClient,
            Worklog issue, CancellationToken cancellationToken = default(CancellationToken))
            
        {
            if (issue == null)
                throw new ArgumentNullException("worklog");

            jiraClient.Behaviour.MakeCurrent();
            var res = await jiraClient.JiraTempoUri
                .AppendSegments("worklogs")
                .PostAsync<HttpResponse<Worklog, Error>>(issue, cancellationToken)
                .ConfigureAwait(false);

            return res.Response;
        }

        public static async Task<Worklog> UpdateAsync(this ITempoDomain jiraClient,
            Worklog issue, CancellationToken cancellationToken = default(CancellationToken))

        {
            if (issue == null)
                throw new ArgumentNullException("worklog");

            if (issue.Id == null)
                throw new ArgumentNullException("worklog.Id");

            jiraClient.Behaviour.MakeCurrent();
            var res = await jiraClient.JiraTempoUri
                .AppendSegments("worklogs")
                .AppendSegments(issue.Id)
                .PutAsync<HttpResponse<Worklog, Error>>(issue, cancellationToken)
                .ConfigureAwait(false);

            return res.Response;
        }

        public static async void DeleteAsync(this ITempoDomain jiraClient,
            long id, CancellationToken cancellationToken = default(CancellationToken))
        {
            jiraClient.Behaviour.MakeCurrent();

            var res = await jiraClient.JiraTempoUri
                .AppendSegments("worklogs")
                .AppendSegments(id)
                .DeleteAsync<HttpResponse>(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}