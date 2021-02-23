using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KnifeZ.Extensions;
using KnifeZ.Virgo.Core;

namespace KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkRoleVMs
{
    public class FrameworkRoleVM : BaseCRUDVM<FrameworkRole>
    {
        public override DuplicatedInfo<FrameworkRole> SetDuplicatedCheck()
        {
            var rv = CreateFieldsInfo(SimpleField(x => x.RoleName));
            rv.AddGroup(SimpleField(x => x.RoleCode));
            return rv;
        }
    }
}
