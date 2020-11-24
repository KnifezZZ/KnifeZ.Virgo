using System;
using System.ComponentModel.DataAnnotations;
using KnifeZ.Virgo.Core;

namespace KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkUserVms
{
    public class FrameworkUserBatchVM : BaseBatchVM<FrameworkUserBase, FrameworkUser_BatchEdit>
    {
        public FrameworkUserBatchVM()
        {
            ListVM = new FrameworkUserListVM();
            LinkedVM = new FrameworkUser_BatchEdit();
        }

    }

	/// <summary>
    /// 批量编辑字段类
    /// </summary>
    public class FrameworkUser_BatchEdit : BaseVM
    {

    }

}
