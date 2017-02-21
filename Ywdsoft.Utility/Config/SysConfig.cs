namespace Ywdsoft.Utility
{
    /// <summary>
    /// 缓存系统所有配置信息，以键值对形式存在
    /// </summary>
    /// <remarks>
    /// 系统相关配置信息都应该通过此类的静态属性读取
    /// </remarks>
    /// <example>
    /// 获取连接字符串 SysConfig.SqlConnect
    /// </example>
    /// <summary>
    /// 系统的配置
    /// </summary>
    public class SysConfig
    {
        /// <summary>
        /// Sqlite数据库连接信息
        /// </summary>
        [PathMap(Key = "SqliteConnect", AppDataDirectoryConvert = true)]
        public static string SqliteConnect { get; set; }

        /// <summary>
        /// SqlServer数据库连接信息
        /// </summary>
        [PathMap(Key = "SqlServerConnect")]
        public static string SqlServerConnect { get; set; }

        /// <summary>
        /// Sql语句存放目录信息
        /// </summary>
        [PathMap(Key = "YwdsoftExt:XmlCommandFolder")]
        public static string XmlCommandFolder { get; set; }
    }
}
