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
                if (Core.CoreProgram.Callerlocalizer[self.Description].ResourceNotFound == true)
                {
                    rv = Core.CoreProgram.Callerlocalizer[self.Description];
                }
                else
                {
                    rv = Core.CoreProgram.Callerlocalizer[self.Description];
                }
            }
            return rv;

        }

    }
}
