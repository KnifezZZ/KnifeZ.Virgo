using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
using KnifeZ.Virgo.Core;

namespace WebDemo.Model
{
    public enum StatusEnum
    {
        [Display(Name = "草稿")]
        Draft,
        [Display(Name = "已发布")]
        Published,
        [Display(Name = "归档")]
        Archive
    }

    [Table("Blog")]
    public class Blog : PersistPoco
    {
        [Display(Name = "标题")]
        [StringLength(100)]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string Title { get; set; }
        [Display(Name = "摘要")]
        [StringLength(200)]
        public string Description { get; set; }
        [Display(Name = "关键字")]
        [StringLength(200)]
        public string Keywords { get; set; }
        [Display(Name = "正文")]
        public string BodyText { get; set; }
        [Display(Name = "博客类别")]
        public Guid? BlogCategoryId { get; set; }
        [Display(Name = "博客类别")]
        public BlogCategory BlogCategory { get; set; }
        [Display(Name = "个人分类")]
        public List<BlogClassificationMiddle> BlogClassificationMiddle { get; set; }
        [Display(Name = "是否单页")]
        public bool IsSinglePage { get; set; }
        [Display(Name = "访问量")]
        public int VisitCount { get; set; }
        [Display(Name = "状态")]
        public StatusEnum Status { get; set; }
        [Display(Name = "静态地址")]
        public string Url { get; set; }
    }
}
