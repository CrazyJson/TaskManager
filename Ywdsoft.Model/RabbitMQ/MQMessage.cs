/*
 * Model: MQ消息实体定义
 * Desctiption: 描述
 * Author: 杜冬军
 * Created: 2016/3/17 13:39:17 
 */
using System;
using System.Collections.Generic;

namespace Ywdsoft.Model.RabbitMQ
{
    /// <summary>
    /// MQ消息格式实体定义
    /// </summary>
    public class MQMessage<T> where T : BaseMessage
    {
        /// <summary>
        /// 用来区分消息类型,必须传值
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 消息内容,支持发送多条同类型消息 
        /// </summary>
        /// <remarks>
        /// 比如批量导入布控数据，这个时候发送的是多条布控数据
        /// </remarks>
        public List<T> Data { get; set; }

        /// <summary>
        /// 消息发送的时间
        /// </summary>
        public string ReportedTime
        {
            get
            {
                return DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }
    }

    /// <summary>
    /// 单条消息基类
    /// </summary>
    public class BaseMessage
    {
        /// <summary>
        /// 数据操作类型<seealso cref="ChangeType"/>
        /// </summary>
        public string ChangeType { get; set; }

        public string Id { get; set; }
    }


    /// <summary>
    /// 数据操作类型
    /// </summary>
    public class ChangeType
    {
        /// <summary>
        /// 新增数据
        /// </summary>
        public const string Insert = "Insert";
        /// <summary>
        /// 更新数据
        /// </summary>
        public const string Update = "Update";
        /// <summary>
        /// 删除数据
        /// </summary>
        public const string Delete = "Delete";
    }
}