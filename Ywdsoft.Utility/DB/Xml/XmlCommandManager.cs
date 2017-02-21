using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.IO;
using System.Web;


namespace Ywdsoft.Utility.Extensions.Xml
{
    /// <summary>
    /// 用于维护配置文件中数据库访问命令的管理类
    /// </summary>
    public static class XmlCommandManager
    {
        private static readonly string CacheKey = Guid.NewGuid().ToString();
        private static System.Exception s_ExceptionOnLoad = null;
        private static Dictionary<string, XmlCommandItem> s_dict = null;


        /// <summary>
        /// 当前运行环境是否为测试环境（非ASP.NET环境）
        /// </summary>
        private static readonly bool IsWindowsEnvironment = (HttpRuntime.AppDomainAppId == null);


        /// <summary>
        /// <para>从指定的目录中加载全部的用于数据访问命令。</para>
        /// <para>说明：1. 这个方法只需要在程序初始化调用一次就够了。</para>
        /// <para>       2. 如果是一个Windows程序，CommandManager还会负责监视此目录，如果文件有更新，会自动重新加载。</para>
        /// </summary>
        /// <param name="directoryPaths">包含数据访问命令的目录。不加载子目录，仅加载扩展名为 .config 的文件。</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void LoadCommnads(string XmlCommandFolder)
        {
            if (s_dict != null && IsWindowsEnvironment)
            {
                // 不要删除这个判断检查，因为后面会监视这个目录。
                throw new InvalidOperationException("不允许重复调用这个方法。");
            }

            if (!string.IsNullOrEmpty(XmlCommandFolder))
            {
                string[] folders = XmlCommandFolder.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < folders.Length; i++)
                {
                    folders[i] = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folders[i]);
                }

                if (folders == null)
                {
                    throw new ArgumentNullException("folders");
                }
                foreach (string directoryPath in folders)
                {
                    if (Directory.Exists(directoryPath) == false)
                        throw new DirectoryNotFoundException(string.Format("目录 {0} 不存在。", directoryPath));
                }
                System.Exception exception = null;
                s_dict = LoadCommandsInternal(folders, out exception);

                if (exception != null)
                {
                    s_ExceptionOnLoad = exception;
                }

                if (s_ExceptionOnLoad != null)
                {
                    throw s_ExceptionOnLoad;
                }

                if (IsWindowsEnvironment)
                {
                    //初始化监控器
                    FileSystemWatcher watcher = new FileSystemWatcher(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, XmlCommandFolder), "*.config");
                    watcher.Created += new FileSystemEventHandler(OnProcess);
                    watcher.Changed += new FileSystemEventHandler(OnProcess);
                    watcher.Deleted += new FileSystemEventHandler(OnProcess);
                    watcher.IncludeSubdirectories = true;
                    watcher.EnableRaisingEvents = true;
                }
            }
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
                if (e.ChangeType == WatcherChangeTypes.Created)
                {
                    if (File.GetAttributes(e.FullPath) != FileAttributes.Directory)
                    {
                        CacheUpdateCallback(e.FullPath);
                    }
                }
                else if (e.ChangeType == WatcherChangeTypes.Changed)
                {

                    if (File.GetAttributes(e.FullPath) == FileAttributes.Directory)
                    {
                        return;
                    }
                    CacheUpdateCallback(e.FullPath);
                }
                else if (e.ChangeType == WatcherChangeTypes.Deleted)
                {
                }
            }
            catch
            {
            }
        }

        internal static void AddOrUpdateValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            try
            {
                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, value);
                }
                else
                {
                    dict[key] = value;
                }
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(string.Format("往集合中插入元素时发生了异常，当前Key={0}", key), ex);
            }
        }

        private static Dictionary<string, XmlCommandItem> LoadCommandsInternal(string[] directoryPaths, out System.Exception exception)
        {
            exception = null;
            Dictionary<string, XmlCommandItem> dict = new Dictionary<string, XmlCommandItem>(1024 * 2);
            try
            {
                foreach (string directoryPath in directoryPaths)
                {
                    string[] files = Directory.GetFiles(directoryPath, "*.config", SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        foreach (string file in files)
                        {
                            List<XmlCommandItem> list = XmlHelper.XmlDeserializeFromFile<List<XmlCommandItem>>(file, Encoding.UTF8);
                            list.ForEach(x => dict.AddOrUpdateValue(x.CommandName, x));
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                exception = ex;
                dict = null;
            }
            return dict;
        }

        private static void CacheUpdateCallback(string Path)
        {
            for (int i = 0; i < 5; i++)
            {
                // 由于事件发生时，文件可能还没有完全关闭，所以只好让程序稍等。
                System.Threading.Thread.Sleep(3000);
                try
                {
                    List<XmlCommandItem> list = XmlHelper.XmlDeserializeFromFile<List<XmlCommandItem>>(Path, Encoding.UTF8);
                    list.ForEach(x => s_dict.AddOrUpdateValue(x.CommandName, x));
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// 根据配置文件中的命令名获取对应的命令对象。
        /// </summary>
        /// <param name="name">命令名称，它应该能对应一个XmlCommand</param>
        /// <returns>如果找到符合名称的XmlCommand，则返回它，否则返回null</returns>
        public static XmlCommandItem GetCommand(string name)
        {
            if (s_ExceptionOnLoad != null)
                throw s_ExceptionOnLoad;

            if (s_dict == null)
                return null;

            XmlCommandItem command;
            if (s_dict.TryGetValue(name, out command))
                return command;

            return null;
        }


    }
}
