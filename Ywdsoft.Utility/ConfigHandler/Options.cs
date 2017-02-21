/*
 * Model: 系统配置
 * Desctiption: 描述
 * Author: 杜冬军
 * Created: 2016/3/21 15:34:13 
 */

using System.Collections.Generic;

namespace Ywdsoft.Utility.ConfigHandler
{
    ///<summary>
    ///系统参数配置选项表
    ///</summary>
    public class Options
    {
        ///<summary>
        ///选项ID 
        ///</summary> 
        public string OptionId { get; set; }

        ///<summary>
        ///选项类型
        ///</summary> 
        public string OptionType { get; set; }

        ///<summary>
        ///选项名称 
        ///</summary> 
        public string OptionName { get; set; }

        ///<summary>
        ///参数Key值 
        ///</summary> 
        public string Key { get; set; }

        ///<summary>
        ///选项值 
        ///</summary> 
        public string Value { get; set; }

        ///<summary>
        ///值类型（0:整形  1:字符串 2:布尔型 3:密码） 
        ///</summary> 
        public string ValueType { get; set; }

        ///<summary>
        ///是否必填,默认为是
        ///</summary> 
        public bool Required { get; set; }

        ///<summary>
        ///是否立即进行服务间同步配置，并且立即应用配置，默认为是  为是会发送MQ消息
        ///</summary> 
        public bool ImmediateUpdate { get; set; }

        /// <summary>
        /// 字段校验规则
        /// </summary>
        public string ValidateRule { get; set; }

        /// <summary>
        /// 鼠标悬浮Title
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// 配置分组
    /// </summary>
    public class OptionGroup
    {
        /// <summary>
        /// 配置分组
        /// </summary>
        public string GroupType { get; set; }

        /// <summary>
        /// 分组中文
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 是否立即进行服务间同步配置，并且立即应用配置，默认为是  为是会发送MQ消息
        /// </summary>
        public bool ImmediateUpdate { get; set; }

        /// <summary>
        /// 自定义页面地址，不为空的化则跳转到自定义配置页面
        /// </summary>
        public string CustomPage { get; set; }
    }

    /// <summary>
    /// configViewModel
    /// </summary>
    public class OptionViewModel
    {
        public OptionGroup Group { get; set; }

        public List<Options> ListOptions { get; set; }
    }
}

