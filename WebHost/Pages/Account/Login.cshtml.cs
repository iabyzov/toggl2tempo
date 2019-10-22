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

        public IActionResult OnPost()
        {
            var redirectUrl = Url.Page("./Login", "Callback");
            var authenticationProperties = new AuthenticationProperties {RedirectUri = redirectUrl};
            return new ChallengeResult(JiraDefaults.AuthenticationScheme, authenticationProperties);
        }

        public IActionResult OnGetCallback(string returnUrl = null)
        {
            var login = HttpContext.User.Identity.GetNameId();
            _userService.AddUserIfNecessary(login);
            return Redirect("/");
        }
    }
}