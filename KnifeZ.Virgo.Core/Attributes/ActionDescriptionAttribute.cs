using System;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace KnifeZ.Virgo.Core
{
    /// <summary>
    /// 标记Controller和Action的描述
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class ActionDescriptionAttribute : Attribute
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        public IStringLocalizer Localizer { get; set; }
        /// <summary>
        /// 新建一个描述
        /// </summary>
        public ActionDescriptionAttribute(string desc)
        {
            this.Description = desc;
        }

        public void SetLoccalizer(Type controllertype)
        {
            if (Core.Program.Buildindll.Any(x => controllertype.FullName.StartsWith(x)))
            {
                if (Core.Program._localizer != null)
                {
                    Localizer = Core.Program._localizer;
                }
            }
            else
            {
                if (Core.Program.Callerlocalizer != null)
                {
                    Localizer = Core.Program.Callerlocalizer;
                }
            }
        }
    }
}
