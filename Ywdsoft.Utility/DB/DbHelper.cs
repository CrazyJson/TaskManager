using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.Common;
using System.Data.SQLite;
using System.Data;
using Ywdsoft.Utility.DB;

namespace Ywdsoft.Utility
{
    internal static class DbHelper
    {


        internal static int ExecuteNonQuery(string strCon, string strSQL, object param)
        {
            using (SQLiteConnection con = new SQLiteConnection(strCon))
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                SQLiteCommand cmd = GetSQLiteCommand(strSQL, param);
                cmd.Connection = con;
                return cmd.ExecuteNonQuery();
            }
        }


        internal static DataTable FillDataTable(string strCon, string strSQL, object param)
        {
            using (SQLiteConnection con = new SQLiteConnection(strCon))
            {
                SQLiteCommand cmd = GetSQLiteCommand(strSQL, param);
                cmd.Connection = con;
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                SQLiteDataReader reader = cmd.ExecuteReader();
                DataSet ds = new DataSet();
                ds.EnforceConstraints = false;
                DataTable table = new DataTable("_tb");
                ds.Tables.Add(table);
                table.Load(reader);
                return table;
            }
        }

        internal static DataTable RunProcedure(string strCon, string procName, object param)
        {
            using (SQLiteConnection con = new SQLiteConnection(strCon))
            {
                SQLiteCommand cmd = GetSQLiteCommand(procName, param);
                cmd.Connection = con;
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.CommandType = CommandType.StoredProcedure;
                SQLiteDataReader reader = cmd.ExecuteReader();
                DataSet ds = new DataSet();
                ds.EnforceConstraints = false;
                DataTable table = new DataTable("_tb");
                ds.Tables.Add(table);
                table.Load(reader);
                return table;
            }
        }

        internal static DataSet RunProcedureToDataSet(string strCon, string procName, object param, params string[] srcTable)
        {
            using (SQLiteConnection con = new SQLiteConnection(strCon))
            {
                SQLiteCommand cmd = GetSQLiteCommand(procName, param);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                DataSet ds = new DataSet();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                adapter.Fill(ds);
                string tablename = "_tb";
                if (srcTable != null && srcTable.Length > 0)
                {
                    tablename = srcTable[0];
                }
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    ds.Tables[i].TableName = tablename + i.ToString();
                }
                return ds;
            }
        }

        internal static DataSet FillDataSet(string strCon, string strSQL, object param)
        {
            using (SQLiteConnection con = new SQLiteConnection(strCon))
            {
                SQLiteCommand cmd = GetSQLiteCommand(strSQL, param);
                cmd.Connection = con;
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                DataSet ds = new DataSet();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                adapter.Fill(ds);
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    ds.Tables[i].TableName = "_tb" + i.ToString();
                }
                return ds;
            }
        }

        internal static T ExecuteScalar<T>(string strCon, string strSQL, object param)
        {
            using (SQLiteConnection con = new SQLiteConnection(strCon))
            {
                SQLiteCommand cmd = GetSQLiteCommand(strSQL, param);
                cmd.Connection = con;
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                return ConvertScalar<T>(cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// 批量保存数据
        /// </summary>
        /// <param name="dataTable">dataTable</param>
        /// <param name="srcTable">目标表名</param>
        internal static void BatchSaveData(string strCon, DataTable dataTable, string srcTable)
        {
            using (SQLiteConnection con = new SQLiteConnection(strCon))
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                //先取出表里面有的字段
                DataTable dtSrc = FillDataTable(strCon, "SELECT * FROM " + srcTable + " WHERE 1=2", null);

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("INSERT INTO {0}(", srcTable);
                StringBuilder sbParam = new StringBuilder();
                sbParam.AppendFormat(" VALUES(");
                Dictionary<string, Type> dict = new Dictionary<string, Type>(dtSrc.Columns.Count);
                //取出数据库和实体里面都存在的列，为bulk添加映射
                foreach (DataColumn item in dtSrc.Columns)
                {
                    if (dataTable.Columns.Contains(item.ColumnName))
                    {
                        sb.AppendFormat("{0},", item.ColumnName);
                        sbParam.AppendFormat("@{0},", item.ColumnName);
                        dict[item.ColumnName] = item.DataType;
                    }
                }
                sb.Remove(sb.Length - 1, 1).Append(")");
                sbParam.Remove(sbParam.Length - 1, 1).Append(")");
                string strSQL = sb.ToString() + sbParam.ToString();
                object value = null;
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    foreach (var key in dict.Keys)
                    {
                        cmd.Parameters.AddWithValue(key, DBNull.Value);
                    }
                    using (var transaction = con.BeginTransaction())
                    {
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            cmd.CommandText = strSQL;
                            foreach (var key in dict.Keys)
                            {
                                value = dr[key];
                                if (value == null || value == DBNull.Value)
                                {
                                    cmd.Parameters[key].Value = DBNull.Value;
                                }
                                else
                                {
                                    cmd.Parameters[key].Value = value;
                                }
                                cmd.Parameters[key].ResetDbType();
                            }
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                }
            }
        }

        internal static T ConvertScalar<T>(object obj)
        {
            if (obj == null || DBNull.Value.Equals(obj))
                return default(T);

            if (obj is T)
                return (T)obj;

            Type targetType = typeof(T);

            if (targetType == typeof(object))
                return (T)obj;

            return (T)Convert.ChangeType(obj, targetType);
        }

        internal static T Single<T>(string strCon, string strSQL, object param)
        {
            using (SQLiteConnection con = new SQLiteConnection(strCon))
            {
                SQLiteCommand cmd = GetSQLiteCommand(strSQL, param);
                cmd.Connection = con;
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                SQLiteDataReader dr = cmd.ExecuteReader();
                T obj = default(T);
                // 循环读取结果集
                while (dr.Read())
                {
                    obj = GetSingleT<T>(dr);
                    break;
                }
                dr.Close();
                return obj;
            }
        }

        internal static List<T> ToList<T>(string strCon, string strSQL, object param)
        {
            using (SQLiteConnection con = new SQLiteConnection(strCon))
            {
                SQLiteCommand cmd = GetSQLiteCommand(strSQL, param);
                cmd.Connection = con;
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                SQLiteDataReader dr = cmd.ExecuteReader();
                List<T> list = new List<T>();
                T obj = default(T);
                // 循环读取结果集
                while (dr.Read())
                {
                    obj = GetSingleT<T>(dr);
                    list.Add(obj);
                }
                dr.Close();
                return list;
            }
        }

        /// <summary>
        /// 读取SQLiteDataReader一行数据转换成T
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dr">SQLiteDataReader</param>
        /// <returns>T</returns>
        private static T GetSingleT<T>(SQLiteDataReader dr)
        {
            Type type = typeof(T);
            T obj = (T)type.FastNew();
            PropertyInfo[] properties = type.GetCanWritePropertyInfo();
            for (int i = 0; i < dr.FieldCount; i++)
            {
                PropertyInfo pInfo = properties.FirstOrDefault(e => e.Name.Equals(dr.GetName(i)));
                if (pInfo != null)
                {
                    Type ptype = pInfo.PropertyType;
                    object val = dr.GetValue(i);
                    if (val != null && DBNull.Value.Equals(val) == false)
                    {
                        pInfo.FastSetValue(obj, val.Convert(ptype));
                    }
                }
            }
            return obj;
        }

        public static SQLiteCommand GetSQLiteCommand(string strSQL, object argsObject)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            if (argsObject != null)
            {
                //如果参数对象为Hashtable，则直接从里面获取参数
                if (argsObject is Hashtable)
                {
                    Hashtable ht = argsObject as Hashtable;
                    object obj = null;
                    foreach (string key in ht.Keys)
                    {
                        obj = ht[key];
                        if (obj == null || obj == DBNull.Value)
                        {
                            cmd.Parameters.AddWithValue(key, DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(key, obj);
                        }
                    }
                }
                else if (argsObject is DbParameter[])
                {
                    DbParameter[] parameters = argsObject as DbParameter[];
                    cmd.Parameters.AddRange(parameters);
                }
                else
                {
                    PropertyInfo[] properties = argsObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    foreach (PropertyInfo pInfo in properties)
                    {
                        object value = pInfo.FastGetValue(argsObject);
                        string name = "@" + pInfo.Name;

                        if (value == null || value == DBNull.Value)
                        {
                            cmd.Parameters.AddWithValue(name, DBNull.Value);
                        }
                        else if (value is ICollection)
                        {
                            StringBuilder sb = new StringBuilder(128);
                            sb.Append("(");
                            bool bFirst = true;
                            foreach (object obj in value as ICollection)
                            {
                                string tmpValue = null;
                                if (obj is string || obj is Guid)
                                {
                                    tmpValue = "N'" + obj.ToString().Replace("'", "''") + "'";
                                }
                                else if (obj is DateTime)
                                {
                                    throw new InvalidOperationException("不支持在IN条件中使用DateTime类型");
                                }
                                else if (obj is Guid)
                                {
                                    tmpValue = "'" + obj.ToString() + "'";
                                }
                                else
                                {
                                    tmpValue = obj.ToString();
                                }
                                if (bFirst)
                                {
                                    sb.Append(tmpValue);
                                    bFirst = false;
                                }
                                else
                                {
                                    sb.AppendFormat(",{0}", tmpValue);
                                }
                            }
                            if (sb.Length == 1)
                            {
                                sb.Append("NULL");
                            }
                            sb.Append(")");
                            string condation = sb.ToString();

                            strSQL.Replace(name, condation);
                        }
                        else
                        {
                            SQLiteParameter parameter = value as SQLiteParameter;
                            if (parameter != null)
                            {
                                cmd.Parameters.Add(parameter);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue(name, value);
                            }
                        }
                    }
                }
            }
            cmd.CommandText = strSQL;
            return cmd;
        }
    }
}
