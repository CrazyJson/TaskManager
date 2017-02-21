using Ywdsoft.Utility;
using Nancy.Hosting.Self;
using Owin_Nancy;
using System;
using System.Diagnostics;
using Ywdsoft.Utility.Mef;
using Ywdsoft.Utility.ConfigHandler;
using Ywdsoft.Utility.Command;
using System.Runtime.InteropServices;
using Ywdsoft.Utility.RabbitMQ;
using Ywdsoft.Utility.Extensions.Xml;
using YwdSoft.Service;

namespace Ywdsoft.ConsoleHosting
{
    class Program
    {
        public delegate bool ControlCtrlDelegate(int CtrlType);
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        private static ControlCtrlDelegate cancelHandler = new ControlCtrlDelegate(HandlerRoutine);

        public static bool HandlerRoutine(int CtrlType)
        {
            switch (CtrlType)
            {
                case 0:
                case 2:
                    return true;
            }
            return false;
        }

        static void Main(string[] args)
        {
            AdminRun.Run();

            if (!SetConsoleCtrlHandler(cancelHandler, true))
            {
                Console.WriteLine("程序监听系统按键异常");
            }
            try
            {
                //1.MEF初始化
                MefConfig.Init();

                //2.数据库初始化连接
                ConfigInit.InitConfig();

                //3.系统参数配置初始化
                ConfigManager configManager = MefConfig.TryResolve<ConfigManager>();
                configManager.Init();

                Console.Title = SystemConfig.ProgramName;
                Console.CursorVisible = false; //隐藏光标

                //4.任务启动
                QuartzHelper.InitScheduler();
                QuartzHelper.StartScheduler();

                //5.加载SQL信息到缓存中
                XmlCommandManager.LoadCommnads(SysConfig.XmlCommandFolder);

                DapperDemoService.Test();

                //启动站点
                using (NancyHost host = Startup.Start(SystemConfig.WebPort))
                {
                    //调用系统默认的浏览器   
                    string url = string.Format("http://127.0.0.1:{0}", SystemConfig.WebPort);
                    Process.Start(url);
                    Console.WriteLine("系统已启动，当前监听站点地址:{0}", url);

                    //4.消息队列启动
                    RabbitMQClient.InitClient();

                    //5.系统命令初始化
                    CommandHelp.Init();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
