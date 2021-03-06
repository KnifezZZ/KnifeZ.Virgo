using System;

namespace KnifeZ.Virgo.Core.Support.Json
{
    [Serializable]
    public class SimpleFunctionPri
    {
        public Guid ID { get; set; }
        public Guid? RoleId { get; set; }

        public Guid? UserId { get; set; }

        public Guid MenuItemId { get; set; }

        public string Url { get; set; }

        public bool? Allowed { get; set; }

    }
}
