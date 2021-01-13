using Microsoft.Extensions.Caching.Distributed;
using NPOI.HSSF.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using KnifeZ.Virgo.Core.Extensions;
using KnifeZ.Virgo.Core.Support;
using KnifeZ.Virgo.Core.Support.Json;

namespace KnifeZ.Virgo.Core
{
    public class Utils
    {

        public static string GetCurrentComma()
        {
            if (CultureInfo.CurrentUICulture.Name == "zh-cn")
            {
                return "：";
            }
            else
            {
                return ":";
            }
        }

        public static List<Assembly> GetAllAssembly()
        {
            var rv = new List<Assembly>();
            var path = Assembly.GetEntryAssembly().Location;
            var dir = new DirectoryInfo(Path.GetDirectoryName(path));

            var dlls = dir.GetFiles("*.dll", SearchOption.AllDirectories);
            string[] systemdll = new string[]
            {
                "KnifeZ.Virgo",
                Assembly.GetEntryAssembly().GetName().Name
            };
            foreach (var dll in dlls)
            {
                try
                {
                    if (systemdll.Any(x => dll.Name.StartsWith(x)) == true)
                    {
                        rv.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(dll.FullName));
                    }
                }
                catch { }
            }
            return rv;
        }

        public static SimpleMenu FindMenu (string url, List<SimpleMenu> menus)
        {
            if (url == null)
            {
                return null;
            }
            url = url.ToLower();
            if (menus == null)
            {
                return null;
            }
            //寻找菜单中是否有与当前判断的url完全相同的
            var menu = menus.Where(x => x.Url != null && x.Url.ToLower() == url).FirstOrDefault();

            //如果没有，抹掉当前url的参数，用不带参数的url比对
            if (menu == null)
            {
                var pos = url.IndexOf("?");
                if (pos > 0)
                {
                    url = url.Substring(0, pos);
                    menu = menus.Where(x => x.Url != null && (x.Url.ToLower() == url || x.Url.ToLower() + "async" == url)).FirstOrDefault();
                }
            }

            //如果还没找到，则判断url是否为/controller/action/id这种格式，如果是则抹掉/id之后再对比
            if (menu == null && url.EndsWith("/index"))
            {
                url = url.Substring(0, url.Length - 6);
                menu = menus.Where(x => x.Url != null && x.Url.ToLower() == url).FirstOrDefault();
            }
            if (menu == null && url.EndsWith("/indexasync"))
            {
                url = url.Substring(0, url.Length - 11);
                menu = menus.Where(x => x.Url != null && x.Url.ToLower() == url).FirstOrDefault();
            }
            return menu;
        }

        public static string GetIdByName(string fieldName)
        {
            return fieldName == null ? "" : fieldName.Replace(".", "_").Replace("[", "_").Replace("]", "_");
        }

        public static void CheckDifference<T>(IEnumerable<T> oldList, IEnumerable<T> newList, out IEnumerable<T> ToRemove, out IEnumerable<T> ToAdd) where T : TopBasePoco
        {
            List<T> tempToRemove = new List<T>();
            List<T> tempToAdd = new List<T>();
            oldList ??= new List<T>();
            newList ??= new List<T>();
            foreach (var oldItem in oldList)
            {
                bool exist = false;
                foreach (var newItem in newList)
                {
                    if (oldItem.GetID().ToString() == newItem.GetID().ToString())
                    {
                        exist = true;
                        break;
                    }
                }
                if (exist == false)
                {
                    tempToRemove.Add(oldItem);
                }
            }
            foreach (var newItem in newList)
            {
                bool exist = false;
                foreach (var oldItem in oldList)
                {
                    if (newItem.GetID().ToString() == oldItem.GetID().ToString())
                    {
                        exist = true;
                        break;
                    }
                }
                if (exist == false)
                {
                    tempToAdd.Add(newItem);
                }
            }
            ToRemove = tempToRemove.AsEnumerable();
            ToAdd = tempToAdd.AsEnumerable();
        }

        public static short GetExcelColor(string color)
        {
            var colors = typeof(HSSFColor).GetNestedTypes().ToList();
            foreach (var col in colors)
            {
                var pro = col.GetField("hexString");
                if (pro == null)
                {
                    continue;
                }
                var hex = pro.GetValue(null);
                var rgb = hex.ToString().Split(':');
                for (int i = 0; i < rgb.Length; i++)
                {
                    if (rgb[i].Length > 2)
                    {
                        rgb[i] = rgb[i].Substring(0, 2);
                    }
                }
                int r = Convert.ToInt16(rgb[0], 16);
                int g = Convert.ToInt16(rgb[1], 16);
                int b = Convert.ToInt16(rgb[2], 16);

                if (color.Length == 8)
                {
                    color = color[2..];
                }
                string c1 = color.Substring(0, 2);
                string c2 = color.Substring(2, 2);
                string c3 = color.Substring(4, 2);

                int r1 = Convert.ToInt16(c1, 16);
                int g1 = Convert.ToInt16(c2, 16);
                int b1 = Convert.ToInt16(c3, 16);


                if (r == r1 && g == g1 && b == b1)
                {
                    return (short)col.GetField("index").GetValue(null);
                }
            }
            return HSSFColor.COLOR_NORMAL;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ZipAndBase64Encode(string input)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(input);
            MemoryStream inputms = new MemoryStream(buffer);
            MemoryStream outputms = new MemoryStream();
            using (GZipStream zip = new GZipStream(outputms, CompressionMode.Compress))
            {
                inputms.CopyTo(zip);
            }
            byte[] rv = outputms.ToArray();
            inputms.Dispose();
            return Convert.ToBase64String(rv);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string UnZipAndBase64Decode(string input)
        {
            byte[] inputstr = Convert.FromBase64String(input);
            MemoryStream inputms = new MemoryStream(inputstr);
            MemoryStream outputms = new MemoryStream();
            using (GZipStream zip = new GZipStream(inputms, CompressionMode.Decompress))
            {
                zip.CopyTo(outputms);
            }
            byte[] rv = outputms.ToArray();
            outputms.Dispose();
            return Encoding.UTF8.GetString(rv);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string EncodeScriptJson(string input)
        {
            if (input == null)
            {
                return "";
            }
            else
            {
                return input.Replace(Environment.NewLine, "").Replace("\"", "\\\\\\\"").Replace("'", "\\'");
            }
        }

        #region 读取txt文件
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path">文件路径绝对</param>
        /// <returns></returns>
        public static string ReadTxt(string path)
        {
            string result = string.Empty;

            if (File.Exists(path))
            {
                using (Stream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (TextReader sr = new StreamReader(fs, UnicodeEncoding.UTF8))
                    {
                        result = sr.ReadToEnd();
                    }
                }
            }

            return result;
        }
        #endregion

        public static string GetCS(string cs, string mode, Configs config)
        {
            if (string.IsNullOrEmpty(cs) || config.DBconfigs.Any(x=>x.Key.ToLower() == cs.ToLower()) == false)
            {
                cs = "default";
            }
            int index = cs.LastIndexOf("_");
            if (index > 0)
            {
                cs = cs.Substring(0, index);
            }
            if (mode?.ToLower() == "read")
            {
                var reads = config.DBconfigs.Where(x => x.Key.StartsWith(cs + "_")).Select(x=>x.Key).ToList();
                if (reads.Count > 0)
                {
                    Random r = new Random();
                    var v = r.Next(0, reads.Count);
                    cs = reads[v];
                }
            }
            return cs;
        }


        #region MD5加密
        /// <summary>
        /// 字符串MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns>返回大写32位MD5值</returns>
        public static string GetMD5String(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);

            return MD5String(buffer);
        }

        /// <summary>
        /// 流MD5加密
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetMD5Stream(Stream stream)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return MD5String(buffer);
        }

        /// <summary>
        /// 文件MD5加密
        /// </summary>
        /// <param name="path"></param>
        /// <returns>返回大写32位MD5值</returns>
        public static string GetMD5File(string path)
        {
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    return GetMD5Stream(fs);
                }
            }
            else
            {
                return string.Empty;
            }
        }

        private static string MD5String(byte[] buffer)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] cryptBuffer = md5.ComputeHash(buffer);
            StringBuilder sb = new StringBuilder();
            foreach (byte item in cryptBuffer)
            {
                sb.Append(item.ToString("X2"));
            }
            return sb.ToString();
        }
        #endregion

        //public static string GetNugetVersion(string start = null, bool pre = false)
        //{
        //    var Cache = GlobalServices.GetRequiredService<IDistributedCache>() as IDistributedCache;
        //    if (Cache.TryGetValue("nugetversion", out NugetInfo rv) == false || rv == null)
        //    {
        //        NugetInfo v = APIHelper.CallAPI<NugetInfo>($"https://api-v2v3search-0.nuget.org/query?q=KnifeZ.Virgo.Mvc&prerelease={pre.ToString().ToLower()}").Result;
        //        var data = v;
        //            Cache.Add("nugetversion", data, new DistributedCacheEntryOptions()
        //            {
        //                SlidingExpiration = new TimeSpan(0, 0, 36000)
        //            });
        //        rv = data;
        //    }

        //    if (string.IsNullOrEmpty(start))
        //    {
        //        return rv.data[0]?.version;
        //    }
        //    else
        //    {
        //        return rv.data[0].versions.Select(x => x.version).Where(x => x.StartsWith(start)).Last();
        //    }
        //}

        public static string ToFirstLower (string text)
        {
            if (text.Length > 1)
            {
                return text.First().ToString().ToLower() + text.Substring(1);
            }
            return text.ToLower();
        }
        public static string ToFirstUper (string text)
        {
            if (text.Length > 1)
            {
                return text.First().ToString().ToUpper() + text.Substring(1);
            }
            return text.ToLower();
        }
    }
}
