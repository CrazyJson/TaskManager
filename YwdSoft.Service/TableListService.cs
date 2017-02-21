using System.Collections.Generic;
using Ywdsoft.Utility.DB;
using Ywdsoft.Utility.Extensions.Xml;

namespace YwdSoft.Service
{
    /// <summary>
    /// Sql Server表服务
    /// </summary>
    public class TableListService
    {
        /// <summary>
        /// 获取数据库所有表
        /// </summary>
        /// <returns>所有表</returns>
        public static IEnumerable<dynamic> GetAllTable()
        {
            return DapperHelper.Query(XmlCommandManager.GetCommand("Table:GetAllTable").CommandText);
        }

        /// <summary>
        /// 获取数据库表列相关信息
        /// </summary>
        /// <param name="tableNameList">表名集合</param>
        /// <returns>每张表列信息</returns>
        public static IEnumerable<dynamic> GetTableInfo(string[] tableNameList)
        {
            string strSQL = XmlCommandManager.GetCommand("Table:GetAllTableInfo").CommandText;
            strSQL = string.Format(strSQL, string.Join("','", tableNameList));
            return DapperHelper.Query(strSQL);
        }
    }
}
