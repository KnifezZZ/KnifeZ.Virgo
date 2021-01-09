using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnifeZ.Virgo.Core.Models
{

    public interface IVirgoFile
    {
        string Path { get; set; }

        string FileName { get; set; }
        string FileExt { get; set; }
        long Length { get; set; }

        DateTime UploadTime { get; set; }

        string SaveMode { get; set; }
        string ExtraInfo { get; set; }
        string Url { get; set; }

        Stream DataStream { get; set; }

        string GetID ();

    }
}
