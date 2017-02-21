using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Ywdsoft.Utility.ConfigHandler;

namespace Ywdsoft.Utility.Command
{
    /// <summary>
    /// 打开监听端口网页命令
    /// </summary>
    [Export(typeof(ICommand))]
    public class HttpCommand : ICommand
    {
        private static Dictionary<string, string> dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){
            { CommandKey.HTTP,"在浏览器中打开 系统 设置页面" }
        };

        /// <summary>
        /// 处理的命令-打开监听端口网页命令
        /// </summary>
        public string GetCommandKey()
        {
            return CommandKey.HTTP;
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
            //调用系统默认的浏览器   
            string url = string.Format("http://127.0.0.1:{0}", SystemConfig.WebPort);
            Process.Start(url);
            return string.Empty;
        }
    }
}
