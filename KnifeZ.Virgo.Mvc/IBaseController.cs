using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;

using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Support.Json;

namespace KnifeZ.Virgo.Mvc
{
    public interface IBaseController
    {
        /// <summary>
        /// KnifeZ.Virgo统一入口
        /// </summary>
        VirgoContext KnifeVirgo { get; set; }

        ModelStateDictionary ModelState { get; }
        IStringLocalizer Localizer { get; }
    }
}
