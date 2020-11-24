using System;
using KnifeZ.Virgo.Core;


namespace KnifeZ.Virgo.Mvc.Admin.ViewModels.ActionLogVMs
{
    public class ActionLogBatchVM : BaseBatchVM<ActionLog, ActionLog_BatchEdit>
    {
        public ActionLogBatchVM()
        {
            ListVM = new ActionLogListVM();
            LinkedVM = new ActionLog_BatchEdit();
        }

    }

	/// <summary>
    /// 批量编辑字段类
    /// </summary>
    public class ActionLog_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}
