using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace KnifeZ.Virgo.Mvc
{
    public class AuthorizeJwtAttribute : AuthorizeAttribute
    {
        public AuthorizeJwtAttribute()
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }
    }

}
