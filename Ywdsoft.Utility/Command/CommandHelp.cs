using System;
using System.Collections.Generic;
using System.Linq;
using Ywdsoft.Utility.Mef;

namespace Ywdsoft.Utility.Command
{
    /// <summary>
    /// 系统命令初始化构造器
    /// </summary>
    public class CommandHelp
    {
        /// <summary>
        /// 所有命令
        /// </summary>
        private static Dictionary<string, ICommand> CommandDict = null;

        /// <summary>
        /// 获取系统所有命令
        /// </summary>
        /// <returns>系统所有命令</returns>
        public static Dictionary<string, ICommand> GetAllCommand()
        {
            if (CommandDict == null)
            {
                CommandDict = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);
                string key = string.Empty;
                var listCommand = MefConfig.ResolveMany<ICommand>().OrderBy(e => e.GetCommandKey()).ToList();
                foreach (var item in listCommand)
                {
                    key = item.GetCommandKey();
                    if (!string.IsNullOrEmpty(key))
                    {
                        CommandDict[key] = item;
                    }
                }
            }
            return CommandDict;
        }

        /// <summary>
        /// 命令初始化
        /// </summary>
        public static void Init()
        {
            Console.WriteLine("可以输入【Help】查看系统所有命令");
            GetAllCommand();
            while (true)
            {
                string strCommand = Console.ReadLine();
                if (string.IsNullOrEmpty(strCommand))
                {
                    continue;
                }
                else
                {
                    strCommand = strCommand.Trim();
                }
                ExcuteCommad(strCommand);
            }
        }

        public static string ExcuteCommad(string strCommand)
        {
            ICommand command = null;
            var listCommand = strCommand.Split(' ').Where(e => !string.IsNullOrEmpty(e)).ToList();
            if (listCommand.Count == 0)
            {
                return string.Empty;
            }
            if (CommandDict.TryGetValue(listCommand[0], out command))
            {
                listCommand.RemoveAt(0);
                try
                {
                    return command.Excute(listCommand.ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return ex.Message;
                }
            }
            else
            {
                string msg = string.Format("'{0}' 不是可识别的命令", listCommand[0]);
                Console.WriteLine(msg);
                return msg;
            }
        }
    }
}
