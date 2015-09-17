using System;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;

namespace Mysoft.Utility
{
    /// <summary> 
    /// SqlServer数据访问接口实现
    /// </summary> 
    public class SQLHelper
    {
        private static string strConnect =SysConfig.SqlConnect;

        /// <summary>
        /// 执行命令,并返回影响函数
        /// </summary>
        /// <param name="strSQL">执行的SQL语句</param>
        /// <param name="param">参数</param>
        /// <returns>影响行数</returns>
        public static int ExecuteNonQuery(string strSQL, object param)
        {
            return DbHelper.ExecuteNonQuery(strConnect, strSQL, param);
        }

        /// <summary>
        /// 执行命令,并返回影响函数
        /// </summary>
        /// <param name="strSQL">执行的SQL语句</param>
        /// <returns>影响行数</returns>
        public static int ExecuteNonQuery(string strSQL)
        {
            return ExecuteNonQuery(strSQL,null);
        }

        /// <summary>
        /// 执行命令,返回第一行,第一列的值,并将结果转换为T类型
        /// </summary>
        /// <param name="strSQL">执行的SQL语句</param>
        /// <param name="param">参数</param>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <returns>结果集的第一行,第一列</returns>
        public static T ExecuteScalar<T>(string strSQL, object param)
        {
            return DbHelper.ExecuteScalar<T>(strConnect, strSQL, param);
        }

        /// <summary>
        /// 执行命令,返回第一行,第一列的值,并将结果转换为T类型
        /// </summary>
        /// <param name="strSQL">执行的SQL语句</param>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <returns>结果集的第一行,第一列</returns>
        public static T ExecuteScalar<T>(string strSQL)
        {
            return ExecuteScalar<T>(strSQL,null);
        }

        /// <summary>
        /// 执行命令,返回第一行,并将结果转换为T类型
        /// </summary>
        /// <param name="strSQL">执行的SQL语句</param>
        /// <param name="param">参数</param>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <returns>结果集的第一行</returns>
        public static T Single<T>(string strSQL, object param)
        {
            return DbHelper.Single<T>(strConnect, strSQL, param);
        }

        /// <summary>
        /// 执行命令,返回第一值,并将结果转换为T类型
        /// </summary>
        /// <param name="strSQL">执行的SQL语句</param>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <returns>结果集的第一行</returns>
        public static T Single<T>(string strSQL)
        {
            return Single<T>(strSQL, null);
        }

        /// <summary>
        /// 执行命令,并将结果转换为List<T>类型
        /// </summary>
        /// <param name="strSQL">执行的SQL语句</param>
        /// <param name="param">参数</param>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <returns>结果集</returns>
        public static List<T> ToList<T>(string strSQL, object param)
        {
            return DbHelper.ToList<T>(strConnect, strSQL, param);
        }

        /// <summary>
        /// 执行命令,并将结果转换为List<T>类型
        /// </summary>
        /// <param name="strSQL">执行的SQL语句</param>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <returns>结果集</returns>
        public static List<T> ToList<T>(string strSQL)
        {
            return ToList<T>(strSQL, null);
        }

        /// <summary>
        /// 执行查询,并将结果集填充到DataSet
        /// </summary>
        /// <param name="strSQL">执行的SQL语句</param>
        /// <param name="param">参数</param>
        /// <returns>数据集</returns>
        public static DataSet FillDataSet(string strSQL, object param)
        {
            return DbHelper.FillDataSet(strConnect, strSQL, param);
        }

        /// <summary>
        /// 执行查询,并将结果集填充到DataSet
        /// </summary>
        /// <param name="strSQL">执行的SQL语句</param>
        /// <returns>数据集</returns>
        public static DataSet FillDataSet(string strSQL)
        {
            return FillDataSet(strSQL, null);
        }

        /// <summary>
        /// 执行命令,并将结果集填充到DataTable
        /// </summary>
        /// <param name="strSQL">执行的SQL语句</param>
        /// <param name="param">参数</param>
        /// <returns>数据集</returns>
        public static DataTable FillDataTable(string strSQL, object param)
        {
            return DbHelper.FillDataTable(strConnect, strSQL, param);
        }

        /// <summary>
        /// 执行命令,并将结果集填充到DataTable
        /// </summary>
        /// <param name="strSQL">执行的SQL语句</param>
        /// <returns>数据集</returns>
        public static DataTable FillDataTable(string strSQL)
        {
            return FillDataTable(strSQL, null);
        }

        /// <summary>
        /// 执行存储过程,并将结果集填充到DataTable
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="param">参数</param>
        /// <returns>数据集</returns>
        public static DataTable RunProcedure(string procName, object param)
        {
            return DbHelper.RunProcedure(strConnect, procName, param);
        }

        /// <summary>
        /// 执行存储过程,并将结果集填充到DataTable
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <returns>数据集</returns>
        public static DataTable RunProcedure(string procName)
        {
            return RunProcedure(procName,null);
        }

        /// <summary>
        /// 执行存储过程,并将结果集填充到DataSet
        /// </summary>
        /// <param name="procName">执行的存储过程名称</param>
        /// <param name="param">参数</param>
        /// <returns>数据集</returns>
        public static DataSet RunProcedureToDataSet(string procName, object param, params string[] srcTable)
        {
            return DbHelper.RunProcedureToDataSet(strConnect, procName, param, srcTable);
        }

        /// <summary>
        /// 执行存储过程,并将结果集填充到DataSet
        /// </summary>
        /// <param name="procName">执行的存储过程名称</param>
        /// <returns>数据集</returns>
        public static DataSet RunProcedureToDataSet(string procName, params string[] srcTable)
        {
            return RunProcedureToDataSet(procName, null, srcTable);
        }

        /// <summary>
        /// 批量保存数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="list">list数据集合</param>
        /// <param name="srcTable">目标表名</param>
        public static void BatchSaveData<T>(List<T> list, string srcTable) where T : class
        {
            try
            {
                DataTable dataTable = list.ToDataTable<T>();
                BatchSaveData(dataTable, srcTable);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 批量保存数据
        /// </summary>
        /// <param name="dataTable">dataTable</param>
        /// <param name="srcTable">目标表名</param>
        public static void BatchSaveData(DataTable dataTable, string srcTable)
        {
            try
            {
                using (var bulk = new SqlBulkCopy(strConnect)
                {
                    DestinationTableName = srcTable,
                    BatchSize = 10000
                })
                {
                    //先取出表里面有的字段
                    DataTable dtSrc = FillDataTable("SELECT * FROM "+srcTable+" WHERE 1=2");
                    //取出数据库和实体里面都存在的列，为bulk添加映射
                    foreach (DataColumn item in dtSrc.Columns)
                    {
                        if (dataTable.Columns.Contains(item.ColumnName))
                        {
                            bulk.ColumnMappings.Add(item.ColumnName, item.ColumnName);
                        }
                    }
                    bulk.WriteToServer(dataTable);
                    bulk.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
