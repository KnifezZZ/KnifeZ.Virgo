using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KnifeZ.Virgo.Core;

using WebDemo.Model;


namespace WebDemo.ViewModel.BlogCategoryVMs
{
    public partial class BlogCategoryVM : BaseCRUDVM<BlogCategory>
    {

        public BlogCategoryVM()
        {
            SetInclude(x => x.Parent);
        }

        protected override void InitVM()
        {
        }

        public override void DoAdd()
        {           
            base.DoAdd();
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            base.DoEdit(updateAllFields);
        }

        public override void DoDelete()
        {
            base.DoDelete();
        }
    }
}

