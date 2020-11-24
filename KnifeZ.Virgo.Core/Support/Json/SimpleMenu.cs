using System;

namespace KnifeZ.Virgo.Core.Support.Json
{
    [Serializable]
    public class SimpleMenu
    {
        public Guid Id { get; set; }
        public bool? IsInherit { get; set; }

        public Guid? ActionId { get; set; }

        public bool? IsPublic { get; set; }

        public string Url { get; set; }

        public Guid? ParentId { get; set; }
    }
}
