using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KnifeZ.Virgo.Core
{
    public abstract class TreePoco : TopBasePoco
    {
        [Display(Name = "Parent")]
        public Guid? ParentId { get; set; }

    }

    public abstract class TreeBasePoco : BasePoco
    {
        [Display(Name = "Parent")]
        public Guid? ParentId { get; set; }

    }

    public class TreePoco<T> : TreePoco where T : TreePoco<T>
    {

        [Display(Name = "ParentFolder")]
        [JsonIgnore]
        public T Parent { get; set; }

        [JsonIgnore]
        public List<T> Children { get; set; }
    }

}
