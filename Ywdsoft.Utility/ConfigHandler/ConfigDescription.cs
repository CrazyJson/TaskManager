/*
 * Model: 系统参数配置接口
 * Desctiption: 描述
 * Author: 杜冬军
 * Created: 2016/3/21 9:03:25 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Ywdsoft.Utility.ConfigHandler
{
    /// <summary>
    /// 参数配置缓存
    /// </summary>
    public class ConfigDescriptionCache
    {
        private static Hashtable s_typeInfoDict = Hashtable.Synchronized(new Hashtable(2048));

        private static BindingFlags s_flag = BindingFlags.Public | BindingFlags.Static;

        /// <summary>
        /// 类型获取Attribute信息
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        public static ConfigDescription GetTypeDiscription(Type type)
        {
            ConfigDescription description = s_typeInfoDict[type.FullName] as ConfigDescription;
            if (description == null)
            {
                ConfigTypeAttribute typeAttr = type.GetMyAttribute<ConfigTypeAttribute>();
                description = new ConfigDescription { Group = typeAttr.Group, GroupCn = typeAttr.GroupCn, ImmediateUpdate = typeAttr.ImmediateUpdate };
                if (!string.IsNullOrEmpty(description.Group))
                {
                    //获取分组类型属
                    description.GroupTypePropertyInfo = type.GetProperty("GroupType");

                    //获取静态配置项属性集合
                    PropertyInfo[] propertyInfos = type.GetProperties(s_flag);
                    int length = propertyInfos.Length;
                    description.StaticPropertyInfo = new List<PropertyInfo>(length);
                    Dictionary<string, ConfigAttribute> dict = new Dictionary<string, ConfigAttribute>(length, StringComparer.OrdinalIgnoreCase);

                    ConfigAttribute configAttr = null;
                    Type ValueType = typeof(ConfigValueType);
                    foreach (PropertyInfo prop in propertyInfos)
                    {
                        //将配置了ConfigAttribute的静态属性添加到缓存，其它排除
                        configAttr = prop.GetMyAttribute<ConfigAttribute>();
                        if (configAttr != null)
                        {
                            if (string.IsNullOrEmpty(configAttr.Name))
                            {
                                //显示中文必须填写
                                continue;
                            }
                            if (string.IsNullOrEmpty(configAttr.Key))
                            {
                                //默认为属性值
                                configAttr.Key = prop.Name;
                            }
                            //设置值类型
                            if (!Enum.IsDefined(ValueType, configAttr.ValueType))
                            {
                                SetConfigValueType(configAttr, prop.PropertyType);
                            }
                            dict[prop.Name] = configAttr;
                            description.StaticPropertyInfo.Add(prop);
                        }
                    }
                    description.MemberDict = dict;
                }

                // 添加到缓存字典
                s_typeInfoDict[type.FullName] = description;
            }
            return description;
        }

        /// <summary>
        /// 设置默认项数值类型-根据属性类型进行转换
        /// </summary>
        /// <param name="configAttr"></param>
        /// <param name="propertyType">属性类型</param>
        private static void SetConfigValueType(ConfigAttribute configAttr,Type propertyType)
        {
            switch (propertyType.ToString()) {
                case "System.String":
                    configAttr.ValueType = ConfigValueType.String;
                    break;
                case "System.Boolean":
                    configAttr.ValueType = ConfigValueType.Bool;
                    break;
                case "System.Int32":
                case "System.Double":
                    configAttr.ValueType = ConfigValueType.Number;
                    break;
                default:
                    configAttr.ValueType = ConfigValueType.String;
                    break;
            }
        }
    }

    /// <summary>
    /// 参数配置相关信息
    /// </summary>
    public class ConfigDescription
    {
        /// <summary>
        /// 分组类型-用来区分数据
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupCn { get; set; }

        public bool ImmediateUpdate { get; set; }

        /// <summary>
        /// 静态配置项属性
        /// </summary>
        public List<PropertyInfo> StaticPropertyInfo { get; set; }

        /// <summary>
        /// 分组类型
        /// </summary>
        public PropertyInfo GroupTypePropertyInfo { get; set; }

        public Dictionary<string, ConfigAttribute> MemberDict { get; set; }
    }
}
