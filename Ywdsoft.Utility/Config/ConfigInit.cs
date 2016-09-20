using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Ywdsoft.Utility
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
        private static readonly string ConfigPath = FileHelper.GetAbsolutePath("Config/Config.config");

        /// <summary>
        /// 本地配置文件地址
        /// </summary>
        private static readonly string ConfigLocalPath = FileHelper.GetAbsolutePath("Config/Config_Local.config");


        private static BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.SetProperty;

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
                PropertyInfo[] Props = type.GetProperties(flags);
                PathMapAttribute PathMap = null;
                foreach (var prop in Props)
                {
                    PathMap = GetMyAttribute<PathMapAttribute>(prop, false);
                    if (PathMap != null)
                    {
                        prop.SetValue(null, Convert.ChangeType(GetConfigValue(PathMap), prop.PropertyType), null);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region "私有方法"
        /// <summary>
        /// 通过键读取配置文件信息
        /// </summary>
        /// <param name="PathMap">自定义属性信息</param>
        /// <returns>值</returns>
        private static string GetConfigValue(PathMapAttribute PathMap)
        {
            if (!File.Exists(ConfigPath))
            {
                return "";
            }
            string path = GetXmlPath(PathMap.Key, PathMap.XmlPath);
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
                            throw new Exception("本地配置文件设置异常,节点" + PathMap.Key + "没有相应的value属性,请检查！");
                        }
                        return GetRealValue(attr.Value, PathMap.IsDecrypt);
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
                        throw new Exception("服务器配置文件设置异常,节点" + PathMap.Key + "没有相应的value属性,请检查！");
                    }
                    return GetRealValue(attr.Value, PathMap.IsDecrypt);
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

        /// <summary>
        /// 获取配置文件真实值
        /// </summary>
        /// <param name="value">原始值</param>
        /// <param name="IsDecrypt">是否需要解密</param>
        /// <returns>真实值</returns>
        private static string GetRealValue(string value, bool IsDecrypt)
        {
            if (IsDecrypt)
            {
                return DESEncrypt.Decrypt(value);
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// 返回MemberInfo对象指定类型的Attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        private static T GetMyAttribute<T>(MemberInfo m, bool inherit) where T : Attribute
        {
            T[] array = m.GetCustomAttributes(typeof(T), inherit) as T[];

            if (array.Length == 1)
                return array[0];

            if (array.Length > 1)
                throw new InvalidProgramException(string.Format("方法 {0} 不能同时指定多次 [{1}]。", m.Name, typeof(T)));

            return default(T);
        }

        private static T ConvertScalar<T>(object obj)
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

        #endregion
    }


    /// <summary>
    /// 配置文件标注
    /// </summary>
    public class PathMapAttribute : Attribute
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key;

        /// <summary>
        /// xmlPath路径前缀
        /// </summary>
        public string XmlPath = @"/configuration/add";

        /// <summary>
        /// 是否需要对该值进行DES解密
        /// </summary>
        public bool IsDecrypt = false;
    }
}
