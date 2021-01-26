using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using KnifeZ.Virgo.Core.Extensions;
using KnifeZ.Virgo.Core.Models;

namespace KnifeZ.Virgo.Core.Support.FileHandlers
{
    [Display(Name = "database")]
    public class VirgoDataBaseFileHandler : VirgoFileHandlerBase
    {
        public VirgoDataBaseFileHandler(Configs config, IDataContext dc) : base(config, dc)
        {
        }

        public override Stream GetFileData(IVirgoFile file)
        {
            var rv = _dc.Set<FileAttachment>().CheckID(file.GetID()).FirstOrDefault();
            if (rv != null)
            {
                return new MemoryStream((rv as FileAttachment).FileData);
            }
            return null;
        }


        public override IVirgoFile Upload(string fileName, long fileLength, Stream data, string groupName = null, string subdir = null, string extra = null, string fileContentType = null)
        {
            FileAttachment file = new FileAttachment
            {
                FileName = fileName,
                Length = fileLength,
                UploadTime = DateTime.Now,
                SaveMode = SaveFileModeEnum.Database.ToString(),
                ExtraInfo = extra
            };
            var ext = string.Empty;
            if (string.IsNullOrEmpty(fileName) == false)
            {
                var dotPos = fileName.LastIndexOf('.');
                ext = fileName.Substring(dotPos + 1);
            }
            file.FileExt = ext;
            using (var dataStream = new MemoryStream())
            {
                data.CopyTo(dataStream);
                file.FileData = dataStream.ToArray();
            }
                _dc.AddEntity(file);
                _dc.SaveChanges();
            return file;
        }
    }
}
