using System;
using Dapplo.Jira;

namespace Tempo.Services
{
    public interface IWorklogService : IJiraClient
    {
        Uri JiraTempoUri { get; }

        ITempoDomain Tempo { get; }
    }
}