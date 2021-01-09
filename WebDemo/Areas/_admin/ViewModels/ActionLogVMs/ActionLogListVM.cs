using System.Collections.Generic;
using System.Linq;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;

namespace KnifeZ.Virgo.Mvc.Admin.ViewModels.ActionLogVMs
{
    public class ActionLogListVM : BasePagedListVM<ActionLog, ActionLogSearcher>
    {

        protected override IEnumerable<IGridColumn<ActionLog>> InitGridHeader()
        {
            var header = new List<GridColumn<ActionLog>>();

            header.Add(this.MakeGridHeader(x => x.LogType));
            header.Add(this.MakeGridHeader(x => x.ModuleName));
            header.Add(this.MakeGridHeader(x => x.ActionName));
            header.Add(this.MakeGridHeader(x => x.ITCode));
            header.Add(this.MakeGridHeader(x => x.ActionUrl));
            header.Add(this.MakeGridHeader(x => x.ActionTime));
            header.Add(this.MakeGridHeader(x => x.Duration));
            header.Add(this.MakeGridHeader(x => x.IP));
            header.Add(this.MakeGridHeader(x => x.Remark));

            return header;
        }
        
        public override IOrderedQueryable<ActionLog> GetSearchQuery()
        {
            var query = DC.Set<ActionLog>()
                .CheckContain(Searcher.ITCode, x=>x.ITCode)
                .CheckContain(Searcher.ActionUrl, x=>x.ActionUrl)
                .CheckContain(Searcher.LogType, x=>x.LogType)
                .CheckContain(Searcher.IP, x=>x.IP)
                .CheckBetween(Searcher.ActionTime?.GetStartTime(), Searcher.ActionTime?.GetEndTime(), x=>x.ActionTime, includeMax:false)
                .CheckWhere(Searcher.Duration,x=>x.Duration >= Searcher.Duration)
                .Select(x=>new ActionLog()
                {
                    ID          = x.ID,
                    ModuleName  = x.ModuleName,
                    ITCode      = x.ITCode,
                    ActionTime  = x.ActionTime,
                    ActionName  = x.ActionName,
                    ActionUrl   = x.ActionUrl,
                    Duration    = x.Duration,
                    IP          = x.IP,
                    LogType     = x.LogType,
                    Remark      = x.Remark,
                    CreateBy    = x.CreateBy,
                    CreateTime  = x.CreateTime,
                    UpdateBy    = x.UpdateBy,
                    UpdateTime  = x.UpdateTime
                })
                .OrderByDescending(x=>x.ActionTime);

            return query;
        }
    }
}
