using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using KnifeZ.Virgo.Core;
namespace WebDemo.Model
{
    public class BlogClassification : BasePoco
    {
        [Display(Name = "类别名称")]
        public string Name { get; set; }
        [Display(Name = "所属用户")]
        public Guid FrameworkUserId { get; set; }
        [Display(Name = "所属用户")]
        public FrameworkUserBase FrameworkUser { get; set; }
        [Display(Name = "个人分类")]
        public List<BlogClassificationMiddle> BlogClassificationMiddle { get; set; }
    }
}
