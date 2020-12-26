using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Auth.Attribute;
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
                var vm = CreateVM<ActionLogListVM>();
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
            var vm = CreateVM<ActionLogVM>(id);
            return vm;
        }

        [HttpPost("[action]")]
        [ActionDescription("Delete")]
        public IActionResult BatchDelete(string[] ids)
        {
            var vm = CreateVM<ActionLogBatchVM>();
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
            var vm = CreateVM<ActionLogListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return vm.GetExportData();
        }

        [ActionDescription("ExportByIds")]
        [HttpPost("[action]")]
        public IActionResult ExportExcelByIds(string[] ids)
        {
            var vm = CreateVM<ActionLogListVM>();
            if (ids != null && ids.Length > 0)
            {
                vm.Ids = new List<string>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            return vm.GetExportData();
        }
    }
}
