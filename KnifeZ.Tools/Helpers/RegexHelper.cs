using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace KnifeZ.Tools.Helpers
{
    public class RegexHelper
    {
        #region 公共正则表达式
        private static readonly Regex regImg_Alt = new Regex(@"<img\b[^<>]*?\balt[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
        private static readonly Regex regImg_Src = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
        private static readonly Regex regImg_SrcArry = new Regex(@"<IMG[^>]+src=\s*(?:'(?<src>[^']+)'|""(?<src>[^""]+)""|(?<src>[^>\s]+))\s*[^>]*>", RegexOptions.IgnoreCase);
        private static readonly Regex regImg_Arry = new Regex(@"<IMG[^>]*>", RegexOptions.IgnoreCase);
        #endregion

        #region 正则匹配图片

        /// <summary>
        /// 获取内容中的img标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string MatchImg(string content)
        {
            string reg = "<img[^<>]*?\\ssrc=['\"]?(.*?)['\"].*?>";
            Regex regImg = new Regex(reg, RegexOptions.IgnoreCase);
            var str = regImg.Match(content).Value;
            return str;
        }
        /// <summary>
        /// 匹配内容所有img标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string[] MatchAllImgs(string content)
        {
            var matches = regImg_Arry.Matches(content);
            var i = 0;
            var sUrlList = new string[matches.Count];
            foreach (Match match in matches) { sUrlList[i++] = match.Value; }
            return sUrlList;
        }


        /// <summary>
        /// 匹配内容中所有Img标签 src
        /// </summary>
        /// <param name="sHtmlText"></param>
        /// <returns></returns>
        public static string[] MatchImgSrcArry(string sHtmlText)
        {
            var matches = regImg_SrcArry.Matches(sHtmlText);
            var i = 0;
            var sUrlList = new string[matches.Count];
            foreach (Match match in matches) { sUrlList[i++] = match.Groups["src"].Value; }
            return sUrlList;
        }
        /// <summary>
        /// 匹配img标签的图片地址
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string MatchImgSrc(string content)
        {
            var str = regImg_Src.Match(content).Groups["imgUrl"].Value;
            return str;
        }
        /// <summary>
        /// 匹配img标签的alt内容
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string MatchImgAlt(string content)
        {
            var str = regImg_Alt.Match(content).Groups["imgUrl"].Value;
            return str;
        }
        #endregion

        #region 正则处理内容
        /// <summary>
        /// 匹配正文p标签
        /// </summary>
        /// <param name="sHtmlText"></param>
        /// <returns></returns>
        public static string[] MatchPlist(string sHtmlText)
        {
            var regImg = new Regex(@"<p.*?>[\s\S]*?<\/p>", RegexOptions.IgnoreCase);
            var matches = regImg.Matches(sHtmlText);
            var i = 0;
            var sUrlList = new string[matches.Count];
            foreach (Match match in matches) { sUrlList[i++] = match.Value; }
            return sUrlList;
        }

        #endregion

        /// <summary>
        /// 根据关键字分割字符串
        /// </summary>
        /// <param name="input">内容</param>
        /// <param name="pattern">分隔字符串</param>
        /// <returns></returns>
        public static string[] SplitString(string input, string pattern)
        {
            return Regex.Split(input,pattern, RegexOptions.IgnoreCase);
        }
    }
}
