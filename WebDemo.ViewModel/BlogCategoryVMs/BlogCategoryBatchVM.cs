using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KnifeZ.Virgo.Core;
using WebDemo.Model;


namespace WebDemo.ViewModel.BlogCategoryVMs
{
    public partial class BlogCategoryBatchVM : BaseBatchVM<BlogCategory, BlogCategory_BatchEdit>
    {
        public BlogCategoryBatchVM()
        {
            ListVM = new BlogCategoryListVM();
            LinkedVM = new BlogCategory_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class BlogCategory_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}

