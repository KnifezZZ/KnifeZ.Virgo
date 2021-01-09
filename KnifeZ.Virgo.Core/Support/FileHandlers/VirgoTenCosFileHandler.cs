using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COSXML;
using COSXML.Auth;
using COSXML.Model.Object;
using COSXML.Utils;
using KnifeZ.Virgo.Core.ConfigOptions;
using KnifeZ.Virgo.Core.Models;

namespace KnifeZ.Virgo.Core.Support.FileHandlers
{

    [Display(Name = "TenCos")]
    public class VirgoTenCosFileHandler: VirgoFileHandlerBase
    {
        private static string _modeName = "TenCos";

        public VirgoTenCosFileHandler (Configs config, IDataContext dc) : base(config, dc)
        {
        }

        //public override Stream GetFileData (IVirgoFile file)
        //{
        //    var ossSettings = _config.FileUploadOptions.Settings.Where(x => x.Key.ToLower() == "tenCos").Select(x => x.Value).FirstOrDefault();
        //    FileHandlerOptions groupInfo = null;
        //    if (string.IsNullOrEmpty(file.ExtraInfo))
        //    {
        //        groupInfo = ossSettings?.FirstOrDefault();
        //    }
        //    else
        //    {
        //        groupInfo = ossSettings?.Where(x => x.GroupName.ToLower() == file.ExtraInfo.ToLower()).FirstOrDefault();
        //        if (groupInfo == null)
        //        {
        //            groupInfo = ossSettings?.FirstOrDefault();
        //        }
        //    }
        //    if (groupInfo == null)
        //    {
        //        return null;
        //    }

        //    OssClient client = new OssClient(groupInfo.ServerUrl, groupInfo.Key, groupInfo.Secret);
        //    var rv = new MemoryStream();
        //    client.GetObject(new GetObjectRequest(groupInfo.GroupLocation, file.Path), rv);
        //    rv.Position = 0;
        //    return rv;

        //}


        public override IVirgoFile Upload (string fileName, long fileLength, Stream data, string group = null, string subdir = null, string extra = null, string fileContentType = null)
        {
            FileAttachment file = new FileAttachment
            {
                FileName = fileName,
                Length = fileLength,
                UploadTime = DateTime.Now,
                SaveMode = _modeName,
                ExtraInfo = extra
            };
            var ext = string.Empty;
            if (string.IsNullOrEmpty(fileName) == false)
            {
                var dotPos = fileName.LastIndexOf('.');
                ext = fileName.Substring(dotPos + 1);
            }
            file.FileExt = ext;

            var ossSettings = _config.FileUploadOptions.Settings.Where(x => x.Key.ToLower() == "tenCos").Select(x => x.Value).FirstOrDefault();
            FileHandlerOptions groupInfo = null;
            if (string.IsNullOrEmpty(group))
            {
                groupInfo = ossSettings?.FirstOrDefault();
            }
            else
            {
                groupInfo = ossSettings?.Where(x => x.GroupName.ToLower() == group.ToLower()).FirstOrDefault();
                if (groupInfo == null)
                {
                    groupInfo = ossSettings?.FirstOrDefault();
                }
            }
            if (groupInfo == null)
            {
                return null;
            }


            string pathHeader = "";
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
            var fullPath = Path.Combine(pathHeader, $"{Guid.NewGuid():N}.{file.FileExt}");
            fullPath = fullPath.Replace("\\", "/");

            BinaryReader binaryReader = new BinaryReader(data);
            var fs = binaryReader.ReadBytes((int)fileLength);


            CosXmlConfig config = new CosXmlConfig.Builder()
                .SetConnectionTimeoutMs(60000)  //设置连接超时时间，单位毫秒，默认45000ms
                .SetReadWriteTimeoutMs(40000)  //设置读写超时时间，单位毫秒，默认45000ms
                .IsHttps(false)  //设置默认 HTTPS 请求
                .SetAppid(groupInfo.AppId) //设置腾讯云账户的账户标识 APPID
                .SetRegion(groupInfo.GroupName)
                .Build();
            long durationSecond = 600;          //每次请求签名有效时长，单位为秒
            QCloudCredentialProvider qCloudCredentialProvider = new DefaultQCloudCredentialProvider(groupInfo.Secret,
              groupInfo.Key, durationSecond);
            CosXml cosXml = new CosXmlServer(config, qCloudCredentialProvider);
            string bucket = groupInfo.GroupLocation + "-" + groupInfo.AppId; //存储桶，格式：BucketName-APPID
            string key = groupInfo.GroupLocation + "-" + Guid.NewGuid().ToString("N") + "." + file.FileName.Split(".")[1]; //对象在存储桶中的位置，即称对象键
            PutObjectRequest request = new PutObjectRequest(bucket, key, fs);

            request.SetRequestHeader("Content-Type", fileContentType);
            //设置签名有效时长
            request.SetSign(TimeUtils.GetCurrentTime(TimeUnit.Seconds), 600);
            //设置进度回调
            //request.SetCosProgressCallback(delegate (long completed, long total)
            //{
            //    Console.WriteLine(String.Format("progress = {0:##.##}%", completed * 100.0 / total));
            //});
            //执行请求
            PutObjectResult result = cosXml.PutObject(request);

            if (result.httpCode == 200)
            {
                file.Path = "http://" + bucket + ".cos." + groupInfo.GroupName + ".myqcloud.com" + request.RequestPath;
                file.ExtraInfo = groupInfo.GroupName;
                _dc.AddEntity(file);
                _dc.SaveChanges();
                return file;
            }
            else
            {
                return null;
            }
        }

        //public override void DeleteFile (IVirgoFile file)
        //{
        //    var ossSettings = _config.FileUploadOptions.Settings.Where(x => x.Key.ToLower() == "oss").Select(x => x.Value).FirstOrDefault();
        //    FileHandlerOptions groupInfo = null;
        //    if (string.IsNullOrEmpty(file.ExtraInfo))
        //    {
        //        groupInfo = ossSettings?.FirstOrDefault();
        //    }
        //    else
        //    {
        //        groupInfo = ossSettings?.Where(x => x.GroupName.ToLower() == file.ExtraInfo.ToLower()).FirstOrDefault();
        //        if (groupInfo == null)
        //        {
        //            groupInfo = ossSettings?.FirstOrDefault();
        //        }
        //    }
        //    if (groupInfo == null)
        //    {
        //        return;
        //    }
        //    try
        //    {
        //        OssClient client = new OssClient(groupInfo.ServerUrl, groupInfo.Key, groupInfo.Secret);
        //        client.DeleteObject(groupInfo.GroupLocation, file.Path);
        //    }
        //    catch { }
        //    return;
        //}
    }

}
