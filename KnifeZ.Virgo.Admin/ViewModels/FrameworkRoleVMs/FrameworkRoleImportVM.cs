using KnifeZ.Virgo.Core;

namespace KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkRoleVMs
{
    public class FrameworkRoleTemplateVM : BaseTemplateVM
    {
        public ExcelPropety r1 = ExcelPropety.CreateProperty<FrameworkRole>(x => x.RoleCode);
        public ExcelPropety r2 = ExcelPropety.CreateProperty<FrameworkRole>(x => x.RoleName); 
        public ExcelPropety r3 = ExcelPropety.CreateProperty<FrameworkRole>(x => x.RoleRemark);
    }

    public class FrameworkRoleImportVM : BaseImportVM<FrameworkRoleTemplateVM, FrameworkRole>
    {

    }
}
