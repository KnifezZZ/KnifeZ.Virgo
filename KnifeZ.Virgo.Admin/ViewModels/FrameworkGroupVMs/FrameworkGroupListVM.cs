using System.Collections.Generic;
using System.Linq;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;

namespace KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkGroupVMs
{
    public class FrameworkGroupListVM : BasePagedListVM<FrameworkGroup, FrameworkGroupSearcher>
    {
        protected override IEnumerable<IGridColumn<FrameworkGroup>> InitGridHeader()
        {
            return new List<GridColumn<FrameworkGroup>>{
                this.MakeGridHeader(x => x.GroupCode),
                this.MakeGridHeader(x => x.GroupName),
                this.MakeGridHeader(x => x.GroupRemark),
                this.MakeGridHeaderAction(width: 300)
            };
        }

        public override IOrderedQueryable<FrameworkGroup> GetSearchQuery()
        {
            var query = DC.Set<FrameworkGroup>()
                .CheckContain(Searcher.GroupCode, x=>x.GroupCode)
                .CheckContain(Searcher.GroupName, x=>x.GroupName)
                .OrderBy(x => x.GroupCode);
            return query;
        }

    }
}
