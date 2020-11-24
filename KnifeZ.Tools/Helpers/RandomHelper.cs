using System;
using System.Collections.Generic;
using System.Text;

namespace KnifeZ.Tools.Helpers
{
    public class RandomHelper
    {
        /// <summary>
        /// 随机结果
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool GetRandResult(int f = 50)
        {
            return new Random().Next(0, 100) > f;
        }
        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int GetRandNum(int start = 0, int end = 10)
        {
            if (start > end) return 0;

            return new Random().Next(start, end);
        }
        /// <summary>
        /// 获取指定范围的随机日期
        /// </summary>
        /// <param name="day">减去的最大天数</param>
        /// <returns>间隔日期之间的 随机日期</returns>
        public static DateTime GetRandomTime(int day, int x = 8, int y = 18)
        {
            Random random = new Random();
            var h = random.Next(x, y);
            var m = random.Next(0, 60);
            var s = random.Next(0, 60);
            //先把时间转换成年月日的字符串格式 在转换成时间（把时间格式转换成日期格式）
            DateTime dateTime = DateTime.Now;
            var add = dateTime.AddDays(-random.Next(0, day));
            //减去的天数加上随机生成的时间得到我们自己需要的时间
            var newDateTime = Convert.ToDateTime(add.ToString("yyyy-MM-dd 00:00:00")) + TimeSpan.Parse(h + ":" + m + ":" + s);
            //返回需要的时间
            return newDateTime;
        }
    }
}
