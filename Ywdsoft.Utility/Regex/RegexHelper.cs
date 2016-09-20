using System.Text.RegularExpressions;

namespace Ywdsoft.Utility
{
    public class RegexHelper
    {
        /// <summary>
        /// 是否手机号码
        /// </summary>
        /// <param name="input">要判断的手机号码</param>
        /// <returns></returns>
        public static bool IsMobile(string input)
        {
            if (!Regex.IsMatch(input, @"^[1][1-9]\d{9}$", RegexOptions.IgnoreCase))
                return false;
            if (input.Length == 11 && (input.StartsWith("13") || input.StartsWith("14") || input.StartsWith("15") || input.StartsWith("18")))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否e-mail
        /// </summary>
        /// <param name="input">要判断的字符串</param>
        /// <returns></returns>
        public static bool IsEmail(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                string EmailReg = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";

                return Regex.IsMatch(input, EmailReg, RegexOptions.IgnoreCase);
            }
            return false;
        }

        /// <summary>
        /// 是否中文汉字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsChinese(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            Regex rx = new Regex("^[\u4e00-\u9fa5]$");
            for (int i = 0; i < input.Length; i++)
            {
                if (!rx.IsMatch(input[i].ToString()))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 是否IP地址
        /// </summary>
        /// <param name="str1">待判断的IP地址</param>
        /// <returns></returns>
        public static bool IsIPAddress(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length < 7 || input.Length > 15)
                return false;

            Regex regex = new Regex(@"^([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])$", RegexOptions.IgnoreCase);
            return regex.IsMatch(input);
        }
    }
}
