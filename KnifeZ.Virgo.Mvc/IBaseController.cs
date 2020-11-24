using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;

using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Support.Json;

namespace KnifeZ.Virgo.Mvc
{
    public interface IBaseController
    {
        Configs ConfigInfo { get; }
        GlobalData GlobaInfo { get; }
        string CurrentCS { get; set; }

        DBTypeEnum? CurrentDbType { get; set; }

        IDataContext DC { get; set; }
        LoginUserInfo LoginUserInfo { get; set; }

        IDistributedCache Cache { get; }

        string BaseUrl { get; set; }

        SimpleLog Log { get; set; }

        IDataContext CreateDC(bool isLog = false);

        ModelStateDictionary ModelState { get; }

        IStringLocalizer Localizer { get; }
    }
}
