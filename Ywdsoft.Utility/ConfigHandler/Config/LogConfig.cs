using System.ComponentModel.Composition;

namespace Ywdsoft.Utility.ConfigHandler
{
    /// <summary>
    /// 日志清理配置信息
    /// </summary>
    [Export(typeof(ConfigOption))]

    [ConfigType(Group = "LogConfig", GroupCn = "日志清理", ImmediateUpdate = true)]
    public class LogConfig : ConfigOption
    {
        /// <summary>
        /// 删除X天之前的日志
        /// </summary>
        [Config(Name = "删除X天之前的日志", DefaultValue = "3", ValidateRule = "min=1 digits=true")]
        public static int Days { get; set; }
    }
}