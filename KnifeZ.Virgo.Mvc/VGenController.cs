using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            return Ok(GetAllModels().ToListItems(x => x.Name, x => x.AssemblyQualifiedName));
        }

        [ActionDescription("获取字段列表")]
        [HttpGet("[action]")]
        public IActionResult GetModelFields (string modelName)
        {
            var vm = KnifeVirgo.CreateVM<VGenCodeVM>();
            var res = vm.GetFieldInfos(modelName);
            return Ok(res);
        }

        [ActionDescription("生成代码")]
        [HttpPost("[action]")]
        public IActionResult GenerateCodes ([FromBody] VGenCodeModel model)
        {
            var vm = KnifeVirgo.CreateVM<VGenCodeVM>();
            vm.CodeModel = model;
            string res = vm.GenTemplates();
            return Ok(res);
        }

        private List<Type> GetAllModels ()
        {
            var models = new List<Type>();

            //获取所有模型
            var pros = KnifeVirgo.ConfigInfo.ConnectionStrings.SelectMany(x => x.DcConstructor.DeclaringType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance));
            if (pros != null)
            {
                foreach (var pro in pros)
                {
                    if (pro.PropertyType.IsGeneric(typeof(DbSet<>)))
                    {
                        models.Add(pro.PropertyType.GetGenericArguments()[0]);
                    }
                }
            }
            return models;
        }
    }
}
