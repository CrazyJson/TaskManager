using System;
using System.Collections;
using System.Collections.Generic;

namespace Ywdsoft.Utility
{
    /// <summary>
    /// 任务实体
    /// </summary>
    public class TaskUtil
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public string TaskID { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 运行频率设置
        /// </summary>
        public string CronExpressionString { get; set; }

        /// <summary>
        /// 任务运频率中文说明
        /// </summary>
        public string CronRemark { get; set; }

        /// <summary>
        /// 任务所在DLL对应的程序集名称
        /// </summary>
        public string Assembly { get; set; }

        /// <summary>
        /// 任务所在类
        /// </summary>
        public string Class { get; set; }

        public TaskStatus Status { get; set; }

        /// <summary>
        /// 任务状态中文说明
        /// </summary>
        public string StatusCn
        {
            get
            {
                return Status == TaskStatus.STOP ? "停止" : "运行";
            }
        }

        /// <summary>
        /// 任务创建时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// 任务修改时间
        /// </summary>
        public DateTime? ModifyOn { get; set; }

        /// <summary>
        /// 任务最近运行时间
        /// </summary>
        public DateTime? RecentRunTime { get; set; }

        /// <summary>
        /// 任务下次运行时间
        /// </summary>
        public DateTime? LastRunTime { get; set; }

        /// <summary>
        /// 任务备注
        /// </summary>
        public string Remark { get; set; }

    }

    /// <summary>
    /// 任务状态枚举
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        /// 运行状态
        /// </summary>
        RUN = 0,

        /// <summary>
        /// 停止状态
        /// </summary>
        STOP = 1
    }

    /// <summary>
    /// 任务帮助类
    /// </summary>
    public class TaskHelper
    {
        private static string InsertSQL = @"INSERT INTO p_Task(TaskID,TaskName,CronExpressionString,Assembly,Class,Status,CronRemark,Remark,LastRunTime)
                            VALUES(@TaskID,@TaskName,@CronExpressionString,@Assembly,@Class,@Status,@CronRemark,@Remark,@LastRunTime)";

        private static string UpdateSQL = @"UPDATE p_Task SET TaskName=@TaskName,CronExpressionString=@CronExpressionString,Assembly=@Assembly,
                                Class=@Class,CronRemark=@CronRemark,Remark=@Remark,LastRunTime=@LastRunTime WHERE TaskID=@TaskID";
        /// <summary>
        /// 获取指定id任务数据
        /// </summary>
        /// <param name="TaskID">任务id</param>
        /// <returns>任务数据</returns>
        public static TaskUtil GetById(string TaskID)
        {
            return SQLHelper.Single<TaskUtil>("SELECT * FROM p_Task WHERE TaskID=@TaskID", new { TaskID = TaskID });
        }

        public static void RunById(string TaskID)
        {
            QuartzHelper.RunOnceTask(TaskID);
        }

        /// <summary>
        /// 删除指定id任务
        /// </summary>
        /// <param name="TaskID">任务id</param>
        public static void DeleteById(string TaskID)
        {
            QuartzHelper.DeleteJob(TaskID);
            SQLHelper.ExecuteNonQuery("DELETE FROM p_Task WHERE TaskID=@TaskID", new { TaskID = TaskID });
        }

        /// <summary>
        /// 更新任务运行状态
        /// </summary>
        /// <param name="TaskID">任务id</param>
        /// <param name="Status">任务状态</param>
        public static void UpdateTaskStatus(string TaskID, TaskStatus Status)
        {
            if (Status == TaskStatus.RUN)
            {
                QuartzHelper.ResumeJob(TaskID);
            }
            else
            {
                QuartzHelper.PauseJob(TaskID);
            }
            SQLHelper.ExecuteNonQuery("UPDATE p_Task SET Status=@Status WHERE TaskID=@TaskID", new { TaskID = TaskID, Status = Status });
        }

        /// <summary>
        /// 更新任务下次运行时间
        /// </summary>
        /// <param name="TaskID">任务id</param>
        /// <param name="LastRunTime">下次运行时间</param>
        public static void UpdateLastRunTime(string TaskID, DateTime LastRunTime)
        {
            SQLHelper.ExecuteNonQuery("UPDATE p_Task SET LastRunTime=@LastRunTime WHERE TaskID=@TaskID", new { TaskID = TaskID, LastRunTime = LastRunTime });
        }

        /// <summary>
        /// 更新任务最近运行时间
        /// </summary>
        /// <param name="TaskID">任务id</param>
        public static void UpdateRecentRunTime(string TaskID, DateTime LastRunTime)
        {
            SQLHelper.ExecuteNonQuery("UPDATE p_Task SET RecentRunTime=CURRENT_TIMESTAMP,LastRunTime=@LastRunTime WHERE TaskID=@TaskID", new { TaskID = TaskID, LastRunTime = LastRunTime });
        }

        /// <summary>
        /// 获取所有启用的任务
        /// </summary>
        /// <returns>所有启用的任务</returns>
        public static List<TaskUtil> ReadConfig()
        {
            return SQLHelper.ToList<TaskUtil>("SELECT * FROM p_Task");
        }

        /// <summary>
        /// 根据条件查询任务
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns>符合条件的任务</returns>
        public static ApiResult<List<TaskUtil>> Query(QueryCondition condition)
        {
            ApiResult<List<TaskUtil>> result = new ApiResult<List<TaskUtil>>();
            if (string.IsNullOrEmpty(condition.SortField))
            {
                condition.SortField = "CreatedOn";
                condition.SortOrder = "DESC";
            }
            Hashtable ht = Pagination.QueryBase<TaskUtil>("SELECT * FROM p_Task", condition);
            result.Result = ht["data"] as List<TaskUtil>;
            result.TotalCount = Convert.ToInt32(ht["total"]);
            result.TotalPage = result.CalculateTotalPage(condition.PageSize, result.TotalCount.Value, condition.IsPagination);
            return result;
        }

        /// <summary>
        /// 保存任务
        /// </summary>
        /// <param name="value">任务</param>
        /// <returns>保存结果</returns>
        public static ApiResult<string> SaveTask(TaskUtil value)
        {
            ApiResult<string> result = new ApiResult<string>();
            result.HasError = true;
            if (value == null)
            {
                result.Message = "参数空异常";
                return result;
            }

            #region "校验"
            if (string.IsNullOrEmpty(value.TaskName))
            {
                result.Message = "任务名称不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(value.Assembly))
            {
                result.Message = "程序集名称不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(value.CronExpressionString))
            {
                result.Message = "Cron表达式不能为空";
                return result;
            }
            if (!QuartzHelper.ValidExpression(value.CronExpressionString))
            {
                result.Message = "Cron表达式格式不正确";
                return result;
            }
            if (string.IsNullOrEmpty(value.CronRemark))
            {
                result.Message = "表达式说明不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(value.Class))
            {
                result.Message = "类名不能为空";
                return result;
            }
            #endregion

            ApiResult<DateTime> cronResult = null;
            try
            {
                //新增
                if (string.IsNullOrEmpty(value.TaskID))
                {
                    value.TaskID = Guid.NewGuid().ToString("N");
                    //任务状态处理

                    cronResult = GetTaskeLastRunTime(value.CronExpressionString);
                    if (cronResult.HasError)
                    {
                        result.Message = cronResult.Message;
                        return result;
                    }
                    else
                    {
                        value.LastRunTime = cronResult.Result;
                    }
                    //添加新任务
                    QuartzHelper.ScheduleJob(value);

                    SQLHelper.ExecuteNonQuery(InsertSQL, value);
                }
                else
                {
                    value.ModifyOn = DateTime.Now;
                    TaskUtil srcTask = GetById(value.TaskID.ToString());

                    //表达式改变了重新计算下次运行时间
                    if (!value.CronExpressionString.Equals(srcTask.CronExpressionString, StringComparison.OrdinalIgnoreCase))
                    {
                        cronResult = GetTaskeLastRunTime(value.CronExpressionString);
                        if (cronResult.HasError)
                        {
                            result.Message = cronResult.Message;
                            return result;
                        }
                        else
                        {
                            value.LastRunTime = cronResult.Result;
                        }

                        //更新任务
                        QuartzHelper.ScheduleJob(value, true);
                    }
                    else
                    {
                        value.LastRunTime = srcTask.LastRunTime;
                    }

                    SQLHelper.ExecuteNonQuery(UpdateSQL, value);
                }

                result.HasError = false;
                result.Result = value.TaskID.ToString();
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 计算任务下次运行时间
        /// </summary>
        /// <param name="CronExpressionString"></param>
        /// <returns>下次运行时间</returns>
        private static ApiResult<DateTime> GetTaskeLastRunTime(string CronExpressionString)
        {
            ApiResult<DateTime> result = new ApiResult<DateTime>();
            try
            {
                //计算下次任务运行时间
                result.Result = QuartzHelper.GetTaskeFireTime(CronExpressionString, 1)[0];
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = "任务Cron表达式设置错误";
                LogHelper.WriteLog("任务Cron表达式设置错误", ex);
            }
            return result;
        }
    }
}
