using Ywdsoft.Task.Utils;
using Ywdsoft.Utility;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Ywdsoft.Task.TaskSet
{
    /// <summary>
    /// 自动运行程序的任务
    /// </summary>
    ///<remarks>DisallowConcurrentExecution属性标记任务不可并行，要是上一任务没运行完即使到了运行时间也不会运行</remarks>
    [DisallowConcurrentExecution]
    public class AutoRunJob : IJob
    {
        /// <summary>
        /// 运行程序和进程名关系
        /// </summary>
        private static Dictionary<string, string> dictProcess = new Dictionary<string, string>();

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                object objParam = context.JobDetail.JobDataMap.Get("TaskParam");
                if (objParam != null)
                {
                    string[] listexe = objParam.ToString().Split(';');
                    string processName;
                    int startIndex, endIndex;
                    foreach (var path in listexe)
                    {
                        if (File.Exists(path))
                        {
                            if (!dictProcess.TryGetValue(path, out processName))
                            {
                                startIndex = path.LastIndexOf("\\") + 1;
                                endIndex = path.LastIndexOf(".");
                                //进程名默认为运行程序名称
                                processName = path.Substring(startIndex, endIndex - startIndex);
                            }
                            if (Process.GetProcessesByName(processName).ToList().Count <= 0)
                            {
                                //不存在
                                ProcessStartInfo ps = new ProcessStartInfo(path);
                                ps.UseShellExecute = false;
                                ps.CreateNoWindow = true;
                                ps.RedirectStandardOutput = true;
                                Process p = Process.Start(ps);
                                processName = p.ProcessName;
                            }
                            dictProcess[path] = processName;

                            TaskLog.AutoRunLogInfo.WriteLogE("程序:【" + processName + "】已经运行,时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "||" + Process.GetProcessesByName(processName).Count());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                JobExecutionException e2 = new JobExecutionException(ex);
                TaskLog.AutoRunLogError.WriteLogE("自动运行程序的任务异常", ex);
                //1.立即重新执行任务 
                e2.RefireImmediately = true;
            }
        }
    }
}
