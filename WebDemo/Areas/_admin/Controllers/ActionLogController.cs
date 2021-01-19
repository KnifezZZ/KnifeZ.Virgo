using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;
using KnifeZ.Virgo.Mvc;
using KnifeZ.Virgo.Mvc.Admin.ViewModels.ActionLogVMs;

namespace KnifeZ.Virgo.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ActionDescription("Log")]
    [ApiController]
    [Route("api/_[controller]")]
    public class ActionLogController : BaseApiController
    {
        [ActionDescription("Search")]
        [HttpPost("[action]")]
        public IActionResult Search(ActionLogSearcher searcher)
        {
            if (ModelState.IsValid)
            {
                var vm = KnifeVirgo.CreateVM<ActionLogListVM>(passInit:true);
                vm.Searcher = searcher;
                return Content(vm.GetJson());
            }
            else
            {
                return BadRequest(ModelState.GetErrorJson());
            }
        }

        [ActionDescription("Get")]
        [HttpGet("{id}")]
        public ActionLogVM Get(Guid id)
        {
            var vm = KnifeVirgo.CreateVM<ActionLogVM>(id);
            return vm;
        }

        [HttpPost("[action]")]
        [ActionDescription("Delete")]
        public IActionResult BatchDelete(string[] ids)
        {
            var vm = KnifeVirgo.CreateVM<ActionLogBatchVM>();
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
        public IActionResult ExportExcel(ActionLogSearcher searcher)
        {
            var vm = KnifeVirgo.CreateVM<ActionLogListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return vm.GetExportData();
        }

        [ActionDescription("ExportByIds")]
        [HttpPost("[action]")]
        public IActionResult ExportExcelByIds(string[] ids)
        {
            var vm = KnifeVirgo.CreateVM<ActionLogListVM>();
            if (ids != null && ids.Length > 0)
            {
                vm.Ids = new List<string>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            return vm.GetExportData();
        }
    }
}
