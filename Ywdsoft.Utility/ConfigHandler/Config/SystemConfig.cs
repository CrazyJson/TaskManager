using System.ComponentModel.Composition;

namespace Ywdsoft.Utility.ConfigHandler
{
    /// <summary>
    /// 系统基础配置信息
    /// </summary>
    [Export(typeof(ConfigOption))]

    [ConfigType(Group = "SystemConfig", GroupCn = "系统基础配置", ImmediateUpdate = true)]
    public class SystemConfig : ConfigOption
    {
        /// <summary>
        /// 系统页面标题
        /// </summary>
        [Config(Name = "系统页面标题", DefaultValue = "任务管理平台TaskManager")]
        public static string SystemTitle { get; set; }


        /// <summary>
        /// 系统静态资源文件版本管理
        /// </summary>
        [Config(Name = "静态资源版本", DefaultValue = "20160322195917")]
        public static string StaticVersion { get; set; }

        /// <summary>
        /// 是否显示异常信息
        /// </summary>
        [Config(Name = "是否显示异常", DefaultValue = true)]
        public static bool ShowException { get; set; }

        /// <summary>
        /// MQ连接端口
        /// </summary>
        [Config(Name = "管理站点端口", DefaultValue = "9000", ValidateRule = "min=0 digits=true")]
        public static int WebPort { get; set; }
    }
}