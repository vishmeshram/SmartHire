using System.Security.Claims;

namespace SmartHire.Helpers
{
    public static class ClaimsHelper
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(claim!.Value);
        }
    }
}
