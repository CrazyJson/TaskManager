using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Text;
using Ywdsoft.Utility.ConfigHandler;

namespace Ywdsoft.Utility.Command
{
    /// <summary>
    /// 日志命令
    /// </summary>
    [Export(typeof(ICommand))]
    public class LogCommand : ICommand
    {
        private static Dictionary<string, string> dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){
            { CommandKey.LOG,"日志相关命令" },
            { CommandKey.EXAMPLE,"LOG download|show [n]" },
            { "download","下载日志命令" },
            { "show","显示日志命令 " },
            { "n","显示日志文件最新的n行，最多 10000 行。show命令一起使用" }
        };

        /// <summary>
        /// 处理的命令-清屏命令
        /// </summary>
        public string GetCommandKey()
        {
            return CommandKey.LOG;
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
            StringBuilder sb = new StringBuilder();
            string tMsg = string.Empty;
            if (args == null || args.Length == 0)
            {
                tMsg = "无效的参数数量";
                Console.WriteLine(tMsg);
                sb.AppendLine(tMsg);
            }
            else
            {
                switch (args[0].ToLower())
                {
                    case "download":
                        //调用系统默认的浏览器   
                        string url = string.Format("http://127.0.0.1:{0}/Log/Download", SystemConfig.WebPort);
                        Process.Start(url);
                        tMsg = "命令log download执行完成，日志下载成功";
                        LogHelper.WriteLog(tMsg);
                        sb.AppendLine(tMsg);
                        break;
                    case "show":
                        int count = 10000;
                        if (args.Length > 1)
                        {
                            if (!int.TryParse(args[1], out count))
                            {
                                tMsg = "命令log show[n] 格式不正确,应该为小于等于10000的整数";
                                LogHelper.WriteLog(tMsg);
                                sb.AppendLine(tMsg);
                            }
                        }
                        tMsg = LogHelper.WriteNewLog(count);
                        sb.AppendLine(tMsg);
                        break;
                }
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// 日志命令帮助
    /// </summary>
    public class LogCmdHelper
    {
        private static int BUFFER_SIZE = 0x10000;

        /// <summary>
        /// 下载所有日志
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <param name="s">文件输出流</param>
        public static void DowloadLog(string FilePath, Stream s)
        {
            string desc = FileHelper.GetAbsolutePath(Guid.NewGuid().ToString("N"));
            FileHelper.CopyDirectory(FileHelper.GetAbsolutePath("Logs"), desc);
            SharpZip.PackFiles(FileHelper.GetAbsolutePath(FilePath), desc);
            Directory.Delete(desc, true);
            byte[] m_buffer = new byte[BUFFER_SIZE];
            int count = 0;
            using (FileStream fs = File.OpenRead(FilePath))
            {
                do
                {
                    count = fs.Read(m_buffer, 0, BUFFER_SIZE);
                    s.Write(m_buffer, 0, count);
                } while (count == BUFFER_SIZE);
            }
            File.Delete(FilePath);
        }


        /// <summary>
        /// 下载所有日志
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <param name="s">文件输出流</param>
        /// <param name="list">要导出的文件路径集合</param>
        public static void DowloadLog(string FilePath, List<string> list, Stream s)
        {
            string desc = FileHelper.GetAbsolutePath(Guid.NewGuid().ToString("N"));
            FileHelper.CopyFiles(list, desc);
            SharpZip.PackFiles(FileHelper.GetAbsolutePath(FilePath), desc);
            Directory.Delete(desc, true);
            byte[] m_buffer = new byte[BUFFER_SIZE];
            int count = 0;
            using (FileStream fs = File.OpenRead(FilePath))
            {
                do
                {
                    count = fs.Read(m_buffer, 0, BUFFER_SIZE);
                    s.Write(m_buffer, 0, count);
                } while (count == BUFFER_SIZE);
            }
            File.Delete(FilePath);
        }
    }
}
