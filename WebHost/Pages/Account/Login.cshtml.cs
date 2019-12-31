using System.Collections.Generic;
using System.Security.Claims;
using AspNet.Security.OAuth.Jira;
using BLL;
using Common.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebHost.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IApplicationUserService _userService;

        public LoginModel(IApplicationUserService userService)
        {
            _userService = userService;
        }

        public IActionResult OnPostOAuth()
        {
            var redirectUrl = Url.Page("./Login", "Callback");
            var authenticationProperties = new AuthenticationProperties {RedirectUri = redirectUrl};
            return new ChallengeResult(JiraDefaults.AuthenticationScheme, authenticationProperties);
        }

        public IActionResult OnPostBasicAuth()
        {
            var userName = Request.Form["username"];
            _userService.AddUser(userName, Request.Form["password"]);
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
            };

            ClaimsIdentity id = new ClaimsIdentity(
                claims,
                "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            var redirectUrl = Url.Page("./Login", "Callback");
            var authenticationProperties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return new SignInResult(Startup.TogglToTempoAuthScheme, new ClaimsPrincipal(id), authenticationProperties);
        }

        public IActionResult OnGetCallback(string returnUrl = null)
        {
            var login = HttpContext.User.Identity.GetNameId();
            _userService.AddUserIfNecessary(login);
            return Redirect("/");
        }
    }
}