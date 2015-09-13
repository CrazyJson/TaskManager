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
