using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Ywdsoft.Utility.Command
{
    /// <summary>
    /// web资源更新命令
    /// </summary>
    [Export(typeof(ICommand))]
    public class WebResourceUpdateCommand : ICommand
    {
        private static Dictionary<string, string> dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){
            { CommandKey.WRU,"web资源更新命令 用于更新替换开发时修改的静态资源，从而无需重新运行系统" }
        };

        /// <summary>
        /// 更新资源文件目录名
        /// </summary>
        private static string[] ArrCopyDir = new string[] { "Content", "Views", "Template" };

        /// <summary>
        /// 获取静态资源文件夹目录
        /// </summary>
        /// <returns>静态资源文件夹目录</returns>
        public static string[] GetResurceDir()
        {
            return ArrCopyDir;
        }

        /// <summary>
        /// 处理的命令
        /// </summary>
        public string GetCommandKey()
        {
            return CommandKey.WRU;
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
        public string Excute(params string[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("开始寻找父级TaskManagerWeb项目下的资源文件" + string.Join("，", ArrCopyDir));

            //获取当前运行路径的上级目录（父目录）
            string baseDirectorry = System.Environment.CurrentDirectory;
            DirectoryInfo topDir = null;
            do
            {
                topDir = Directory.GetParent(baseDirectorry);
                if (topDir != null)
                {
                    baseDirectorry = topDir.FullName;
                }
            }
            while (topDir != null && topDir.GetDirectories().FirstOrDefault(e => e.Name == "TaskManagerWeb") == null);
            bool isSucess = false;
            if (topDir != null)
            {
                DirectoryInfo webDir = topDir.GetDirectories().FirstOrDefault(e => e.Name == "TaskManagerWeb");
                if (webDir != null)
                {
                    sb.AppendLine("开始拷贝资源文件...");
                    string desDir = string.Empty;
                    foreach (var item in ArrCopyDir)
                    {
                        desDir = System.Environment.CurrentDirectory + "\\" + item;
                        Directory.Delete(desDir, true);
                        FileHelper.CopyDirectory(webDir.FullName + "\\" + item, desDir);
                        sb.AppendLine("资源文件【" + item + "】拷贝完成");
                    }
                    isSucess = true;
                }
            }

            if (isSucess)
            {
                sb.AppendLine("资源文件更新完成");
            }
            else
            {
                sb.AppendLine("资源文件更新失败");
            }
            LogHelper.WriteLog(sb.ToString());
            return sb.ToString();
        }
    }

    public class DevelperHelper
    {
        /// <summary>
        /// 监听文件改变
        /// </summary>
        public static void WatcherResourceChange()
        {
            //获取当前运行路径的上级目录（父目录）
            string baseDirectorry = System.Environment.CurrentDirectory;
            DirectoryInfo topDir = null;
            do
            {
                topDir = Directory.GetParent(baseDirectorry);
                if (topDir != null)
                {
                    baseDirectorry = topDir.FullName;
                }
            }
            while (topDir != null && topDir.GetDirectories().FirstOrDefault(e => e.Name == "TaskManagerWeb") == null);
            if (topDir != null)
            {
                DirectoryInfo webDir = topDir.GetDirectories().FirstOrDefault(e => e.Name == "TaskManagerWeb");
                var arrPath = WebResourceUpdateCommand.GetResurceDir();
                string[] arrRealPath = new string[arrPath.Length];
                for (int i = 0; i < arrPath.Length; i++)
                {
                    arrRealPath[i] = webDir.FullName + "\\" + arrPath[i] + "\\";
                }
                Parallel.ForEach(arrRealPath, Watch);
            }
        }

        private static void Watch(string Dir)
        {
            //初始化监控器
            FileSystemWatcher watcher = new FileSystemWatcher(Dir);
            watcher.Created += new FileSystemEventHandler(OnProcess);
            watcher.Changed += new FileSystemEventHandler(OnProcess);
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 文件改变事件监听
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnProcess(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (!e.FullPath.EndsWith(".TMP"))
                {
                    return;
                }
                string realPath = e.FullPath.Split('~')[0];
                if (File.Exists(realPath))
                {
                    System.Threading.Thread.Sleep(1000);
                    string desDir = System.Environment.CurrentDirectory + realPath.Substring(realPath.IndexOf("TaskManagerWeb") + "TaskManagerWeb".Length);
                    File.Copy(realPath, desDir, true);
                    LogHelper.WriteLog("资源文件" + Path.GetFileName(realPath) + "复制完成");
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("资源文件监听改变刷新异常", ex);
            }
        }
    }
}
