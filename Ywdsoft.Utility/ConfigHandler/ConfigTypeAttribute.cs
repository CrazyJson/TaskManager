/*
 * Model: 系统参数配置接口
 * Desctiption: 描述
 * Author: 杜冬军
 * Created: 2016/3/21 9:03:25 
 */

using System;

namespace Ywdsoft.Utility.ConfigHandler
{

    [AttributeUsageAttribute(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ConfigTypeAttribute : Attribute
    {
        /// <summary>
        /// 分组类型-用来区分数据
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupCn { get; set; }


        /// <summary>
        /// 是否立即进行服务间同步配置，并且立即应用配置，默认为是
        /// </summary>
        private bool _immediateUpdate = true;

        /// <summary>
        /// 是否立即进行服务间同步配置，并且立即应用配置，默认为是  为是会发送MQ消息
        /// </summary>
        public bool ImmediateUpdate
        {
            get
            {
                return _immediateUpdate;
            }
            set
            {
                _immediateUpdate = value;
            }
        }
    }
}
