using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Localization;
using KnifeZ.Virgo.Core;

namespace KnifeZ.Virgo.Mvc
{
    public static class ActionDescriptionExtension
    {
        public static string GetDescription(this ActionDescriptionAttribute self, IBaseController controller)
        {
            return self.GetDescription(controller.GetType());
        }

        public static string GetDescription(this ActionDescriptionAttribute self, Type controllertype)
        {
            string rv = "";
            if (string.IsNullOrEmpty(self.Description) == false)
            {
                if (Core.Program.Callerlocalizer[self.Description].ResourceNotFound == true)
                {
                    rv = Core.Program._localizer[self.Description];
                }
                else
                {
                    rv = Core.Program.Callerlocalizer[self.Description];
                }
            }
            return rv;

        }

    }
}
