using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using BLL;
using Common;
using Dapplo.Jira;
using Dapplo.Jira.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Tempo.Services;

namespace WebHost.ClientApi.Synchronization
{
    public class AuthenticationController : Controller
    {
        private readonly IApplicationUserService _userService;
        private IConfiguration _configuration;
        private readonly IJiraCookieAuthentication _cookieAuthentication;

        public AuthenticationController(IApplicationUserService userService, IConfiguration configuration, IJiraCookieAuthentication cookieAuthentication)
        {
            _userService = userService;
            _configuration = configuration;
            _cookieAuthentication = cookieAuthentication;
        }

        [HttpPost]
        [Authorize]
        [Route("api/[controller]/[action]")]
        public void SignInJira([FromBody] JiraAuthModel model)
        {
        }

        [HttpPost]
        [Authorize]
        [Route("api/[controller]/[action]")]
        public void SignInToggl([FromBody] TogglAuthModel model)
        {
            _userService.SaveTogglTokenForCurrentUser(model.Token);
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public LoginResult Login([FromBody] JiraAuthModel model)
        {
            var worklogService = new WorklogService(new Uri(_configuration.GetSection("Jira")["Host"]));
            try
            {
                worklogService.Session.StartAsync(model.User, model.Password).Wait();
            }
            catch
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return null;
            }

            // TODO [as] Rework
            var jiraClaim = _cookieAuthentication.GetClaimWithJiraSession(worklogService);
            Authenticate(model.User, jiraClaim);
            return new LoginResult { Username = model.User };
        }

        private int GetUserId(string userName)
        {
            return _userService.AddUserIfNecessary(userName);
        }

        private void Authenticate(string userName, Claim jiraClaim)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, GetUserId(userName).ToString()),
                jiraClaim
            };

            ClaimsIdentity id = new ClaimsIdentity(
                claims,
                "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            HttpContext.SignInAsync(Startup.TogglToTempoAuthScheme, new ClaimsPrincipal(id)).Wait();
        }
    }
}