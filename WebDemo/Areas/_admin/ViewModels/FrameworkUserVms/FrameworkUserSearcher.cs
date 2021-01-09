using System;
using System.ComponentModel.DataAnnotations;
using KnifeZ.Virgo.Core;

namespace KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkUserVms
{
    public class FrameworkUserSearcher : BaseSearcher
    {
        [Display(Name = "Account")]
        public string ITCode { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "IsValid")]
        public bool? IsValid { get; set; }

    }
}
