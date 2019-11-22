using IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Authentication.Server
{
    public static class IdentityHelperExtensions
    {
        public static long GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            var claim = principal.FindFirst(JwtClaimTypes.Id);

            if (claim != null)
            {
                return Convert.ToInt64(claim.Value);
            }

            return 0;
        }
    }
}
