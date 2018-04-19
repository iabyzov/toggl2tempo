using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using Common;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Tempo.Services;

namespace BLL
{
    public class JiraCookieAuthentication : IJiraCookieAuthentication
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string JiraAuthCookie = "JiraAuthCookie";

        public JiraCookieAuthentication(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IWorklogService GetWorkLogServiceFromCookie(Uri jiraHostUri)
        {
            var worklogService = new WorklogService(jiraHostUri);
            var jiraClaim = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == JiraAuthCookie);
            var jiraCookies = JsonConvert.DeserializeObject<IEnumerable<JiraCookie>>(jiraClaim.Value);

            foreach (var jiraCookie in jiraCookies)
            {
                var cookie = new Cookie(jiraCookie.Name, jiraCookie.Value);
                worklogService.Behaviour.CookieContainer.Add(worklogService.JiraBaseUri, cookie);
            }

            return worklogService;
        }

        private IEnumerable<JiraCookie> GetAuthorizationCookie(IWorklogService worklogService)
        {
            var cookies = worklogService.Behaviour.CookieContainer.GetCookies(worklogService.JiraBaseUri);

            var keyValuePairs = cookies.Cast<Cookie>().Select(x => new JiraCookie(x.Name, x.Value));

            return keyValuePairs;
        }

        public Claim GetClaimWithJiraSession(IWorklogService worklogService)
        {
            var jiraCookies = GetAuthorizationCookie(worklogService);

            return new Claim(JiraAuthCookie, JsonConvert.SerializeObject(jiraCookies));
        }
    }
}