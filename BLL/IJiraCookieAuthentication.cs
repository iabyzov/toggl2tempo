using System;
using System.Security.Claims;
using Tempo.Services;

namespace BLL
{
    public interface IJiraCookieAuthentication
    {
        IWorklogService GetWorkLogServiceFromCookie(Uri jiraHostUri);

        Claim GetClaimWithJiraSession(IWorklogService worklogService);
    }
}