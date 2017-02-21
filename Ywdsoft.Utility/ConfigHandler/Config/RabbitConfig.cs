/*
 * Model: 模块名称
 * Desctiption: 描述
 * Author: 杜冬军
 * Created: 2016/3/21 15:34:13 
 * Copyright：武汉中科通达高新技术股份有限公司
 */

using System.ComponentModel.Composition;

namespace Ywdsoft.Utility.ConfigHandler
{
    /// <summary>
    /// MQ消息队列配置信息
    /// </summary>
    [Export(typeof(ConfigOption))]

    [ConfigType(Group = "RabbitMQConfig", GroupCn = "消息队列配置", ImmediateUpdate = false)]
    public class RabbitMQConfig : ConfigOption
    {
        /// <summary>
        /// 任务队列名称
        /// </summary>
        [Config(Name = "任务队列名称", DefaultValue = "Ywdsoft.Queue.Task")]
        public static string QueueName { get; set; }

        /// <summary>
        /// MQ主机IP
        /// </summary>
        [Config(Name = "主机地址", ValidateRule = "ipv4=true", DefaultValue = "127.0.0.1")]
        public static string Host { get; set; }

        /// <summary>
        /// MQ连接用户名
        /// </summary>
        [Config(Name = "用户名", DefaultValue = "admin")]
        public static string UserName { get; set; }

        /// <summary>
        /// MQ连接密码
        /// </summary>
        [Config(Name = "密码", ValueType = ConfigValueType.Password, DefaultValue = "admin")]
        public static string Password { get; set; }

        /// <summary>
        /// MQ连接端口
        /// </summary>
        [Config(Name = "端口", DefaultValue = "5672", ValidateRule = "min=0 digits=true")]
        public static int Port { get; set; }

        /// <summary>
        /// 消息队列持久化
        /// </summary>
        [Config(Name = "消息队列持久化", DefaultValue = true)]
        public static bool Durable { get; set; }

        /// <summary>
        /// 消息回执
        /// </summary>
        [Config(Name = "消息回执", DefaultValue = true)]
        public static bool Ack { get; set; }

    }
}