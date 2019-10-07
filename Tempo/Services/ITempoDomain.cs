using Dapplo.HttpExtensions;
using Dapplo.Jira.Domains;

namespace Tempo.Services
{
    public interface ITempoDomain : IJiraDomain, IWorklogService
    {
        IHttpBehaviour TempoBehaviour { get; }
    }
}