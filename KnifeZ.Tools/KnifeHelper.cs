using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace KnifeZ.Tools
{
    public class KnifeHelper
    {
        #region 字符串加密
        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">明文（待加密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesEncrypt (string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            rm.Dispose();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">明文（待解密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesDecrypt (string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;

            Byte[] toEncryptArray = Convert.FromBase64String(str.Replace(" ", "+"));

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            rm.Dispose();
            return Encoding.UTF8.GetString(resultArray);
        }
        /// <summary>
        /// MD5 64位加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string MD5Encrypt64 (string text)
        {
            MD5 md5 = MD5.Create();
            //实例化一个md5对像           
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(text));
            return Convert.ToBase64String(s);
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Encrypt32 (string password)
        {
            string cl = password;
            string pwd = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
                                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd += s[i].ToString("X2");
            }
            md5.Dispose();
            return pwd;
        }

        /// <summary>
        /// C# DES加密方法
        /// </summary>
        /// <param name="encryptedValue">要加密的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <returns>加密后的字符串</returns>
        public static string DESEncrypt (string originalValue, string key, string iv)
        {
            using (DESCryptoServiceProvider sa
                = new DESCryptoServiceProvider { Key = Encoding.UTF8.GetBytes(key), IV = Encoding.UTF8.GetBytes(iv) })
            {
                using (ICryptoTransform ct = sa.CreateEncryptor())
                {
                    byte[] by = Encoding.UTF8.GetBytes(originalValue);
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, ct,
                                                         CryptoStreamMode.Write))
                        {
                            cs.Write(by, 0, by.Length);
                            cs.FlushFinalBlock();
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// C# DES解密方法
        /// </summary>
        /// <param name="encryptedValue">待解密的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <returns>解密后的字符串</returns>
        public static string DESDecrypt (string encryptedValue, string key, string iv)
        {
            using (DESCryptoServiceProvider sa =
                new DESCryptoServiceProvider
                { Key = Encoding.UTF8.GetBytes(key), IV = Encoding.UTF8.GetBytes(iv) })
            {
                using (ICryptoTransform ct = sa.CreateDecryptor())
                {
                    byte[] byt = Convert.FromBase64String(encryptedValue);

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write))
                        {
                            cs.Write(byt, 0, byt.Length);
                            cs.FlushFinalBlock();
                        }
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }
        #endregion

        #region Markdown转换
        /// <summary>
        /// Markdown转Html
        /// </summary>
        /// <param name="markdown"></param>
        /// <returns></returns>
        public static string ConvertMarkdownToHtml (string markdown)
        {
            return Markdig.Markdown.ToHtml(markdown);
        }
        /// <summary>
        /// Markdown文件转Html
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ConvertMarkdownFileToHtml (string path)
        {
            var markdown = Helpers.FileHelper.ReadFile(path);
            return ConvertMarkdownToHtml(markdown);
        }
        #endregion


        /// <summary>
        /// 相对路径转绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ConvertToAbsolutePath (string path)
        {
            //绝对路径
            if (path.Contains(":\\"))
            {
                return path;
            }
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }
        /// <summary>
        /// 绝对路径转相对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ConvertToRelativePath (string path)
        {
            if (path.StartsWith(@"/"))
            {
                return path;
            }
            return path.Replace(AppDomain.CurrentDomain.BaseDirectory, "").Replace(@"\", @"/");
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadFile (string path)
        {
            return Helpers.FileHelper.ReadFile(path);
        }

        /// <summary>
        /// 正则移除所有HTML标签
        /// </summary>
        /// <param name="html"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RemoveHtmlTags (string html, int length = 0)
        {
            string strText = Regex.Replace(html, "<[^>]+>", "");
            strText = Regex.Replace(strText, "&[^;]+;", "");
            if (length > 0 && strText.Length > length)
                return strText.Substring(0, length);

            return strText;
        }

    }
}
