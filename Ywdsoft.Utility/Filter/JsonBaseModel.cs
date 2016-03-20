using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ywdsoft.Utility
{
    /// <summary>
    /// 服务器端接口返回值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonBaseModel<T>
    {
        /// <summary>
        /// 执行是否成功：true false
        /// </summary>
        /// <remarks></remarks>
        public bool HasError { get; set; }

        /// <summary>
        /// 数据总条数
        /// </summary>
        public int? TotalCount { get; set; }

        /// <summary>
        /// 数据总页数
        /// </summary>
        public int? TotalPage { get; set; }

        /// <summary>
        /// 执行返回消息
        /// </summary>
        /// <remarks></remarks>
        public string Message { get; set; }

        /// <summary>
        /// 数据实体
        /// </summary>
        /// <remarks></remarks>
        public T Result { get; set; }


        public JsonBaseModel()
        {
            Result = default(T);
            HasError = false;
        }

        /// <summary>
        /// 根据每页显示数与总记录数计算出总页数
        /// </summary>
        /// <param name="rows">每页显示数</param>
        /// <param name="totalRecord">结果总记录数</param>
        /// <param name="isPagination">是否分页 如果分页则进行计算 不分页则返回null</param>
        /// <returns></returns>
        public int? CalculateTotalPage(int rows, int totalRecord, bool isPagination)
        {
            if (isPagination && rows > 0)
            {
                return Convert.ToInt32(Math.Ceiling((double)totalRecord / (double)rows));
            }
            else
            {
                return null;
            }
        }
    }
}
