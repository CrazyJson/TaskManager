using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ywdsoft.Utility.Command
{
    /// <summary>
    /// 命令行帮助
    /// </summary>
    [Export(typeof(ICommand))]
    public class HelpCommand : ICommand
    {
        private static Dictionary<string, string> dictRemark = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){
            { CommandKey.HELP,"提供 系统 命令的帮助信息" }
        };

        /// <summary>
        /// 处理的命令
        /// </summary>
        public string GetCommandKey()
        {
            return CommandKey.HELP;
        }

        /// <summary>
        /// 命令-及参数描述
        /// </summary>
        /// <returns>命令-及参数描述</returns>
        public Dictionary<string, string> GetCommandRemark()
        {
            return dictRemark;
        }

        /// <summary>
        /// 具体命令执行函数
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns>执行结果</returns>
        public string Excute(params string[] args)
        {
            var dictCommand = CommandHelp.GetAllCommand();
            StringBuilder sb = new StringBuilder();
            ICommand command = null;
            Dictionary<string, string> dictRemark = null;
            string remark = string.Empty;
            Dictionary<string, string> helpRemark = GetCommandRemark();
            if (args == null || args.Length == 0)
            {
                //输出所有命令
                sb.Append("有关某个命令的详细信息，请键入 HELP 命令名");
                sb.AppendLine();
                foreach (string key in dictCommand.Keys)
                {
                    command = dictCommand[key];
                    dictRemark = command.GetCommandRemark();
                    if (dictRemark == null)
                    {
                        continue;
                    }
                    if (dictRemark.TryGetValue(key, out remark))
                    {
                        sb.AppendFormat("{0}{1}", key.PadRight(15, ' ').ToUpper(), remark);
                        sb.AppendLine();
                    }
                }
            }
            else if (args.Length == 1)
            {
                string key = args[0];
                if (dictCommand.TryGetValue(key, out command))
                {
                    dictRemark = command.GetCommandRemark();
                    if (dictRemark != null)
                    {
                        foreach (string tkey in dictRemark.Keys)
                        {
                            remark = dictRemark[tkey];
                            if (tkey.Equals(key, StringComparison.CurrentCultureIgnoreCase) ||
                                tkey.Equals(CommandKey.EXAMPLE, StringComparison.CurrentCultureIgnoreCase))
                            {
                                sb.Append(remark);
                            }
                            else
                            {
                                sb.AppendFormat("  {0}{1}", tkey.PadRight(13, ' '), remark);
                            }
                            sb.AppendLine();
                        }
                    }
                    sb.AppendLine();
                }
                else
                {
                    sb.Append("不支持此命令");
                    sb.AppendLine();
                }
            }
            else
            {
                sb.Append(helpRemark[CommandKey.HELP]);
                sb.AppendLine();
                sb.Append("HELP [command]");
                sb.AppendLine();
                sb.Append("    command - 显示该命令的帮助信息");
                sb.AppendLine();
            }
            Console.Write(sb.ToString());
            Console.WriteLine();
            return sb.ToString();
        }
    }
}
