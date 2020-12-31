using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace KnifeZ.Virgo.Mvc
{
    [DebugOnly]
    [ActionDescription("VGen")]
    [Route("api/_[controller]")]
    public class VGenController : BaseApiController
    {
        [ActionDescription("获取Model列表")]
        [HttpGet("[action]")]
        public IActionResult GetModelList ()
        {
            return Ok(GlobaInfo.AllModels.ToListItems(x => x.Name, x => x.AssemblyQualifiedName));
        }

        [ActionDescription("获取字段列表")]
        [HttpGet("[action]")]
        public IActionResult GetModelFields (string modelName)
        {
            var vm = CreateVM<VGenCodeVM>();
            var res = vm.GetFieldInfos(modelName);
            return Ok(res);
        }

        [ActionDescription("生成代码")]
        [HttpPost("[action]")]
        public IActionResult GenerateCodes ([FromBody]VGenCodeModel model)
        {
            var vm = CreateVM<VGenCodeVM>();
            vm.CodeModel = model;
            string res = vm.GenTemplates();
            return Ok(res);
        }
    }
}
