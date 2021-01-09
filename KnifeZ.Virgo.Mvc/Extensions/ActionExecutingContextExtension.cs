using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnifeZ.Virgo.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace KnifeZ.Virgo.Mvc
{

    public static class ActionExecutingContextExtension
    {
        public static void SetVirgoContext (this ActionExecutingContext self)
        {
            var controller = self.Controller as IBaseController;
            if (controller == null)
            {
                return;
            }
            if (controller.KnifeVirgo == null)
            {
                controller.KnifeVirgo = self.HttpContext.RequestServices.GetRequiredService<VirgoContext>();
            }
            try
            {
                controller.KnifeVirgo.MSD = new ModelStateServiceProvider(self.ModelState);
                controller.KnifeVirgo.Session = new SessionServiceProvider(self.HttpContext.Session);
            }
            catch { }
        }
    }
}
