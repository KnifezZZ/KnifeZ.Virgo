using System;
using System.Collections.Generic;
using System.Text;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Attributes;

namespace WebDemo.Model
{
    [MiddleTable]
    public class BlogClassificationMiddle : BasePoco
    {
        public Blog Blog { get; set; }
        public BlogClassification BlogClassification { get; set; }
        public Guid BlogId { get; set; }
        public Guid BlogClassificationId { get; set; }
    }
}
