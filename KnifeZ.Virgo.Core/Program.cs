using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace KnifeZ.Virgo.Core
{
    public class Program
    {
        public static IStringLocalizer _localizer =
            new ResourceManagerStringLocalizerFactory(Options.Create<LocalizationOptions>(new LocalizationOptions { ResourcesPath = "Resources" }), new Microsoft.Extensions.Logging.LoggerFactory()).Create(typeof(Program));

        public static IStringLocalizer Callerlocalizer { get; set; }

        public static string[] Buildindll = new string[]
            {
                    "KnifeZ.Virgo.Core",
                    "KnifeZ.Virgo.Mvc",
                    "KnifeZ.Virgo.Admin",
            };

    }
}
