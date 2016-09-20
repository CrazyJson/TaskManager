using Ywdsoft.Utility;
using Quartz;
using System;

namespace Ywdsoft.Task.TaskSet
{
    /// <summary>
    /// 测试任务
    /// </summary>
    ///<remarks>DisallowConcurrentExecution属性标记任务不可并行，要是上一任务没运行完即使到了运行时间也不会运行</remarks>
    [DisallowConcurrentExecution]
    public class TestJob : IInterruptableJob
    {
        //信号量 解决任务 while无法停止任务
        private bool _interrupted = true;

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                while (!_interrupted && true)
                {
                    LogHelper.WriteLog("测试任务,当前系统时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }

            }
            catch (Exception ex)
            {
                JobExecutionException e2 = new JobExecutionException(ex);
                LogHelper.WriteLog("测试任务异常", ex);
                //1.立即重新执行任务 
                e2.RefireImmediately = true;
                //2 立即停止所有相关这个任务的触发器
                //e2.UnscheduleAllTriggers=true; 
            }
        }

        public void Interrupt()
        {
            _interrupted = true;
        }
    }
}
