using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace KnifeZ.Virgo.Core.Auth.Attribute
{
    public class AuthorizeCookieAttribute : AuthorizeAttribute
    {
        public AuthorizeCookieAttribute()
        {
            AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme;
        }
    }

}
