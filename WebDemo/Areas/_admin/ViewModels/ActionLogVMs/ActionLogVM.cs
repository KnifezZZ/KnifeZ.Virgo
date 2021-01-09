using KnifeZ.Virgo.Core;

namespace KnifeZ.Virgo.Mvc.Admin.ViewModels.ActionLogVMs
{
    public class ActionLogVM : BaseCRUDVM<ActionLog>
    {
        public ActionLogListVM ListVm { get; set; }
    }
}
