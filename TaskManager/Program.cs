using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace TaskManager
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {   
            ServiceBase[] ServicesToRun = new ServiceBase[] 
            { 
                new TaskManagerService() 
            };
            //运行window任务
            ServiceBase.Run(ServicesToRun);
        }
    }
}
