/*
 * Model: 系统参数配置接口
 * Desctiption: 描述
 * Author: 杜冬军
 * Created: 2016/3/21 9:03:25 
 */

using System;

namespace Ywdsoft.Utility.ConfigHandler
{

    [AttributeUsageAttribute(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ConfigAttribute : Attribute
    {
        /// <summary>
        /// 对应数据库的键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 每项值中文说明,表单前面的说明
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// 每项值中文说明,表单前面的说明
        /// </summary>
        public ConfigValueType ValueType { get; set; }

        /// <summary>
        /// 默认为必填
        /// </summary>
        private bool _required = true;
        /// <summary>
        /// 值是否必填
        /// </summary>
        public bool Required
        {
            get
            {
                return _required;
            }
            set
            {
                _required = value;
            }
        }

        /// <summary>
        /// 字段前端校验规则-对应jquery.validate的校验规则
        /// </summary>
        public string ValidateRule { get; set; }

        /// <summary>
        /// 界面鼠标放上去Title
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// 每项数值类型
    /// </summary>
    public enum ConfigValueType
    {
        /// <summary>
        /// 整形
        /// </summary>
        Number = 1,

        /// <summary>
        /// 字符串
        /// </summary>
        String = 2,

        /// <summary>
        /// 布尔型
        /// </summary>
        Bool = 3,

        /// <summary>
        /// 密码
        /// </summary>
        Password = 4,

        /// <summary>
        /// 多行文本
        /// </summary>
        TextArea = 5
    }
}
