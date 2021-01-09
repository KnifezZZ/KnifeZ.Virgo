using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
using KnifeZ.Virgo.Core;

namespace WebDemo.Model
{
    public class BlogCategory : TreePoco<BlogCategory>
    {
        [Display(Name = "类别名称")]
        public string Name { get; set; }
        [Display(Name = "类别描述")]
        public string Description { get; set; }

        [Display(Name = "图标")]
        public string Icon { get; set; }

        [Display(Name = "静态地址")]
        public string Url { get; set; }

        [Display(Name = "排序")]
        public int Sort { get; set; }
    }
}
