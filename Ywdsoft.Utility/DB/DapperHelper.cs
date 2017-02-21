using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Ywdsoft.Utility.DB
{
    /// <summary>
    /// Dapper再次封装
    /// </summary>
    /// <remarks>
    /// 更加详细说明请参考链接：https://github.com/ericdc1/Dapper.SimpleCRUD
    /// https://github.com/StackExchange/dapper-dot-net/blob/master/Dapper.Tests/Tests.cs
    /// </remarks>
    public class DapperHelper
    {
        /// <summary>
        /// 获取SQLSever数据库连接对象
        /// </summary>
        /// <param name="conStr">连接字符串默认为空(使用配置文件中连接字符串)</param>
        /// <returns>SQLSever数据库连接对象</returns>
        public static SqlConnection GetConnection(string conStr = "")
        {
            if (string.IsNullOrEmpty(conStr))
            {
                conStr = SysConfig.SqlServerConnect;
            }
            SqlConnection con = new SqlConnection(conStr);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            return con;
        }

        /// <summary>
        /// 通过Id获取指定数据
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="id">数据Id</param>
        /// <returns>指定数据</returns>
        /// <example>
        /// SQL Select Id, Name, Age from [User] where Id = 1 
        /// </example>
        public static T Get<T>(object id) where T : new()
        {
            using (SqlConnection con = GetConnection())
            {
                return con.Get<T>(id);
            }
        }

        /// <summary>
        /// 获取第一列数据
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="sql">SQL</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static T ExecuteScalar<T>(string sql, object param = null)
        {
            using (SqlConnection con = GetConnection())
            {
                return con.ExecuteScalar<T>(sql, param);
            }
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="param">参数</param>
        /// <returns>影响条数</returns>
        public static int Execute(string sql, object param = null)
        {
            using (SqlConnection con = GetConnection())
            {
                return con.Execute(sql, param);
            }
        }

        /// <summary>
        /// 查询第一条数据 返回动态类
        /// </summary>
        /// <param name="sql">查询sql</param>
        /// <param name="param">查询参数</param>
        /// <returns>第一条数据</returns>
        public static dynamic QueryFirst<dynamic>(string sql, object param = null)
        {
            using (SqlConnection con = GetConnection())
            {
                return con.QueryFirst(sql, param);
            }
        }

        /// <summary>
        /// 查询所有符合条件的数据
        /// </summary>
        /// <param name="sql">查询sql</param>
        /// <param name="param">查询参数</param>
        /// <returns>所有符合条件的数据</returns>
        public static IEnumerable<dynamic> Query(string sql, object param = null)
        {
            using (SqlConnection con = GetConnection())
            {
                return con.Query(sql, param);
            }
        }

        /// <summary>
        /// 查询所有的数据
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <returns>所有的数据</returns>
        /// <example>
        /// SQL  Select * from [User]
        /// </example>
        public static IEnumerable<T> GetList<T>()
        {
            using (SqlConnection con = GetConnection())
            {
                return con.GetList<T>();
            }
        }

        /// <summary>
        /// 查询符合条件的数据
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="parameters">参数</param>
        /// <returns>符合条件的数据</returns>
        /// <example>
        /// var user = connection.GetList<User>(new { Age = 10 });  
        /// SQL  Select * from [User] where Age = @Age
        /// </example>
        public static IEnumerable<T> GetList<T>(object parameters = null)
        {
            using (SqlConnection con = GetConnection())
            {
                return con.GetList<T>(parameters);
            }
        }

        /// <summary>
        /// 查询符合条件的数据
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="conditions">查询条件</param>
        /// <param name="parameters">参数</param>
        /// <returns>符合条件的数据</returns>
        /// <example>
        /// var encodeForLike = term => term.Replace("[", "[[]").Replace("%", "[%]");
        /// string likename = "%" + encodeForLike("Smith") + "%";
        /// var user = connection.GetList<User>("where age = @Age or Name like @Name", new { Age = 10, Name = likename });
        /// SQL  Select * from [User] where age = 10 or Name like '%Smith%'
        /// </example>
        public static IEnumerable<T> GetList<T>(string conditions, object parameters = null)
        {
            using (SqlConnection con = GetConnection())
            {
                return con.GetList<T>(conditions, parameters);
            }
        }

        /// <summary>
        /// 获取分页查询数据
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="pageNumber">页码  从1开始</param>
        /// <param name="rowsPerPage">每页条数</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="parameters">参数</param>
        /// <returns>分页结果</returns>
        /// <example>
        /// var user = connection.GetListPaged<User>(1,10,"where age = 10 or Name like '%Smith%'","Name desc");
        /// SQL:SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY Name desc) AS PagedNumber, Id, Name, Age FROM [User] where age = 10 or Name like '%Smith%') AS u WHERE PagedNUMBER BETWEEN ((1 - 1) * 10 + 1) AND (1 * 10)
        /// </example>
        public static IEnumerable<T> GetListPaged<T>(int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null)
        {
            using (SqlConnection con = GetConnection())
            {
                return con.GetListPaged<T>(pageNumber, rowsPerPage, conditions, orderby, parameters);
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="entityToUpdate">实体</param>
        /// <returns>影响条数</returns>
        public static int Update(object entityToUpdate)
        {
            using (SqlConnection con = GetConnection())
            {
                return con.Update(entityToUpdate);
            }
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="entityToInsert">待保存实体</param>
        /// <returns>返回主键</returns>
        /// <remarks>
        /// 默认的表名，将类的名称匹配的表的属性重写此
        /// 默认的主键将ID的关键属性重写此
        /// 默认情况下，插入语句将包含在类的所有属性编辑（假），只读（真正的），和IgnoreInsert的属性删除项目从INSERT语句
        /// 性能用只读（真）仅用于选择
        /// 复杂类型是不包括在插入语句，这使列插入即使没有可编辑的属性。您可以包括复杂的类型，如果你用编辑（真的）。这个枚举是有用的。
        /// </remarks>
        /// <example>
        /// var newId = connection.Insert(new User { FirstName = "User", LastName = "Person",  Age = 10 });  
        /// Insert into [Users] (FirstName, LastName, Age) VALUES (@FirstName, @LastName, @Age)
        /// </example>
        public static int? Insert(object entityToInsert)
        {
            using (SqlConnection con = GetConnection())
            {
                return con.Insert(entityToInsert);
            }
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">主键的类型</typeparam>
        /// <param name="entityToInsert">待保存实体</param>
        /// <returns>返回主键</returns>
        /// <remarks>
        /// 默认的表名，将类的名称匹配的表的属性重写此
        /// 默认的主键将ID的关键属性重写此
        /// 默认情况下，插入语句将包含在类的所有属性编辑（假），只读（真正的），和IgnoreInsert的属性删除项目从INSERT语句
        /// 性能用只读（真）仅用于选择
        /// 复杂类型是不包括在插入语句，这使列插入即使没有可编辑的属性。您可以包括复杂的类型，如果你用编辑（真的）。这个枚举是有用的。
        /// </remarks>
        /// <example>
        /// var newId = connection.Insert(new User { FirstName = "User", LastName = "Person",  Age = 10 });  
        /// Insert into [Users] (FirstName, LastName, Age) VALUES (@FirstName, @LastName, @Age)
        /// </example>
        public static TKey Insert<TKey>(object entityToInsert)
        {
            using (SqlConnection con = GetConnection())
            {
                return con.Insert<TKey>(entityToInsert);
            }
        }

        /// <summary>
        /// 根据主键删除数据
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="Id">主键</param>
        /// <returns>影响条数</returns>
        /// <example>
        /// Delete From [User] Where ID = @ID
        /// </example>
        public static int Delete<T>(object Id)
        {
            using (SqlConnection con = GetConnection())
            {
                return con.Delete<T>(Id);
            }
        }


        /// <summary>
        /// 根据主键删除数据
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="entityToDelete">实体数据</param>
        /// <returns>影响条数</returns>
        /// <example>
        /// Delete From [User] Where ID = @ID
        /// </example>
        public static int Delete<T>(T entityToDelete)
        {
            using (SqlConnection con = GetConnection())
            {
                return con.Delete<T>(entityToDelete);
            }
        }

        /// <summary>
        /// 根据条件删除多条数据
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="whereConditions">条件  new { Age = 10 }  或者 Where age = 20</param>
        /// <returns>影响条数</returns>
        /// <example>
        /// connection.DeleteList<User>(new { Age = 10 });
        /// connection.DeleteList<User>("Where age > 20");
        /// connection.DeleteList<User>("Where age > @Age", new {Age = 20});
        /// delete from user where age=10
        /// </example>
        public static int DeleteList<T>(object whereConditions)
        {
            using (SqlConnection con = GetConnection())
            {
                return con.DeleteList<T>(whereConditions);
            }
        }

        /// <summary>
        /// 统计符合条件的数量
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="conditions">条件</param>
        /// <param name="parameters">参数</param>
        /// <returns>符合条件的数量</returns>
        /// <example>
        /// var count = connection.RecordCount<User>("Where age > 20");
        /// var count = connection.RecordCount<User>("Where age > @Age", new {Age = 20});
        /// </example>
        public static int RecordCount<T>(string conditions = "", object parameters = null)
        {
            using (SqlConnection con = GetConnection())
            {
                return con.RecordCount<T>(conditions, parameters);
            }
        }
    }
}
