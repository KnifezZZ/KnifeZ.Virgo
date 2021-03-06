using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KnifeZ.Virgo.Core;

namespace KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkRoleVMs
{
    public class FrameworkRoleBatchVM : BaseBatchVM<FrameworkRole, BaseVM>
    {
        public FrameworkRoleBatchVM()
        {
            ListVM = new FrameworkRoleListVM();
        }

        protected override bool CheckIfCanDelete(object id, out string errorMessage)
        {
            Guid? checkid = Guid.Parse(id.ToString());
            var check = DC.Set<FrameworkUserRole>().Any(x => x.RoleId == checkid);
            if (check == true)
            {
                errorMessage = Localizer["CannotDelete", Localizer["Role"]];
                return false;
            }
            else
            {
                errorMessage = null;
                return true;
            }
        }
    }
}
