using System.ComponentModel.Composition;

namespace Ywdsoft.Utility.ConfigHandler
{
    /// <summary>
    /// 任务配置信息
    /// </summary>
    [Export(typeof(ConfigOption))]

    [ConfigType(Group = "TaskConfig", GroupCn = "任务配置", ImmediateUpdate = false)]
    public class TaskConfig : ConfigOption
    {
        /// <summary>
        /// 任务线程数
        /// </summary>
        [Config(Name = "任务线程数", DefaultValue = "50", ValidateRule = "min=1 max=1000 digits=true")]
        public static int TaskThreadCount { get; set; }
    }
}