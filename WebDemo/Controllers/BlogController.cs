using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;
using KnifeZ.Virgo.Mvc;

using WebDemo.ViewModel.BlogVMs;
using WebDemo.Model;

namespace WebDemo.Controllers
{
    
    [AuthorizeJwtWithCookie]
    [ActionDescription("博客管理")]
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : BaseApiController
    {
        #region 基础增删改查

        [ActionDescription("Search")]
        [HttpPost("[action]")]
        public string Search (BlogSearcher searcher)
        {
            var vm = KnifeVirgo.CreateVM <BlogListVM > ();
            vm.Searcher = searcher;
            return vm.GetJson();
        }

        [ActionDescription("Get")]
        [HttpGet("{id}")]
        public BlogVM Get (Guid id)
        {
            var vm = KnifeVirgo.CreateVM <BlogVM > (id);
            return vm;
        }

        [ActionDescription("Create")]
        [HttpPost("[action]")]
        public IActionResult Add (BlogVM vm)
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
        public IActionResult Edit (BlogVM vm)
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
            var vm = KnifeVirgo.CreateVM<BlogBatchVM>();
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
        public IActionResult ExportExcel (BlogSearcher searcher)
        {
            var vm = KnifeVirgo.CreateVM <BlogListVM > ();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return vm.GetExportData();
        }

        [ActionDescription("ExportByIds")]
        [HttpPost("[action]")]
        public IActionResult ExportExcelByIds (string[] ids)
        {
            var vm = KnifeVirgo.CreateVM <BlogListVM > ();
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
            var vm = KnifeVirgo.CreateVM <BlogImportVM > ();
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
        public ActionResult Import (BlogImportVM vm)
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

        [ActionDescription("获取BlogClassification列表")]
        [HttpGet("[action]")]
        public ActionResult GetBlogClassificationList()
        {
            return Ok(KnifeVirgo.DC.Set<BlogClassification>().GetSelectListItems(KnifeVirgo, x => x.Name));
        }

    }
}

