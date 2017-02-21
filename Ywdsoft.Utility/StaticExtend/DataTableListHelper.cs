using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace Ywdsoft.Utility
{
    /// <summary>
    /// DataTable和 List集合相互转换
    /// </summary>
    public static class DataTableListHelper
    {
        /// <summary>
        /// DataTable 转换成泛型List
        /// </summary>
        /// <typeparam name="T">实体对象类</typeparam>
        /// <param name="table">数据DatatTable</param>
        /// <returns> List<实体对象></returns>
        public static List<T> ToList<T>(this DataTable table)
        {
            if (table == null)
            {
                return null;
            }
            List<T> list = new List<T>();
            T t = default(T);
            PropertyInfo[] propertypes = null;
            string tempName = string.Empty;
            foreach (DataRow row in table.Rows)
            {
                t = Activator.CreateInstance<T>();
                propertypes = t.GetType().GetProperties();
                foreach (PropertyInfo pro in propertypes)
                {
                    if (!pro.CanWrite)
                    {
                        continue;
                    }
                    tempName = pro.Name;
                    if (table.Columns.Contains(tempName.ToUpper()))
                    {
                        object value = row[tempName];
                        if (value is System.DBNull)
                        {
                            value = null;
                            if (pro.PropertyType.FullName == "System.String")
                            {
                                value = string.Empty;
                            }
                        }
                        if (pro.PropertyType.IsGenericType && pro.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && value != null)
                        {
                            pro.SetValue(t, Convert.ChangeType(value, Nullable.GetUnderlyingType(pro.PropertyType)), null);
                        }
                        else if (pro.PropertyType.IsEnum)
                        {
                            pro.SetValue(t, Convert.ChangeType(value, Enum.GetUnderlyingType(pro.PropertyType)), null);
                        }
                        else
                        {
                            pro.SetValue(t, value, null);
                        }
                    }
                }
                list.Add(t);
            }
            return list;
        }

        /// <summary>
        /// 将List转换成DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dt = new DataTable();
            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor property = properties[i];
                Type colType = property.PropertyType;
                if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    colType = colType.GetGenericArguments()[0];
                }
                dt.Columns.Add(property.Name, colType);
            }
            object[] values = new object[properties.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = properties[i].GetValue(item);
                }
                dt.Rows.Add(values);
            }
            return dt;
        }
    }
}
