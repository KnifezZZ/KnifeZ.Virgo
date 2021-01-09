using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

using KnifeZ.Virgo.Core.Auth;

namespace KnifeZ.Virgo.Core.Extensions
{
    public static class LoginUserInfoExtension
    {
        public static ClaimsPrincipal CreatePrincipal (this LoginUserInfo self)
        {
            if (self.Id == Guid.Empty) throw new ArgumentException("Id is mandatory", nameof(self.Id));
            var claims = new List<Claim> { new Claim(AuthConstants.JwtClaimTypes.Subject, self.Id.ToString()) };

            if (!string.IsNullOrEmpty(self.Name))
            {
                claims.Add(new Claim(AuthConstants.JwtClaimTypes.Name, self.Name));
            }

            var id = new ClaimsIdentity(
                claims.Distinct(new ClaimComparer()),
                AuthConstants.AuthenticationType,
                AuthConstants.JwtClaimTypes.Name,
                AuthConstants.JwtClaimTypes.Role);
            return new ClaimsPrincipal(id);
        }
    }
}
