﻿using KnifeZ.Extensions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KnifeZ.Virgo.Core
{
    /// <summary>
    /// EncHash
    /// </summary>
    [Table("EncHashs")]
    public class EncHash : BasePoco
    {
        public Guid Key { get; set; }
        public int Hash { get; set; }
    }
}
