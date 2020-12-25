using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using KnifeZ.Virgo.Core;

namespace KnifeZ.Virgo.Mvc
{
    public static class IconFontsHelper
    {
        private static List<ComboSelectListItem> _iconFontItems;
        public static List<ComboSelectListItem> IconFontItems
        {
            get
            {
                foreach (var item in _iconFontItems.Where(x => x.Selected == true))
                    item.Selected = false;
                return _iconFontItems;
            }
        }

        private static Dictionary<string, List<MenuItem>> _iconFontDicItems;
        public static Dictionary<string, List<MenuItem>> IconFontDicItems => _iconFontDicItems;


        private static void RecursiveDir(string dirPath, HashSet<string> iconFonts, Dictionary<string, string[]> iconFontDic)
        {
            var dirs = Directory.GetDirectories(dirPath);
            foreach (var dir in dirs)
            {
                RecursiveDir(dir, iconFonts, iconFontDic);
            }

            var files = Directory.GetFiles(dirPath, "*.css");
            foreach (var cssPath in files)
            {
                ResolveIconfont(cssPath, iconFonts, iconFontDic);
            }
        }

        /// <summary>
        /// 解析
        /// </summary>
        private static void ResolveIconfont(string cssPath, HashSet<string> iconFonts, Dictionary<string, string[]> iconFontDic)
        {
            var file = File.ReadAllText(cssPath).Replace("\r\n", string.Empty);

            // 找到自定义的 iconfont
            var regex = new Regex("@font-face\\s{0,}{\\s{0,}('|\"|)font-family(\\1)\\s{0,}:\\s{0,}('|\"|)([a-zA-Z0-9-_.#]{1,})(\\3)\\s{0,};");
            var iconMatchs = regex.Matches(file);
            foreach (Match iconfontItem in iconMatchs)
            {
                // icon family name
                var iconName = iconfontItem.Groups[4].ToString();

                var itemRegex = new Regex($".({iconName}-([a-zA-Z0-9-_.#]{{1,}}))\\s{{0,}}:before\\s{{0,}}{{");
                var itemMatchs = itemRegex.Matches(file);

                var iconFontItems = new List<string>();
                foreach (Match item in itemMatchs)
                {
                    iconFontItems.Add(item.Groups[1].ToString());
                }

                if (iconFontItems.Count > 0)
                {
                    iconFonts.Add(iconName);
                    iconFontDic.Add(iconName, iconFontItems.OrderBy(x => x).ToArray());
                }
            }
        }
    }
}
