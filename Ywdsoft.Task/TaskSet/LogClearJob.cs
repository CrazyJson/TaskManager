using Ywdsoft.Utility;
using Quartz;
using System;
using System.IO;
using Ywdsoft.Utility.ConfigHandler;

namespace Ywdsoft.Task.TaskSet
{
    /// <summary>
    ///  
    /// </summary>
    ///<remarks>DisallowConcurrentExecution属性标记任务不可并行，要是上一任务没运行完即使到了运行时间也不会运行</remarks>
    [DisallowConcurrentExecution]
    public class LogClearJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                string LogDir = FileHelper.GetAbsolutePath("/Logs");
                string[] files = Directory.GetFiles(LogDir, "*.*", SearchOption.AllDirectories);
                FileInfo fi = null;
                foreach (var item in files)
                {
                    fi = new FileInfo(item);
                    if ((DateTime.Now - fi.CreationTime).TotalDays > LogConfig.Days)
                    {
                        try
                        {
                            File.Delete(item);
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                JobExecutionException e2 = new JobExecutionException(ex);
                LogHelper.WriteLog("日志清理任务异常", ex);
                //1.立即重新执行任务 
                e2.RefireImmediately = true;
            }
        }
    }
}
