using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Auth;
using KnifeZ.Virgo.Core.Extensions;
using KnifeZ.Virgo.Core.Support.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace KnifeZ.Virgo.Mvc
{
    public abstract class BaseApiController : ControllerBase, IBaseController
    {
        [JsonIgnore]
        [BindNever]
        public VirgoContext KnifeVirgo { get; set; }

        [JsonIgnore]
        [BindNever]
        public IStringLocalizer Localizer { get => KnifeVirgo?.Localizer; }
        //-------------------------------------------方法------------------------------------//



        #region ReInit model
        [NonAction]
        private void SetReInit (ModelStateDictionary msd, BaseVM model)
        {
            var reinit = model.GetType().GetTypeInfo().GetCustomAttributes(typeof(ReInitAttribute), false).Cast<ReInitAttribute>().SingleOrDefault();

            if (ModelState.IsValid)
            {
                if (reinit != null && (reinit.ReInitMode == ReInitModes.SUCCESSONLY || reinit.ReInitMode == ReInitModes.ALWAYS))
                {
                    model.DoReInit();
                }
            }
            else
            {
                if (reinit == null || (reinit.ReInitMode == ReInitModes.FAILEDONLY || reinit.ReInitMode == ReInitModes.ALWAYS))
                {
                    model.DoReInit();
                }
            }
        }
        #endregion

        #region Validate model
        [NonAction]
        public Dictionary<string, string> RedoValidation (object item)
        {
            Dictionary<string, string> rv = new Dictionary<string, string>();
            TryValidateModel(item);

            foreach (var e in ControllerContext.ModelState)
            {
                if (e.Value.ValidationState == ModelValidationState.Invalid)
                {
                    rv.Add(e.Key, e.Value.Errors.Select(x => x.ErrorMessage).ToSepratedString());
                }
            }

            return rv;
        }
        #endregion

        #region update viewmodel
        /// <summary>
        /// Set viewmodel's properties to the matching items posted by user
        /// </summary>
        /// <param name="vm">ViewModel</param>
        /// <param name="prefix">prefix</param>
        /// <returns>true if success</returns>
        [NonAction]
        public bool RedoUpdateModel (object vm, string prefix = null)
        {
            try
            {
                BaseVM bvm = vm as BaseVM;
                foreach (var item in bvm.FC.Keys)
                {
                    PropertyHelper.SetPropertyValue(vm, item, bvm.FC[item], prefix, true);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        protected JsonResult JsonMore (object data, int statusCode = StatusCodes.Status200OK, string msg = "success")
        {
            return new JsonResult(new JsonResultT<object> { Msg = msg, Code = statusCode, Data = data });
        }
    }

}
