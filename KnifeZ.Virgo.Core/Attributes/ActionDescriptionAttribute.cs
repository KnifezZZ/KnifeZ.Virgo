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

        public IStringLocalizer _localizer { get; set; }
        /// <summary>
        /// 新建一个描述
        /// </summary>
        public ActionDescriptionAttribute (string desc)
        {
            this.Description = desc;
        }

        public void SetLoccalizer (Type controllertype)
        {
            if (Core.CoreProgram.Buildindll.Any(x => controllertype.FullName.StartsWith(x)))
            {
                if (Core.CoreProgram.Callerlocalizer != null)
                {
                    _localizer = Core.CoreProgram.Callerlocalizer;
                }
            }
            else
            {
                if (Core.CoreProgram.Callerlocalizer != null)
                {
                    _localizer = Core.CoreProgram.Callerlocalizer;
                }
            }
        }
    }
}