using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Ywdsoft.Utility.Command
{
    /// <summary>
    /// 退出程序命令
    /// </summary>
    [Export(typeof(ICommand))]
    public class ExitCommand : ICommand
    {
        private static Dictionary<string, string> dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){
            { CommandKey.EXIT,"退出程序" }
        };

        /// <summary>
        /// 处理的命令-退出程序
        /// </summary>
        public string GetCommandKey()
        {
            return CommandKey.EXIT;
        }

        /// <summary>
        /// 命令-及参数描述
        /// </summary>
        /// <returns>命令-及参数描述</returns>
        public Dictionary<string, string> GetCommandRemark()
        {
            return dict;
        }

        /// <summary>
        /// 具体命令执行函数
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns>执行结果</returns>
        public string Excute(params string[] args)
        {
            QuartzHelper.StopSchedule();
            System.Environment.Exit(0);
            return string.Empty;
        }
    }
}
