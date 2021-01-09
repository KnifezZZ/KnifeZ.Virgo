using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using KnifeZ.Virgo.Core.Models;

namespace KnifeZ.Virgo.Core.Support.FileHandlers
{
    public interface IVirgoFileHandler
    {
        IVirgoFile Upload (string fileName, long fileLength, Stream data, string group = null, string subdir = null, string extra = null, string fileContentType = null);
        Stream GetFileData(IVirgoFile file);

        void DeleteFile(IVirgoFile file);
    }
}
