using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KnifeZ.Virgo.Core;
using WebDemo.Model;


namespace WebDemo.ViewModel.BlogVMs
{
    public partial class BlogBatchVM : BaseBatchVM<Blog, Blog_BatchEdit>
    {
        public BlogBatchVM()
        {
            ListVM = new BlogListVM();
            LinkedVM = new Blog_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class Blog_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}

