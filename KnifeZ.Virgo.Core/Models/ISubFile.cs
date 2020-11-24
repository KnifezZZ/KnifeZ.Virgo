using System;
using System.Collections.Generic;
using System.Text;

namespace KnifeZ.Virgo.Core
{
    public interface ISubFile
    {
        Guid FileId { get; set; }
        FileAttachment File { get; set; }
        int order { get; set; }
    }
}
