using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Mysoft.Utility
{
    /// <summary>
    /// 配置文件信息初始化,为了解决团队开发中,每个人的config文件不一致,而需要修改app.config或者web.config
    /// 下次获取最新又覆盖了本地修改的相关配置问题，通过添加本地配置文件格式为app_Local.config开解决此问题
    /// </summary>
    public class ConfigInit
    {
        /// <summary>
        /// 服务器配置文件地址
        /// </summary>
        private static readonly string ConfigPath = FileHelper.GetAbsolutePath("Config/Config.xml");

        /// <summary>
        /// 本地配置文件地址
        /// </summary>
        private static readonly string ConfigLocalPath = FileHelper.GetAbsolutePath("Config/Config_Local.xml");

        private static XmlDocument doc = null;

        /// <summary>
        /// 初始化配置信息
        /// </summary>
        public static void InitConfig()
        {
            try
            {
                doc = new XmlDocument();
                if (!File.Exists(ConfigPath))
                {
                    return;
                }
                Type type = typeof(SysConfig);
                PropertyInfo[] Props = type.GetCanWritePropertyInfo();
                PathMap PathMap = null;
                foreach (var prop in Props)
                {
                    PathMap = prop.GetCustomAttribute<PathMap>();
                    if (PathMap != null)
                    {
                        prop.FastSetValue(null, GetConfigValue(PathMap.Key, PathMap.XmlPath));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 通过键读取配置文件信息
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="XmlPath">xmlpath路径前缀</param>
        /// <returns>值</returns>
        public static string GetConfigValue(string key, string XmlPath = @"/configuration/add")
        {
            if (!File.Exists(ConfigPath))
            {
                return "";
            }
            string path = GetXmlPath(key, XmlPath);
            XmlNode node = null;
            XmlAttribute attr = null;
            try
            {
                if (File.Exists(ConfigLocalPath))
                {
                    //读取本地配置文件
                    doc.Load(ConfigLocalPath);
                    node = doc.SelectSingleNode(path);
                    if (node != null)
                    {
                        attr = node.Attributes["value"];
                        if (attr == null)
                        {
                            throw new Exception("本地配置文件设置异常,节点" + key + "没有相应的value属性,请检查！");
                        }
                        return attr.Value;
                    }
                }
                //读取服务器配置文件信息
                doc.Load(ConfigPath);
                node = doc.SelectSingleNode(path);
                if (node != null)
                {
                    attr = node.Attributes["value"];
                    if (attr == null)
                    {
                        throw new Exception("服务器配置文件设置异常,节点" + key + "没有相应的value属性,请检查！");
                    }
                    return attr.Value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }

        /// <summary>
        /// 获取xmlpath全路径
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="XmlPath">xmlpath路径前缀</param>
        /// <returns>xmlpath全路径</returns>
        private static string GetXmlPath(string key, string XmlPath)
        {
            return string.Format("{0}[@key='{1}']", XmlPath, key);
        }
    }

    /// <summary>
    /// 系统的配置
    /// </summary>
    public class SysConfig
    {
        /// <summary>
        /// 数据库连接字符串信息
        /// </summary>
        [PathMap(Key = "SqlConnect")]
        public static string SqlConnect { get; set; }

        /// <summary>
        /// 邮件信息配置
        /// </summary>
        [PathMap(Key = "MailInfo")]
        public static string MailInfo { get; set; }
    }

    /// <summary>
    /// 配置文件标注
    /// </summary>
    public class PathMap : Attribute
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key;

        /// <summary>
        /// xmlPath路径前缀
        /// </summary>
        public string XmlPath = @"/configuration/add";
    }
}
