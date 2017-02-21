using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Ywdsoft.Utility.Command
{
    /// <summary>
    /// 清屏命令
    /// </summary>
    [Export(typeof(ICommand))]
    public class ClsCommand : ICommand
    {
        private static Dictionary<string, string> dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){
            { CommandKey.CLS,"清除屏幕" }
        };

        /// <summary>
        /// 处理的命令-清屏命令
        /// </summary>
        public string GetCommandKey()
        {
            return CommandKey.CLS;
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
            Console.Clear();
            return string.Empty;
        }
    }
}
