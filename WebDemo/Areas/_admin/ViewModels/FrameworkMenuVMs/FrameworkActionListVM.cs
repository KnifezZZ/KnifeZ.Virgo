using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using KnifeZ.Extensions;
using KnifeZ.Virgo.Core;

namespace KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkMenuVMs
{
    public class FrameworkActionListVM : BasePagedListVM<FrameworkAction_ListView, BaseSearcher>
    {

        public FrameworkActionListVM()
        {
            NeedPage = false;
        }


        protected override IEnumerable<IGridColumn<FrameworkAction_ListView>> InitGridHeader()
        {
            var header = new List<GridColumn<FrameworkAction_ListView>>
            {
                this.MakeGridHeader(x => x.ModuleName),
                this.MakeGridHeader(x => x.ActionName),
                this.MakeGridHeader(x => x.ClassName),
                this.MakeGridHeader(x => x.MethodName)
            };

            return header;
        }

        /// <summary>
        /// 查询结果
        /// </summary>
        public override IOrderedQueryable<FrameworkAction_ListView> GetSearchQuery()
        {
            var newdc = DC as FrameworkContext;
            List<FrameworkAction_ListView> actions = new List<FrameworkAction_ListView>();
            var urls = newdc.BaseFrameworkMenus.Where(y => y.IsInside == true && y.FolderOnly == false).Select(y => y.Url).Distinct().ToList();
            if (ControllerName.Contains("/api") == false)
            {
                actions = KnifeVirgo.GlobaInfo.AllModule.SelectMany(x=>x.Actions)
                    .Where(x => urls.Contains(x.Url) == false)
                    .Select(x => new FrameworkAction_ListView
                    {
                        ID = x.ID,
                        ModuleID = x.ModuleId,
                        ModuleName = x.Module.ModuleName,
                        ActionName = x.ActionName,
                        ClassName = x.Module.ClassName,
                        MethodName = x.MethodName,
                        AreaName = x.Module.Area?.AreaName
                    }).ToList();
            }
            else
            {
                actions = KnifeVirgo.GlobaInfo.AllModule.SelectMany(x => x.Actions)
                   .Where(x => x.Module.IsApi == true && urls.Contains(x.Url) == false)
                   .Select(x => new FrameworkAction_ListView
                    {
                        ID = x.ID,
                        ModuleID = x.ModuleId,
                        ModuleName = x.Module.ModuleName,
                        ActionName = x.ActionName,
                        ClassName = x.Module.ClassName,
                        MethodName = x.MethodName,
                        AreaName = x.Module.Area?.AreaName
                    }).ToList();

            }

            var modules = KnifeVirgo.GlobaInfo.AllModule;
            List<FrameworkAction_ListView> toremove = new List<FrameworkAction_ListView>();
            foreach (var item in actions)
            {
                var m = modules.Where(x => x.ClassName == item.ClassName && x.Area?.AreaName == item.AreaName).FirstOrDefault();
                var a = m?.Actions.Where(x => x.MethodName == item.MethodName).FirstOrDefault();
                if(m?.IgnorePrivillege == true || a?.IgnorePrivillege == true)
                {
                    toremove.Add(item);
                }
            }
            toremove.ForEach(x => actions.Remove(x));
            return actions.AsQueryable().OrderBy(x=>x.AreaName).ThenBy(x=>x.ModuleName).ThenBy(x=>x.MethodName);
        }

    }

    public class FrameworkAction_ListView : BasePoco
    {
        public Guid? ModuleID { get; set; }

        [Display(Name = "ModuleName")]
        public string ModuleName { get; set; }
        [Display(Name = "ActionName")]
        public string ActionName { get; set; }
        [Display(Name = "ClassName")]
        public string ClassName { get; set; }
        [Display(Name = "MethodName")]
        public string MethodName { get; set; }

        public string AreaName { get; set; }

    }
}
