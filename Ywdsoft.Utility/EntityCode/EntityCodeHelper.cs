/*
 * 模块名: 实体生成器
 * 描述: 实体生成器
 * 作者: 杜冬军
 * 创建日期: 2016/2/23 8:50:22 
 * 博客地址：http://yanweidie.cnblogs.com
 */
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Ywdsoft.Utility.EntityCode
{
    /// <summary>
    /// 实体代码生成工具
    /// </summary>
    public class EntityCodeHelper
    {
        /// <summary>
        /// 生成指定实体文件
        /// </summary>
        /// <param name="TableInfo">表信息集合</param>
        /// <param name="s">文件输出流</param>
        public static void QuickCode(IEnumerable<dynamic> TableInfo, Stream os)
        {
            var group = TableInfo.GroupBy(e => e.TableName);
            string templatePath = FileHelper.GetAbsolutePath("/Template/Entity.vm");

            using (ZipOutputStream s = new ZipOutputStream(os))
            {
                foreach (var columnList in group)
                {
                    var newColumnList = columnList.OrderBy(e => e.FieldSequence).ToList();

                    string tableName = newColumnList[0].TableName;
                    string tableNameDesc = newColumnList[0].TableNameDesc;
                    string entityName = tableName.Substring(tableName.IndexOf("_") + 1).Replace("_", "");
                    Hashtable htArgs = new Hashtable();
                    htArgs["ColumnList"] = newColumnList;
                    htArgs["TableName"] = tableName;
                    htArgs["EntityName"] = entityName;
                    htArgs["TableNameDesc"] = tableNameDesc;
                    htArgs["ConvertType"] = new ConvertType();
                    var codeText = FileGen.GetFileText(templatePath, htArgs).ToString();
                    byte[] m_buffer = Encoding.UTF8.GetBytes(codeText);
                    ZipEntry entry = new ZipEntry(entityName + ".cs");
                    entry.DateTime = DateTime.Now;
                    s.PutNextEntry(entry);
                    s.Write(m_buffer, 0, m_buffer.Count());
                }
            }
        }
    }

    public class ConvertType
    {
        public string GetDotNetType(string sqlTypeString)
        {
            //匹配枚举，防止SQL注入
            SqlDbType sqlType = (SqlDbType)Enum.Parse(typeof(SqlDbType), sqlTypeString, true);
            switch (sqlType)
            {
                case SqlDbType.VarBinary:
                case SqlDbType.Variant:
                case SqlDbType.Xml:
                case SqlDbType.Udt://自定义的数据类型
                case SqlDbType.Timestamp:
                case SqlDbType.Image:
                case SqlDbType.Binary:
                    return "Object";

                case SqlDbType.Bit:
                    return "Boolean";

                case SqlDbType.Text:
                case SqlDbType.Char:
                case SqlDbType.VarChar:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                    return "string";

                case SqlDbType.SmallDateTime:
                case SqlDbType.DateTime:
                    return "DateTime";

                case SqlDbType.SmallMoney:
                case SqlDbType.Money:
                case SqlDbType.Decimal:
                    return "decimal";

                case SqlDbType.Float:
                    return "double";

                case SqlDbType.SmallInt:
                    return "Int16";
                case SqlDbType.Int:
                    return "int";
                case SqlDbType.BigInt:
                    return "Int64";

                case SqlDbType.TinyInt:
                    return "byte";

                case SqlDbType.Real:
                    return "Single";

                case SqlDbType.UniqueIdentifier:
                    return "Guid";
            }
            return "string";
        }
    }
}
