using System.Security.Claims;
using System.Security.Principal;

namespace Common.Extensions
{
    public static class Identities
    {
        public static string GetNameId(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            return claim?.Value ?? string.Empty;
        }

        public static string GetSelfAccountId(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst("urn:jira:self");

            return claim?.Value.Replace("https://oneinc.atlassian.net/rest/api/2/user?accountId=", string.Empty) ?? string.Empty;
        }
    }
}