using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KnifeZ.Tools.Helpers
{
    public class KnifeFileModel
    {
        public int? ID { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public string FileSize { get; set; }
        public string FileLocalPath { get; set; }

        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }
    public static class FileHelper
    {
        #region 获取文件夹内文件
        /// <summary>
        /// 获取所有文件夹下文件
        /// </summary>
        /// <param name="path">相对路径,以|分割多个父级文件夹</param>
        /// <param name="searchParm">搜索词</param>
        /// <returns></returns>
        public static List<KnifeFileModel> GetAllFilesList (string paths, string searchParm)
        {
            var modelList = new List<KnifeFileModel>();
            if (paths.IndexOf("|") > -1)
            {
                var arrPath = paths.Split('|');
                foreach (var apath in arrPath)
                {
                    modelList.AddRange(GetFolderFiles(apath, searchParm));
                }
            }
            else
            {
                modelList.AddRange(GetFolderFiles(paths, searchParm));
            }
            return modelList;
        }
        /// <summary>
        /// 获取文件夹下所有文件
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="searchParm">搜索词</param>
        /// <param name="searchOption">包含子文件夹与否</param>
        /// <returns></returns>
        public static List<KnifeFileModel> GetFolderFiles (string path, string searchParm, SearchOption searchOption = SearchOption.AllDirectories)
        {
            path = KnifeHelper.ConvertToAbsolutePath(path);
            var modelList = new List<KnifeFileModel>();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            DirectoryInfo folder = new DirectoryInfo(path);
            var ret = folder.EnumerateFiles(searchParm, searchOption);

            var i = 0;
            foreach (var item in ret)
            {
                modelList.Add(new KnifeFileModel()
                {
                    ID = i++,
                    FileType = item.Extension,
                    FileName = item.Name,
                    FilePath = item.FullName,
                    FileLocalPath = item.DirectoryName,
                    FileSize = GetFileSize(item.Length),
                    CreatedTime = item.CreationTime,
                    UpdatedTime = item.LastWriteTime
                });
            }
            return modelList;
        }
        /// <summary>
        /// 获取文件夹列表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static List<KnifeFileModel> GetFolders (string path, string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories)
        {
            var curPath = KnifeHelper.ConvertToAbsolutePath(path);
            string[] directorieStrings = Directory.GetDirectories(curPath, searchPattern, searchOption);
            var relist = new List<KnifeFileModel>();
            var list = directorieStrings;
            IEnumerable<String> query = directorieStrings.OrderBy(x => x);
            return relist;
        }
        #endregion

        /// <summary>
        /// Http下载文件
        /// </summary>
        public static string HttpDownloadFile (string url, string path)
        {
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();
            //创建本地文件写入流
            Stream stream = new FileStream(path, FileMode.Create);
            byte[] bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            while (size > 0)
            {
                stream.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
            }
            stream.Close();
            responseStream.Close();
            return path;
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string GetFileSize (string path)
        {
            if (File.Exists(KnifeHelper.ConvertToAbsolutePath(path)))
            {
                FileInfo fileInfo = new FileInfo(KnifeHelper.ConvertToAbsolutePath(path));
                return GetFileSize(fileInfo.Length);
            }
            else
            {
                return "0KB";
            }
        }

        private static string GetFileSize(long size)
        {
            const double num = 1024.00; //byte
            if (size < num)
                return size + "B";
            if (size < Math.Pow(num, 2))
                return (size / num).ToString("f2") + "KB"; //kb
            if (size < Math.Pow(num, 3))
                return (size / Math.Pow(num, 2)).ToString("f2") + "MB"; //M
            if (size < Math.Pow(num, 4))
                return (size / Math.Pow(num, 3)).ToString("f2") + "GB"; //G

            return (size / Math.Pow(num, 4)).ToString("f2") + "TB"; //T
        }

        /// <summary>
        /// 读取本地文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadFile(string path)
        {
            using (FileStream fsRead = new FileStream(KnifeHelper.ConvertToAbsolutePath(path), FileMode.Open))
            {
                int fsLen = (int)fsRead.Length;
                byte[] heByte = new byte[fsLen];
                int r = fsRead.Read(heByte, 0, heByte.Length);
                string text = Encoding.UTF8.GetString(heByte);
                return text;
            }
        }

        /// <summary>
        /// 写入本地文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        public static void WriteFile(string content,string path)
        {
            path = KnifeHelper.ConvertToAbsolutePath(path);

            //判断文件夹是否存在，不存在则生成文件夹
            string floder = path.Substring(0, path.LastIndexOf("\\"));
            //如果不存在就创建file文件夹
            if (Directory.Exists(floder) == false)
            {
                Directory.CreateDirectory(floder);
            }
            //写入文件
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, bufferSize: 4096))
            {
                var buffer = Encoding.UTF8.GetBytes(content);
                fs.Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// 比较文件是否相同(哈希校验)
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="otherFilepath"></param>
        /// <returns></returns>
        public static bool CompareFile(string filePath, string otherFilepath = "")
        {
            if (!File.Exists(filePath))
            {
                return false;
            }
            var stream_1 = new FileStream(filePath, FileMode.Open);
            var stream_2 = new FileStream(otherFilepath, FileMode.Open);
            try
            {
                //比较两个哈希值
                if (stream_1.GetFileSha1() == stream_2.GetFileSha1())
                    return true;
                else
                    return false;
            }
            finally
            {
                stream_1.Close();
                stream_2.Close();

            }
        }

    }
}
