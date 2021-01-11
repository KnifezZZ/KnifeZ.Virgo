using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;
using KnifeZ.Virgo.Mvc;

using WebDemo.ViewModel.BlogCategoryVMs;
using WebDemo.Model;

namespace WebDemo.Controllers
{
    
    [AuthorizeJwtWithCookie]
    [ActionDescription("类别管理")]
    [ApiController]
    [Route("api/[controller]")]
    public class BlogCategoryController : BaseApiController
    {
        #region 基础增删改查

        [ActionDescription("Search")]
        [HttpPost("[action]")]
        public string Search (BlogCategorySearcher searcher)
        {
            var vm = KnifeVirgo.CreateVM <BlogCategoryListVM > ();
            vm.Searcher = searcher;
            return vm.GetJson();
        }

        [ActionDescription("Get")]
        [HttpGet("{id}")]
        public BlogCategoryVM Get (Guid id)
        {
            var vm = KnifeVirgo.CreateVM <BlogCategoryVM > (id);
            return vm;
        }

        [ActionDescription("Create")]
        [HttpPost("[action]")]
        public IActionResult Add (BlogCategoryVM vm)
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
        public IActionResult Edit (BlogCategoryVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                vm.DoEdit(false);
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
            var vm = KnifeVirgo.CreateVM<BlogCategoryBatchVM>();
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
        public IActionResult ExportExcel (BlogCategorySearcher searcher)
        {
            var vm = KnifeVirgo.CreateVM <BlogCategoryListVM > ();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return vm.GetExportData();
        }

        [ActionDescription("ExportByIds")]
        [HttpPost("[action]")]
        public IActionResult ExportExcelByIds (string[] ids)
        {
            var vm = KnifeVirgo.CreateVM <BlogCategoryListVM > ();
            if (ids != null && ids.Length > 0)
            {
                vm.Ids = new List<string>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            return vm.GetExportData();
        }

        [ActionDescription("DownloadTemplate")]
        [HttpGet("[action]")]
        public IActionResult GetExcelTemplate ()
        {
            var vm = KnifeVirgo.CreateVM <BlogCategoryImportVM > ();
            var qs = new Dictionary<string, string>();
            foreach (var item in Request.Query.Keys)
            {
                qs.Add(item, Request.Query[item]);
            }
            vm.SetParms(qs);
            var data = vm.GenerateTemplate(out string fileName);
            return File(data, "application/vnd.ms-excel", fileName);
        }

        [ActionDescription("Import")]
        [HttpPost("[action]")]
        public ActionResult Import (BlogCategoryImportVM vm)
        {

            if (vm.ErrorListVM.EntityList.Count > 0 || !vm.BatchSaveData())
            {
                return BadRequest(vm.GetErrorJson());
            }
            else
            {
                return Ok(vm.EntityList.Count);
            }
        }

        #endregion
        
        
        [ActionDescription("获取BlogCategory列表")]
        [HttpGet("[action]")]
        public ActionResult GetBlogCategoryList()
        {
            return Ok(KnifeVirgo.DC.Set<BlogCategory>().GetTreeSelectListItems(KnifeVirgo, x => x.Name));
        }

    }
}

