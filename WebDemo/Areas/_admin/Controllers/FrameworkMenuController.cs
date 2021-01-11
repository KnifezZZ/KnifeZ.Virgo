using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;
using KnifeZ.Virgo.Core.Support.Json;
using KnifeZ.Virgo.Mvc;
using KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkMenuVMs;

namespace KnifeZ.Virgo.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ActionDescription("MenuMangement")]
    [ApiController]
    [Route("api/_[controller]")]
    public class FrameworkMenuController : BaseApiController
    {
        [ActionDescription("Search")]
        [HttpPost("[action]")]
        public string Search (FrameworkMenuSearcher searcher)
        {
            var vm = KnifeVirgo.CreateVM<FrameworkMenuListVM2>();
            vm.Searcher = searcher;
            return vm.GetJson();
        }

        [ActionDescription("Get")]
        [HttpGet("{id}")]
        public FrameworkMenuVM2 Get (Guid id)
        {
            var vm = KnifeVirgo.CreateVM<FrameworkMenuVM2>(id);
            return vm;
        }

        [ActionDescription("Create")]
        [HttpPost("[action]")]
        public IActionResult Add (FrameworkMenuVM2 vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                vm.DoAdd();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.GetErrorJson());
                }
                else
                {
                    return Ok(vm.Entity);
                }
            }

        }

        [ActionDescription("Edit")]
        [HttpPut("[action]")]
        public IActionResult Edit (FrameworkMenuVM2 vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                vm.DoEdit(true);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.GetErrorJson());
                }
                else
                {
                    return Ok(vm.Entity);
                }
            }
        }

        [HttpPost("BatchDelete")]
        [ActionDescription("Delete")]
        public IActionResult BatchDelete (string[] ids)
        {
            var vm = KnifeVirgo.CreateVM<FrameworkMenuBatchVM>();
            if (ids != null && ids.Length > 0)
            {
                vm.Ids = ids;
            }
            else
            {
                return Ok();
            }
            if (!ModelState.IsValid || !vm.DoBatchDelete())
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                return Ok(ids.Length);
            }
        }

        [ActionDescription("Export")]
        [HttpPost("[action]")]
        public IActionResult ExportExcel (FrameworkMenuSearcher searcher)
        {
            var vm = KnifeVirgo.CreateVM<FrameworkMenuListVM2>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return vm.GetExportData();
        }

        [ActionDescription("ExportByIds")]
        [HttpPost("[action]")]
        public IActionResult ExportExcelByIds (string[] ids)
        {
            var vm = KnifeVirgo.CreateVM<FrameworkMenuListVM2>();
            if (ids != null && ids.Length > 0)
            {
                vm.Ids = new List<string>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            return vm.GetExportData();
        }

        #region 未设置页面
        [ActionDescription("UnsetPages")]
        [HttpGet("[action]")]
        public string UnsetPages ()
        {
            var vm = KnifeVirgo.CreateVM<FrameworkActionListVM>();
            return vm.GetJson();
        }
        #endregion

        #region 刷新菜单
        [ActionDescription("RefreshMenu")]
        [HttpGet("[action]")]
        public async Task<ActionResult> RefreshMenu ()
        {
            KnifeVirgo.Cache.Delete("FFMenus");
            var userids = KnifeVirgo.DC.Set<FrameworkUserBase>().Select(x => x.ID.ToString().ToLower()).ToArray();
            await KnifeVirgo.RemoveUserCache(userids);
            return Ok(Localizer["OprationSuccess"]);
        }
        #endregion

        [ActionDescription("GetActionsByModelId")]
        [HttpGet("GetActionsByModel")]
        public ActionResult GetActionsByModel (string ModelName)
        {
            if (ModelName.StartsWith("MenuKey."))
            {
                ModelName = Localizer[ModelName];
            }
            var menu = KnifeVirgo.GlobaInfo.AllModule.Where(x => x.IsApi == true && x.ModuleName.ToLower() == ModelName.ToLower()).FirstOrDefault();
            if (menu == null)
            {
                return Ok(new List<ComboSelectListItem>());
            }
            var m = menu.Actions;
            List<SimpleAction> toremove = new List<SimpleAction>();
            foreach (var item in m)
            {
                if (item.IgnorePrivillege == true || item.Module.IgnorePrivillege == true)
                {
                    toremove.Add(item);
                }
            }
            toremove.ForEach(x => m.Remove(x));
            var actions = m.ToListItems(y => y.ActionName, y => y.MethodName);
            actions.ForEach(x => x.Selected = true);
            return Ok(actions);
        }
        [AllRights]
        [ActionDescription("GetModules")]
        [HttpGet("GetAllModules")]
        public ActionResult GetAllModules ()
        {
            List<ComboSelectListItem> comboSelects = new List<ComboSelectListItem>();
            var menus = KnifeVirgo.GlobaInfo.AllModule.Where(x => x.IsApi == true).ToList();
            foreach (var item in menus)
            {
                comboSelects.Add(new ComboSelectListItem()
                {
                    Text = item.ModuleName,
                    Value = item.FullName
                });
            }
            return Ok(comboSelects);
        }

        [AllRights]
        [ActionDescription("GetFolders")]
        [HttpGet("GetFolders")]
        public ActionResult GetFolders ()
        {
            var AllParents = KnifeVirgo.DC.Set<FrameworkMenu>().Where(x => x.FolderOnly == true).OrderBy(x => x.DisplayOrder).GetSelectListItems(KnifeVirgo, x => x.PageName);
            foreach (var p in AllParents)
            {
                if (p.Text.StartsWith("MenuKey."))
                {
                    p.Text = Localizer[p.Text];
                }
            }

            return Ok(AllParents);
        }

    }

}
