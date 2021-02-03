using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace KnifeZ.Virgo.Mvc
{
    public class MvcProgram
    {
        public static IStringLocalizer Callerlocalizer = Core.CoreProgram.Callerlocalizer;
    }
}
