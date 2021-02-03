using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace KnifeZ.Virgo.Core
{
    public class CoreProgram
    {
        public static IStringLocalizer Callerlocalizer { get; set; }

        public static string[] Buildindll = new string[]
            {
                    "KnifeZ.Virgo.Core",
                    "KnifeZ.Virgo.Mvc",
                    "KnifeZ.Virgo.Admin",
            };

    }
}
