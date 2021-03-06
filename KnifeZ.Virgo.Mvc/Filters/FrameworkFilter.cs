using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using KnifeZ.Virgo.Core;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using KnifeZ.Virgo.Core.Support.Json;
using Microsoft.Extensions.DependencyInjection;
using KnifeZ.Virgo.Core.Json;
using KnifeZ.Extensions;

namespace KnifeZ.Virgo.Mvc.Filters
{
    public class FrameworkFilter : ActionFilterAttribute
    {

        public override void OnActionExecuting (ActionExecutingContext context)
        {
            var ctrl = context.Controller as IBaseController;
            if (ctrl == null)
            {
                base.OnActionExecuting(context);
                return;
            }
            context.SetVirgoContext();
            if (context.HttpContext.Items.ContainsKey("actionstarttime") == false)
            {
                context.HttpContext.Items.Add("actionstarttime", DateTime.Now);
            }
            var ctrlActDesc = context.ActionDescriptor as ControllerActionDescriptor;
            var log = new SimpleLog();// 初始化log备用
            var ctrlDes = ctrlActDesc.ControllerTypeInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
            var actDes = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
            var postDes = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<HttpPostAttribute>().FirstOrDefault();
            var validpostonly = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(ValidateFormItemOnlyAttribute), false).Cast<ValidateFormItemOnlyAttribute>().FirstOrDefault();

            log.ITCode = ctrl.KnifeVirgo.LoginUserInfo?.ITCode ?? string.Empty;
            //给日志的多语言属性赋值
            log.ModuleName = ctrlDes?.GetDescription(ctrl) ?? ctrlActDesc.ControllerName;
            log.ActionName = actDes?.GetDescription(ctrl) ?? ctrlActDesc.ActionName + (postDes == null ? string.Empty : "[P]");
            log.ActionUrl = ctrl.KnifeVirgo.BaseUrl;
            log.IP = context.HttpContext.Connection.RemoteIpAddress.ToString();

            ctrl.KnifeVirgo.Log = log;
            foreach (var item in context.ActionArguments)
            {
                if (item.Value is BaseVM)
                {
                    var model = item.Value as BaseVM;
                    model.KnifeVirgo = ctrl.KnifeVirgo;
                    model.FC = new Dictionary<string, object>();
                    model.CreatorAssembly = this.GetType().Assembly.FullName;
                    model.ControllerName = context.HttpContext.Request.Path;
                    try
                    {
                        var f = context.HttpContext.Request.Form;
                        foreach (var key in f.Keys)
                        {
                            if (model.FC.Keys.Contains(key) == false)
                            {
                                model.FC.Add(key, f[key]);
                            }
                        }
                        if (context.HttpContext.Request.QueryString != QueryString.Empty)
                        {
                            foreach (var key in context.HttpContext.Request.Query.Keys)
                            {
                                if (model.FC.Keys.Contains(key) == false)
                                {
                                    model.FC.Add(key, context.HttpContext.Request.Query[key]);
                                }
                            }
                        }
                    }
                    catch { }

                    if (ctrl is BaseApiController apictrl)
                    {
                        //apictrl.TryValidateModel(model);
                        if (context.HttpContext.Items.ContainsKey("DONOTUSE_REQUESTBODY"))
                        {
                            string body = context.HttpContext.Items["DONOTUSE_REQUESTBODY"].ToString();
                            var joption = new JsonSerializerOptions();
                            joption.Converters.Add(new BodyConverter());
                            var obj = JsonSerializer.Deserialize<PostedBody>(body, joption);
                            foreach (var field in obj.ProNames)
                            {
                                model.FC.Add(field, null);
                            }
                        }
                    }
                    //if (model is IBaseCRUDVM<TopBasePoco> crud)
                    //{
                    //    var pros = crud.Entity.GetType().GetProperties();
                    //    foreach (var pro in pros)
                    //    {
                    //        if (model.FC.ContainsKey("Entity." + pro.Name))
                    //        {
                    //            //找到类型为List<xxx>的字段
                    //            if (pro.PropertyType.GenericTypeArguments.Count() > 0)
                    //            {
                    //                //获取xxx的类型
                    //                var ftype = pro.PropertyType.GenericTypeArguments.First();
                    //                //如果xxx继承自TopBasePoco
                    //                if (ftype.IsSubclassOf(typeof(TopBasePoco)))
                    //                {
                    //                    //界面传过来的子表数据

                    //                    if (pro.GetValue(crud.Entity) is IEnumerable<TopBasePoco> list && list.Count() == 0)
                    //                    {
                    //                        pro.SetValue(crud.Entity, null);
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    //如果ViewModel T继承自IBaseBatchVM<BaseVM>，则自动为其中的ListVM和EditModel初始化数据
                    if (model is IBaseBatchVM<BaseVM>)
                    {
                        var temp = model as IBaseBatchVM<BaseVM>;
                        if (temp.ListVM != null)
                        {
                            temp.ListVM.CopyContext(model);
                            temp.ListVM.Ids = temp.Ids == null ? new List<string>() : temp.Ids.ToList();
                            temp.ListVM.SearcherMode = ListVMSearchModeEnum.Batch;
                            temp.ListVM.NeedPage = false;
                        }
                        if (temp.LinkedVM != null)
                        {
                            temp.LinkedVM.CopyContext(model);
                        }
                        if (temp.ListVM != null)
                        {
                            //绑定ListVM的OnAfterInitList事件，当ListVM的InitList完成时，自动将操作列移除
                            temp.ListVM.OnAfterInitList += (self) =>
                            {
                                self.RemoveActionColumn();
                                self.AddErrorColumn();
                            };
                            if (temp.ListVM.Searcher != null)
                            {
                                var searcher = temp.ListVM.Searcher;
                                searcher.CopyContext(model);
                            }
                            temp.ListVM.DoInitListVM();
                        }
                        temp.LinkedVM?.DoInit();
                    }
                    if (model is IBaseImport<BaseTemplateVM>)
                    {
                        var template = (model as IBaseImport<BaseTemplateVM>).Template;
                        template.CopyContext(model);
                        template.DoReInit();
                    }
                    if (model is IBasePagedListVM<TopBasePoco, ISearcher> lvm)
                    {
                        var searcher = lvm.Searcher;
                        searcher.CopyContext(lvm);
                    }
                    model.Validate();
                    var invalid = ctrl.ModelState.Where(x => x.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid).Select(x => x.Key).ToList();
                    if ((ctrl as ControllerBase).Request.Method.ToLower() == "put" || validpostonly != null)
                    {
                        foreach (var v in invalid)
                        {
                            if (v?.StartsWith("Entity.") == true && model.FC.ContainsKey(v) == false)
                            {
                                ctrl.ModelState.Remove(v);
                            }
                        }
                    }
                    if (ctrl is BaseApiController)
                    {
                        var reinit = model.GetType().GetTypeInfo().GetCustomAttributes(typeof(ReInitAttribute), false).Cast<ReInitAttribute>().SingleOrDefault();
                        if (ctrl.ModelState.IsValid)
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

                    //如果是子表外键验证错误，例如Entity.Majors[0].SchoolId为空这种错误，则忽略。因为框架会在添加修改的时候自动给外键赋值
                    var toremove = ctrl.ModelState.Select(x => x.Key).Where(x => Regex.IsMatch(x, ".*?\\[.*?\\]\\..*?id", RegexOptions.IgnoreCase));
                    foreach (var r in toremove)
                    {
                        ctrl.ModelState.Remove(r);
                    }
                }

                if (item.Value is BaseSearcher se)
                {
                    se.FC = new Dictionary<string, object>();
                    se.KnifeVirgo = ctrl.KnifeVirgo;
                    se.Validate();
                }
            }

            base.OnActionExecuting(context);
        }



        public override void OnActionExecuted (ActionExecutedContext context)
        {
            BaseApiController ctrl = context.Controller as BaseApiController;
            if (ctrl == null)
            {
                base.OnActionExecuted(context);
                return;
            }
            //ctrl.ViewData["DONOTUSE_COOKIEPRE"] = ctrl.KnifeVirgo.ConfigInfo.CookiePre; TODO
            var ctrlActDesc = context.ActionDescriptor as ControllerActionDescriptor;
            if (context.Result is PartialViewResult)
            {
                var model = (context.Result as PartialViewResult).ViewData?.Model as BaseVM;
                if (model == null && (context.Result as PartialViewResult).Model == null && (context.Result as PartialViewResult).ViewData != null)
                {
                    model = ctrl.KnifeVirgo.CreateVM<BaseVM>();
                    (context.Result as PartialViewResult).ViewData.Model = model;
                }
                // 为所有 PartialView 加上最外层的 Div
                if (model != null)
                {
                    string pagetitle = string.Empty;
                    var menu = Utils.FindMenu(context.HttpContext.Request.Path, ctrl.KnifeVirgo.GlobaInfo.AllMenus);
                    if (menu == null)
                    {
                        var ctrlDes = ctrlActDesc.ControllerTypeInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
                        var actDes = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
                        if (actDes != null)
                        {
                            if (ctrlDes != null)
                            {
                                pagetitle = ctrlDes.GetDescription(ctrl) + " - ";
                            }
                            pagetitle += actDes.GetDescription(ctrl);
                        }
                    }
                    else
                    {
                        if (menu.ParentId != null)
                        {
                            var pmenu = ctrl.KnifeVirgo.GlobaInfo.AllMenus.Where(x => x.ID == menu.ParentId).FirstOrDefault();
                            if (pmenu != null)
                            {
                                if (ctrl.Localizer[pmenu.PageName].ResourceNotFound == true)
                                {
                                    pmenu.PageName = Core.CoreProgram.Callerlocalizer[pmenu.PageName];
                                }
                                else
                                {
                                    pmenu.PageName = ctrl.Localizer[pmenu.PageName];
                                }

                                pagetitle = pmenu.PageName + " - ";
                            }
                        }
                        if (ctrl.Localizer[menu.PageName].ResourceNotFound == true)
                        {
                            menu.PageName = Core.CoreProgram.Callerlocalizer[menu.PageName];
                        }
                        else
                        {
                            menu.PageName = ctrl.Localizer[menu.PageName];
                        }
                        pagetitle += menu.PageName;
                    }
                    if (string.IsNullOrEmpty(pagetitle) == false)
                    {
                        context.HttpContext.Response.Headers.Add("X-virgo-PageTitle", Convert.ToBase64String(Encoding.UTF8.GetBytes(pagetitle)));
                    }
                }
            }
            if (context.Result is ViewResult)
            {
                var model = (context.Result as ViewResult).ViewData?.Model as BaseVM;
                if (model == null && (context.Result as ViewResult).Model == null && (context.Result as ViewResult).ViewData != null)
                {
                    model = ctrl.KnifeVirgo.CreateVM<BaseVM>();
                    (context.Result as ViewResult).ViewData.Model = model;
                }
            }
            base.OnActionExecuted(context);
        }

        public override void OnResultExecuted (ResultExecutedContext context)
        {
            var ctrl = context.Controller as IBaseController;
            if (ctrl == null)
            {
                base.OnResultExecuted(context);
                return;
            }
            var ctrlActDesc = context.ActionDescriptor as ControllerActionDescriptor;
            var nolog = ctrlActDesc.MethodInfo.IsDefined(typeof(NoLogAttribute), false) || ctrlActDesc.ControllerTypeInfo.IsDefined(typeof(NoLogAttribute), false);

            //如果是来自Error，则已经记录过日志，跳过
            if (ctrlActDesc.ControllerName == "_Framework" && ctrlActDesc.ActionName == "Error")
            {
                return;
            }
            if (nolog == false)
            {
                var log = new ActionLog();
                var ctrlDes = ctrlActDesc.ControllerTypeInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
                var actDes = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
                var postDes = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<HttpPostAttribute>().FirstOrDefault();

                log.LogType = context.Exception == null ? ActionLogTypesEnum.Normal : ActionLogTypesEnum.Exception;
                log.ActionTime = DateTime.Now;
                log.ITCode = ctrl.KnifeVirgo.LoginUserInfo?.ITCode ?? string.Empty;
                // 给日志的多语言属性赋值
                log.ModuleName = ctrlDes?.GetDescription(ctrl) ?? ctrlActDesc.ControllerName;
                log.ActionName = actDes?.GetDescription(ctrl) ?? ctrlActDesc.ActionName + (postDes == null ? string.Empty : "[P]");
                log.ActionUrl = context.HttpContext.Request.Path;
                log.IP = context.HttpContext.GetRemoteIpAddress();
                log.Remark = context.Exception?.ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(log.Remark) == false && log.Remark.Length > 2000)
                {
                    log.Remark = log.Remark.Substring(0, 2000);
                }
                var starttime = context.HttpContext.Items["actionstarttime"] as DateTime?;
                if (starttime != null)
                {
                    log.Duration = DateTime.Now.Subtract(starttime.Value).TotalSeconds;
                }
                try
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ActionLog>>();
                    if (logger != null)
                    {
                        logger.Log<ActionLog>(LogLevel.Information, new EventId(), log, null, (a, b) =>
                        {
                            return a.GetLogString();
                        });
                    }
                }
                catch { }
            }
            if (context.Exception != null)
            {
                context.ExceptionHandled = true;
                if (ctrl.KnifeVirgo.ConfigInfo.IsQuickDebug == true)
                {
                    context.HttpContext.Response.WriteAsync(context.Exception.ToString());
                }
                else
                {
                    context.HttpContext.Response.WriteAsync(MvcProgram.Callerlocalizer["PageError"]);
                }
            }
            base.OnResultExecuted(context);
        }
    }
}
