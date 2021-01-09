using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace KnifeZ.Virgo.Mvc
{
    public class AuthorizeCookieAttribute : AuthorizeAttribute
    {
        public AuthorizeCookieAttribute()
        {
            AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme;
        }
    }

}
