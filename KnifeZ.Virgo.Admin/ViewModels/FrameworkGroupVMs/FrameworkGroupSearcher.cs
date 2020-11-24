using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KnifeZ.Virgo.Core;

namespace KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkGroupVMs
{
    public class FrameworkGroupSearcher : BaseSearcher
    {
        [Display(Name = "GroupCode")]
        public string GroupCode { get; set; }

        [Display(Name = "GroupName")]
        public string GroupName { get; set; }
    }
}
