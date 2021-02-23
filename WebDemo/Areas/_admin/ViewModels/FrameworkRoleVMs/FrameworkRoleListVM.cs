using System.Collections.Generic;
using System.Linq;
using KnifeZ.Virgo.Core;
using KnifeZ.Extensions;

namespace KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkRoleVMs
{
    public class FrameworkRoleListVM : BasePagedListVM<FrameworkRole, FrameworkRoleSearcher>
    {

        protected override IEnumerable<IGridColumn<FrameworkRole>> InitGridHeader()
        {
            return new List<GridColumn<FrameworkRole>>{
                this.MakeGridHeader(x => x.RoleCode),
                this.MakeGridHeader(x => x.RoleName),
                this.MakeGridHeader(x => x.RoleRemark),
            };
        }

        public override IOrderedQueryable<FrameworkRole> GetSearchQuery()
        {
            //var query = DC.Set<FrameworkRole>()
            //    .CheckWhere(Searcher.RoleCode, x => x.RoleCode.Contains(Searcher.RoleCode))
            //    .CheckWhere(Searcher.RoleName, x => x.RoleName.ToLower().Contains(Searcher.RoleName.ToLower()))
            //    .OrderBy(x => x.RoleCode);
            var query = DC.Set<FrameworkRole>()
                .CheckContain(Searcher.RoleCode,x=>x.RoleCode)
                .CheckContain(Searcher.RoleName,x=>x.RoleName)
                .OrderBy(x => x.RoleCode);
            return query;
        }

    }
}
