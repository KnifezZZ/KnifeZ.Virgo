using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;

using WebDemo.Model;


namespace WebDemo.ViewModel.BlogVMs
{
    public partial class BlogSearcher : BaseSearcher
    {
        [Display(Name = "标题")]
        public String Title { get; set; }
        [Display(Name = "状态")]
        public StatusEnum? Status { get; set; }

        protected override void InitVM()
        {
        }

    }
}

