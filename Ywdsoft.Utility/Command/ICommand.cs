using System.Collections.Generic;

namespace Ywdsoft.Utility.Command
{
    /// <summary>
    /// 控制台提供命令执行接口
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// 处理的命令
        /// </summary>
        string GetCommandKey();

        /// <summary>
        /// 命令-及参数描述
        /// </summary>
        /// <returns>命令-及参数描述</returns>
        Dictionary<string, string> GetCommandRemark();

        /// <summary>
        /// 具体命令执行函数
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns>执行结果</returns>
        string Excute(params string[] args);
    }

    /// <summary>
    /// 命令key
    /// </summary>
    public class CommandKey
    {
        /// <summary>
        /// 清屏命令
        /// </summary>
        public const string CLS = "Cls";

        /// <summary>
        /// 打开设置页面命令
        /// </summary>
        public const string HTTP = "Http";

        /// <summary>
        /// 帮助命令
        /// </summary>
        public const string HELP = "Help";

        /// <summary>
        /// 退出程序命令
        /// </summary>
        public const string EXIT = "Exit";

        /// <summary>
        /// 重启程序命令
        /// </summary>
        public const string RESTART = "Restart";

        /// <summary>
        /// 日志相关命令
        /// </summary>
        public const string LOG = "Log";

        /// <summary>
        /// 查看站点绑定端口
        /// </summary>
        public const string PORT = "Port";

        /// <summary>
        /// 查看站点绑定端口
        /// </summary>
        public const string WRU = "WRU";

        /// <summary>
        /// 每个命令样例
        /// </summary>
        public const string EXAMPLE = "Example";
    }
}
