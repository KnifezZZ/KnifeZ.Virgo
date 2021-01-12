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

    [Display(Name = "local")]
    public class VirgoLocalFileHandler : VirgoFileHandlerBase
    {
        private static string _modeName = "local";

        public VirgoLocalFileHandler (Configs config, IDataContext dc) : base(config, dc)
        {
        }

        public override Stream GetFileData (IVirgoFile file)
        {
            return File.OpenRead(Path.GetFullPath(_config.HostRoot+ file.Path));
        }


        public override IVirgoFile Upload (string fileName, long fileLength, Stream data, string group = null, string subdir = null, string extra = null, string fileContentType = null)
        {
            FileAttachment file = new FileAttachment();
            file.FileName = fileName;
            file.Length = fileLength;
            file.UploadTime = DateTime.Now;
            file.SaveMode = _modeName;
            file.ExtraInfo = extra;
            var ext = string.Empty;
            if (string.IsNullOrEmpty(fileName) == false)
            {
                var dotPos = fileName.LastIndexOf('.');
                ext = fileName.Substring(dotPos + 1);
            }
            file.FileExt = ext;

            var localSettings = _config.FileUploadOptions.Settings.Where(x => x.Key.ToLower() == "local").Select(x => x.Value).FirstOrDefault();

            var groupdir = "";
            if (string.IsNullOrEmpty(group))
            {
                groupdir = localSettings?.FirstOrDefault().GroupLocation;
            }
            else
            {
                groupdir = localSettings?.Where(x => x.GroupName.ToLower() == group.ToLower()).FirstOrDefault().GroupLocation;
            }
            if (string.IsNullOrEmpty(groupdir))
            {
                groupdir = "./uploads";
            }
            string pathHeader = groupdir;
            if (pathHeader.StartsWith("."))
            {
                pathHeader = Path.Combine(_config.HostRoot, pathHeader);
            }
            if (string.IsNullOrEmpty(subdir) == false)
            {
                pathHeader = Path.Combine(pathHeader, subdir);
            }
            else
            {
                var sub = VirgoFileProvider._subDirFunc?.Invoke(this);
                if (string.IsNullOrEmpty(sub) == false)
                {
                    pathHeader = Path.Combine(pathHeader, sub);
                }
            }
            if (!Directory.Exists(pathHeader))
            {
                Directory.CreateDirectory(pathHeader);
            }

            var fullPath = Path.Combine(pathHeader, $"{Guid.NewGuid().ToString("N")}.{file.FileExt}");
            file.Path ="/"+Path.GetRelativePath(_config.HostRoot,Path.GetFullPath(fullPath)).Replace("\\","/");
            using (var fileStream = File.Create(fullPath))
            {
                data.CopyTo(fileStream);
            }
            _dc.AddEntity(file);
            _dc.SaveChanges();
            return file;
        }

        public override void DeleteFile (IVirgoFile file)
        {
            if (string.IsNullOrEmpty(file?.Path) == false)
            {
                try
                {
                    File.Delete(file?.Path);
                }
                catch { }
            }
        }
    }

}
