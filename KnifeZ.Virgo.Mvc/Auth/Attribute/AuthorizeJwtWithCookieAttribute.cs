using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace KnifeZ.Virgo.Mvc
{
    public class AuthorizeJwtWithCookieAttribute : AuthorizeAttribute
    {
        public AuthorizeJwtWithCookieAttribute()
        {
            AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}";
        }
    }
}
