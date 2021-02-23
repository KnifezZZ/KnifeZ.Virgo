using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace KnifeZ.Extensions
{
    public static class KnifeExtension
    {
        #region 字符串转换

        /// <summary>
        /// 字符串转时间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string value)
        {
            DateTime.TryParse(value, out var result);
            return result;
        }

        /// <summary>
        /// 字符串转Guid
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string s)
        {
            return Guid.Parse(s);
        }

        #endregion

        /// <summary>
        /// 根据正则替换
        /// </summary>
        /// <param name="input"></param>
        /// <param name="regex">正则表达式</param>
        /// <param name="replacement">新内容</param>
        /// <returns></returns>
        public static string Replace(this string input, Regex regex, string replacement)
        {
            return regex.Replace(input, replacement);
        }

        /// <summary>
        /// 按字段去重
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> hash = new HashSet<TKey>();
            return source.Where(p => hash.Add(keySelector(p)));
        }
        /// <summary>
        /// 去重
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector)
        {
            return source.Distinct(new CommonEqualityComparer<T, V>(keySelector));
        }
    }
}
