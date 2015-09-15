using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Data.Common;

namespace Mysoft.Utility
{
    internal static class DbHelper
    {
        internal static int ExecuteNonQuery(string strCon, string strSQL, object param)
        {
            using (SqlConnection con = new SqlConnection(strCon))
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                SqlCommand cmd = GetSqlCommand(strSQL, param);
                cmd.Connection = con;
                return cmd.ExecuteNonQuery();
            }
        }


        internal static DataTable FillDataTable(string strCon, string strSQL, object param)
        {
            using (SqlConnection con = new SqlConnection(strCon))
            {
                SqlCommand cmd = GetSqlCommand(strSQL, param);
                cmd.Connection = con;
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                SqlDataReader reader = cmd.ExecuteReader();
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
            using (SqlConnection con = new SqlConnection(strCon))
            {
                SqlCommand cmd = GetSqlCommand(procName, param);
                cmd.Connection = con;
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
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
            using (SqlConnection con = new SqlConnection(strCon))
            {
                SqlCommand cmd = GetSqlCommand(procName, param);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                DataSet ds = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
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
            using (SqlConnection con = new SqlConnection(strCon))
            {
                SqlCommand cmd = GetSqlCommand(strSQL, param);
                cmd.Connection = con;
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                DataSet ds = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
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
            using (SqlConnection con = new SqlConnection(strCon))
            {
                SqlCommand cmd = GetSqlCommand(strSQL, param);
                cmd.Connection = con;
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                return ConvertScalar<T>(cmd.ExecuteScalar());
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
            using (SqlConnection con = new SqlConnection(strCon))
            {
                SqlCommand cmd = GetSqlCommand(strSQL, param);
                cmd.Connection = con;
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                SqlDataReader dr = cmd.ExecuteReader();
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
            using (SqlConnection con = new SqlConnection(strCon))
            {
                SqlCommand cmd = GetSqlCommand(strSQL, param);
                cmd.Connection = con;
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                SqlDataReader dr = cmd.ExecuteReader();
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
        /// 读取SqlDataReader一行数据转换成T
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dr">SqlDataReader</param>
        /// <returns>T</returns>
        private static T GetSingleT<T>(SqlDataReader dr)
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
                    if (!ptype.IsEnum)
                    {
                        pInfo.FastSetValue(obj, dr[i]);
                    }
                    else
                    {
                        pInfo.FastSetValue(obj, Enum.ToObject(ptype, dr[i]));
                    }
                }
            }
            return obj;
        }

        public static SqlCommand GetSqlCommand(string strSQL, object argsObject)
        {
            SqlCommand cmd = new SqlCommand();
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
                            SqlParameter parameter = value as SqlParameter;
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
