using System.ComponentModel.Composition;

namespace Ywdsoft.Utility.ConfigHandler
{
    /// <summary>
    /// IP代理获取任务配置信息
    /// </summary>
    [Export(typeof(ConfigOption))]

    [ConfigType(Group = "IpProxyConfig", GroupCn = "代理IP任务配置", ImmediateUpdate = true)]
    public class IpProxyConfig : ConfigOption
    {
        /// <summary>
        /// 提取代理ip站点地址
        /// </summary>
        [Config(Name = "提取代理IP站点", DefaultValue = "http://www.xicidaili.com/nn")]
        public static string IPUrl { get; set; }

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

        /// <summary>
        /// 是否对获取的代理ip进行ping命令处理,确定该代理是否有效
        /// </summary>
        [Config(Name = "IP有效性校验", DefaultValue = false)]
        public static bool IsPingIp { get; set; }

    }
}