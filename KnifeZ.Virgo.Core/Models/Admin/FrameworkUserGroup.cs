using System;
using KnifeZ.Virgo.Core.Attributes;
using KnifeZ.Extensions;

namespace KnifeZ.Virgo.Core
{
    [MiddleTable]
    public class FrameworkUserGroup : BasePoco
    {
        public FrameworkUserBase User { get; set; }
        public FrameworkGroup Group { get; set; }
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
    }

}
