using System.Security.Claims;
using System.Security.Principal;

namespace Common
{
    public static class Identities
    {
        public static string GetNameId(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            return claim?.Value ?? string.Empty;
        }

        public static string GetSelf(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst("urn:jira:self");

            return claim?.Value ?? string.Empty;
        }
    }
}