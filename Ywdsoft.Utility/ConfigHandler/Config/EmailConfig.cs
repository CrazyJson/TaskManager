using System.ComponentModel.Composition;

namespace Ywdsoft.Utility.ConfigHandler
{
    /// <summary>
    /// Email配置
    /// </summary>
    [Export(typeof(ConfigOption))]

    [ConfigType(Group = "EmailConfig", GroupCn = "Email配置", ImmediateUpdate = true)]
    public class EmailConfig : ConfigOption
    {
        /// <summary>
        /// Email服务器主机
        /// </summary>
        [Config(Name = "服务器主机", DefaultValue = "smtp.exmail.qq.com")]
        public static string Host { get; set; }


        /// <summary>
        /// 用户名
        /// </summary>
        [Config(Name = "用户名", DefaultValue = "notify@myscloud.cn")]
        public static string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Config(Name = "密码", DefaultValue = "task123456", ValueType = ConfigValueType.Password)]
        public static string Password { get; set; }

        /// <summary>
        /// 发送邮箱
        /// </summary>
        [Config(Name = "发送邮箱", DefaultValue = "notify@myscloud.cn")]
        public static string SendMail { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [Config(Name = "显示名称", DefaultValue = "任务系统通知")]
        public static string DisplayName { get; set; }
    }
}