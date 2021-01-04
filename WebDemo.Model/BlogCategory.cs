﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
using KnifeZ.Virgo.Core;

namespace WebDemo.Model
{
    public class BlogCategory : BasePoco, ITreeData<BlogCategory>
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

        #region ITreeData成员

        [Display(Name = "所属类别")]
        public Guid? ParentId { get; set; }
        [Display(Name = "ParentFolder")]
        [JsonIgnore]
        public BlogCategory Parent { get; set; }
        [Display(Name = "Children")]
        [JsonIgnore]
        public List<BlogCategory> Children { get; set; }

        #endregion
    }
}
