using Mysoft.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

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
            DebuggableAttribute att = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttribute<DebuggableAttribute>();
            if (att.IsJITTrackingEnabled)
            {
                //Debug模式才让线程停止10s,方便附加到进程调试
                Thread.Sleep(10000);
            }
            //配置信息读取
            ConfigInit.InitConfig();
            QuartzHelper.InitScheduler();
            QuartzHelper.StartScheduler();
        }

        protected override void OnStop()
        {
            QuartzHelper.StopSchedule();
            System.Environment.Exit(0);
        }
    }
}
