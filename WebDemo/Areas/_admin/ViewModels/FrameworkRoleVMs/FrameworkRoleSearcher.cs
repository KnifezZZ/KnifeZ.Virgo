using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KnifeZ.Virgo.Core;

namespace KnifeZ.Virgo.Mvc.Admin.ViewModels.FrameworkRoleVMs
{
    public class FrameworkRoleSearcher : BaseSearcher
    {
        [Display(Name = "RoleCode")]
        public string RoleCode { get; set; }

        [Display(Name = "RoleName")]
        public string RoleName { get; set; }
    }
}
