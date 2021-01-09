using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace KnifeZ.Virgo.Core
{
    public class JwtOption
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int Expires { get; set; }
        public string SecurityKey { get; set; }
        public string LoginPath { get; set; }
        public int RefreshTokenExpires { get; set; }
    }
}
