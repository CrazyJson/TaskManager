using Mysoft.Task.Utils;
using Mysoft.Utility;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mysoft.Task.TaskSet
{
    /// <summary>
    /// 快递进度处理任务
    /// </summary>
    ///<remarks>DisallowConcurrentExecution属性标记任务不可并行，要是上一任务没运行完即使到了运行时间也不会运行</remarks>
    [DisallowConcurrentExecution]
    public class ExpressProgressJob : IJob
    {
        /// <summary>
        /// 任务总共执行次数
        /// </summary>
        private static int ExecuteCount = 0;

        /// <summary>
        /// 没执行5次任务切换代理IP
        /// </summary>
        private static int Speed = 5;

        /// <summary>
        /// 是否需要切换代理ip
        /// </summary>
        private static bool NeedChangeIP = false;

        /// <summary>
        /// 代理IP
        /// </summary>
        private static string ProxyIp;

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                object objParam = context.JobDetail.JobDataMap.Get("TaskParam");
                if (objParam != null)
                {
                    DateTime start = DateTime.Now;
                    TaskLog.ExpressProgressLogInfo.WriteLogE("\r\n\r\n\r\n\r\n------------------快递进度信息任务开始执行 " + start.ToString("yyyy-MM-dd HH:mm:ss") + " BEGIN-----------------------------\r\n\r\n");

                    //每执行5次任务,换一个代理IP
                    if (NeedChangeIP || ExecuteCount % Speed == 0)
                    {
                        if (NeedChangeIP)
                        {
                            ExecuteCount = (ExecuteCount / Speed + 1) * Speed;
                        }
                        TaskLog.ExpressProgressLogInfo.WriteLogE("\r\n\r\n\r\n\r\n------------------开始解析使用的代理ip " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " BEGIN-----------------------------\r\n\r\n");
                        ProxyIp = ExpressUtil.GetCorrectIP(objParam.ToString());
                        TaskLog.ExpressProgressLogInfo.WriteLogE("------------------保存使用的代理ip：" + ProxyIp + " -----------------------------");
                        SQLHelper.ExecuteNonQuery("INSERT INTO dbo.p_ProxyIPUseHistory(ProxyIP,Type) VALUES (@ProxyIP,'ExpressProgressJob')", new { ProxyIP = ProxyIp });
                        NeedChangeIP = false;
                    }

                    TaskLog.ExpressProgressLogInfo.WriteLogE("\r\n\r\n\r\n\r\n------------------任务使用的代理ip:" + ProxyIp + "----------------------------\r\n\r\n");

                    if (!ExpressUtil.HandleProecssInfo(ProxyIp))
                    {
                        NeedChangeIP = true;
                    }

                    DateTime end = DateTime.Now;
                    ExecuteCount++;
                    TaskLog.ExpressProgressLogInfo.WriteLogE("\r\n\r\n------------------快递进度信息任务完成:" + end.ToString("yyyy-MM-dd HH:mm:ss") + ",本次共耗时(分):" + (end - start).TotalMinutes + " END------------------------\r\n\r\n\r\n\r\n");

                }
            }
            catch (Exception ex)
            {
                JobExecutionException e2 = new JobExecutionException(ex);
                TaskLog.ExpressProgressLogError.WriteLogE("快递进度信息任务异常", ex);
                ExecuteCount++;
                //1.立即重新执行任务 
                e2.RefireImmediately = true;
            }
        }
    }
}
