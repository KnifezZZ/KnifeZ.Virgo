using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KnifeZ.Extensions;
using KnifeZ.Virgo.Core;

namespace KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkGroupVMs
{
    public class FrameworkGroupVM : BaseCRUDVM<FrameworkGroup>
    {
        public override DuplicatedInfo<FrameworkGroup> SetDuplicatedCheck()
        {
            var rv = CreateFieldsInfo(SimpleField(x => x.GroupName));
            rv.AddGroup(SimpleField(x => x.GroupCode));
            return rv;
        }
    }
}
