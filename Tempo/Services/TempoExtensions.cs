using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.Jira.Entities;
using Dapplo.HttpExtensions;

using Worklog = Tempo.DataObjects.Worklog;
using System.Text;
using Common.Extensions;

namespace Tempo.Services
{
    public static class TempoExtensions
    {
        public static async Task<List<Worklog>> GetWorklogsAsync(this ITempoDomain jiraClient,
            DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default(CancellationToken))
        {
            jiraClient.TempoBehaviour.MakeCurrent();
            var res = await jiraClient.JiraTempoUri
                .AppendSegments("worklogs")
                .ExtendQuery("dateFrom", dateFrom.ToIsoDateStr())
                .ExtendQuery("dateTo", dateTo.ToIsoDateStr())
                .GetAsAsync<HttpResponse<List<Worklog>, Error>>(cancellationToken)
                .ContinueWith(HandleResponseTask, cancellationToken)
                .ConfigureAwait(false);

            return res.Response;
        }

        public static async Task CreateAsync(this ITempoDomain jiraClient,
            Worklog issue, CancellationToken cancellationToken = default(CancellationToken))
            
        {
            if (issue == null)
                throw new ArgumentNullException(nameof(issue));

            jiraClient.TempoBehaviour.MakeCurrent();
            await jiraClient.JiraTempoUri
                .AppendSegments("worklogs")
                .PostAsync<HttpResponse<Error>>(issue, cancellationToken)
                .ContinueWith(HandleResponseTask, cancellationToken)
                .ConfigureAwait(false);
        }

        private static HttpResponse<T, Error> HandleResponseTask<T>(Task<HttpResponse<T, Error>> task) where T : class
        {
            if (task.Result.HasError)
            {
                var errorsStringBuilder = new StringBuilder($"ERRORS:{Environment.NewLine}");
                foreach (var error in task.Result.ErrorResponse.Errors)
                {
                    errorsStringBuilder.AppendLine($"KEY: {error.Key}. VALUE: {error.Value}");
                }

                throw new Exception(errorsStringBuilder.ToString());
            }

            return task.Result;
        }

        private static HttpResponse<Error> HandleResponseTask(Task<HttpResponse<Error>> task)
        {
            if (task.Result.Response.Errors != null)
            {
                var errorsStringBuilder = new StringBuilder($"ERRORS:{Environment.NewLine}");
                foreach (var error in task.Result.Response.Errors)
                {
                    errorsStringBuilder.AppendLine($"KEY: {error.Key}. VALUE: {error.Value}");
                }

                throw new Exception(errorsStringBuilder.ToString());
            }

            return task.Result;
        }
    }
}