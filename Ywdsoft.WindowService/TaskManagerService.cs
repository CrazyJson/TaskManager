using Ywdsoft.Utility;
using Owin_Nancy;
using System.ServiceProcess;
using System.Threading;
using Ywdsoft.Utility.ConfigHandler;
using Ywdsoft.Utility.Mef;
using Ywdsoft.Utility.Extensions.Xml;
using Ywdsoft.Utility.Command;
using System;
using Nancy.Hosting.Self;
using Ywdsoft.Utility.RabbitMQ;

namespace TaskManager
{
    /*
     * 功能介绍： TaskManager是基于Quartz.NET的一款开源任务管理系统，
     * 使用Window服务来承载。目前系统集成了四个常用任务，代理IP爬虫，快递进度，消息通知，动态修改Job任务
     * 
     * 作者：焰尾迭
     * 博客地址：http://www.cnblogs.com/yanweidie/p/3564062.html
     */
    public partial class TaskManagerService : ServiceBase
    {
        public TaskManagerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                //1.MEF初始化
                MefConfig.Init();

                //2.数据库初始化连接
                ConfigInit.InitConfig();

                //3.系统参数配置初始化
                ConfigManager configManager = MefConfig.TryResolve<ConfigManager>();
                configManager.Init();

                //4.任务启动
                QuartzHelper.InitScheduler();
                QuartzHelper.StartScheduler();

                //5.加载SQL信息到缓存中
                XmlCommandManager.LoadCommnads(SysConfig.XmlCommandFolder);
                //开发时监听资源文件变化，用于实时更新
                DevelperHelper.WatcherResourceChange();

                // 保持web服务运行  
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    //7.启动站点
                    using (NancyHost host = Startup.Start(SystemConfig.WebPort))
                    {
                        //调用系统默认的浏览器   
                        string url = string.Format("http://127.0.0.1:{0}", SystemConfig.WebPort);
                        LogHelper.WriteLog(string.Format("系统已启动，当前监听站点地址:{0}", url));
                        try
                        {
                            //4.消息队列启动
                            RabbitMQClient.InitClient();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        //8.系统命令初始化
                        CommandHelp.Init();
                    }
                });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("服务启动异常", ex);
            }
        }

        protected override void OnStop()
        {
            //回收资源
            Startup.Dispose();
            Environment.Exit(0);
        }
    }
}
