using Mysoft.Utility;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mysoft.Task.TaskSet
{
    /// <summary>
    /// 动态读取TaskConfig.xml配置文件，看是否修改了配置文件(新增任务，修改任务，删除任务)
    /// 来动态更改当前运行的任务信息,解决只能停止Windows服务才能添加新任务问题
    /// </summary>
    public class ConfigJob : IJob
    {
        /// <summary>
        ///任务是否正在执行标记 ：false--未执行； true--正在执行； 默认未执行
        /// </summary>
        private static bool isRun = false;

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (!isRun)
                {
                    isRun = true;
                    LogHelper.WriteLog("Job修改任务开始,当前系统时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    LogHelper.WriteLog("Job修改任务执行前,系统当前的所有任务信息:" + JsonConvert.SerializeObject(TaskHelper.CurrentTaskList, Formatting.Indented));
                    ///获取所有执行中的任务
                    List<TaskUtil> listTask = TaskHelper.ReadConfig().Where(e => e.IsExcute).ToList<TaskUtil>();
                    //开始对比当前配置文件和上一次配置文件之间的改变

                    //1.修改的任务
                    var UpdateJobList = (from p in listTask
                                         from q in TaskHelper.CurrentTaskList
                                         where p.TaskID == q.TaskID && (p.TaskParam != q.TaskParam || p.Assembly != q.Assembly || p.Class != q.Class ||
                                            p.CronExpressionString != q.CronExpressionString
                                         )
                                         select new { NewTaskUtil = p, OriginTaskUtil = q }).ToList();
                    foreach (var item in UpdateJobList)
                    {
                        try
                        {
                            QuartzHelper.ScheduleJob(item.NewTaskUtil);
                            //修改原有的任务
                            int index = TaskHelper.CurrentTaskList.IndexOf(item.OriginTaskUtil);
                            TaskHelper.CurrentTaskList[index] = item.NewTaskUtil;

                        }
                        catch (Exception e)
                        {
                            LogHelper.WriteLog(string.Format("任务“{0}”配置信息更新失败！", item.NewTaskUtil.TaskName), e);
                        }
                    }

                    //2.新增的任务(TaskID在原集合不存在)
                    var AddJobList = (from p in listTask
                                      where !(from q in TaskHelper.CurrentTaskList select q.TaskID).Contains(p.TaskID)
                                      select p).ToList();

                    foreach (var taskUtil in AddJobList)
                    {
                        try
                        {
                            QuartzHelper.ScheduleJob(taskUtil);
                            //添加新增的任务
                            TaskHelper.CurrentTaskList.Add(taskUtil);
                        }
                        catch (Exception e)
                        {
                            LogHelper.WriteLog(string.Format("任务“{0}”新增失败！", taskUtil.TaskName), e);
                        }
                    }

                    //3.删除的任务
                    var DeleteJobList = (from p in TaskHelper.CurrentTaskList
                                         where !(from q in listTask select q.TaskID).Contains(p.TaskID)
                                         select p).ToList();
                    foreach (var taskUtil in DeleteJobList)
                    {
                        try
                        {
                            QuartzHelper.DeleteJob(taskUtil.TaskID);
                            //添加新增的任务
                            TaskHelper.CurrentTaskList.Remove(taskUtil);
                        }
                        catch (Exception e)
                        {
                            LogHelper.WriteLog(string.Format("任务“{0}”删除失败！", taskUtil.TaskName), e);
                        }
                    }
                    LogHelper.WriteLog("Job修改任务执行完成后,系统当前的所有任务信息:" + JsonConvert.SerializeObject(TaskHelper.CurrentTaskList, Formatting.Indented));
                    isRun = false;
                }
            }
            catch (Exception ex)
            {
                JobExecutionException e2 = new JobExecutionException(ex);
                LogHelper.WriteLog("Job修改任务异常", ex);
                isRun = false;
                //1.立即重新执行任务 
                e2.RefireImmediately = true;
            }
        }
    }
}
