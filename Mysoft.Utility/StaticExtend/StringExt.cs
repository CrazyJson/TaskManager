using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mysoft.Utility
{
    public static class StringExt
    {
        /// <summary>
        /// 格式化超过指定长度的字符串,显示截取后字符串加...
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="displayLength">能显示的字节长度</param>
        /// <returns></returns>
        public static string FormatStringLength(this string str, int displayLength)
        {
            string result = string.Empty;// 最终返回的结果
            int byteLen = System.Text.Encoding.Default.GetByteCount(str);// 单字节字符长度
            int charLen = str.Length;// 把字符平等对待时的字符串长度
            int byteCount = 0;// 记录读取进度
            int pos = 0;// 记录截取位置
            if (byteLen > displayLength)
            {
                for (int i = 0; i < charLen; i++)
                {
                    if (Convert.ToInt32(str.ToCharArray()[i]) > 255)// 按中文字符计算加2
                        byteCount += 2;
                    else// 按英文字符计算加1
                        byteCount += 1;
                    if (byteCount > displayLength)// 超出时只记下上一个有效位置
                    {
                        pos = i;
                        break;
                    }
                    else if (byteCount == displayLength)// 记下当前位置
                    {
                        pos = i + 1;
                        break;
                    }
                }

                if (pos >= 0)
                    result = str.Substring(0, pos);
            }
            else
                result = str;

            return result;

        }
    }
}
