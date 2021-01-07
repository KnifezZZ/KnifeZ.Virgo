using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;

using WebDemo.Model;


namespace WebDemo.ViewModel.BlogCategoryVMs
{
    public partial class BlogCategorySearcher : BaseSearcher
    {
        [Display(Name = "类别名称")]
        public String Name { get; set; }
        [Display(Name = "静态地址")]
        public String Url { get; set; }

        protected override void InitVM()
        {
        }

    }
}

