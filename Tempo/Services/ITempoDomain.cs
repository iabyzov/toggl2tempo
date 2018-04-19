using Dapplo.Jira;
using Dapplo.Jira.Domains;

namespace Tempo.Services
{
    public interface ITempoDomain : IJiraDomain, IWorklogService
    {
    }
}