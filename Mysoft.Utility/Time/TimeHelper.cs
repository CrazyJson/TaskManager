using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mysoft.Utility
{
    public static class TimeHelper
    {
        /// <summary>
        /// 计算两个日期相隔的天数和小时数
        /// </summary>
        /// <param name="dt1">开始日期</param>
        /// <param name="dt2">截止日期</param>
        /// <returns>相隔的天数和小时数</returns>
        public static string GetDayAndHours(this DateTime dt1,DateTime dt2)
        {
            double jg = (dt1 - dt2).TotalDays;
            //取相隔天数
            double iDay = Math.Floor(jg);
            //取相隔小时数
            double iHour = Math.Ceiling((jg - iDay) * 24);
            return string.Format("{0}天{1}小时", iDay, iHour);
        }
    }
}
