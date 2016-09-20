using System.ComponentModel.Composition;

namespace Ywdsoft.Utility.ConfigHandler
{
    /// <summary>
    /// 快递进度任务配置信息
    /// </summary>
    [Export(typeof(ConfigOption))]

    [ConfigType(Group = "ExpressProgressConfig", GroupCn = "快递进度任务配置", ImmediateUpdate = true)]
    public class ExpressProgressConfig : ConfigOption
    {
        /// <summary>
        /// 请求站点默认使用的代理ip信息
        /// </summary>
        [Config(Name = "代理IP", Required = false)]
        public static string DefaultProxyIp { get; set; }

        /// <summary>
        /// 每执行x次任务切换代理IP
        /// </summary>
        [Config(Name = "x次任务切换代理IP", DefaultValue = "5", ValidateRule = "min=1 max=50 digits=true")]
        public static int Speed { get; set; }

    }
}