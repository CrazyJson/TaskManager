/*
 * 模块名: 主页模块
 * 描述: 主页系统框架
 * 作者: 杜冬军
 * 创建日期: 2016/2/23 8:50:22 
 * 博客地址：http://yanweidie.cnblogs.com
 */
using Ywdsoft.Utility;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ywdsoft.Modules
{
    public class TaskModule : NancyModule
    {
        public TaskModule() : base("Task")
        {
            //任务列表
            Get["/Grid"] = r =>
            {
                return View["Grid"];
            };
            //任务编辑界面
            Get["/Edit"] = r =>
            {
                return View["Edit"];
            };

            //取数接口API
            #region
            Get["/GetById/{Id}"] = r =>
            {
                JsonBaseModel<TaskUtil> result = new JsonBaseModel<TaskUtil>();
                try
                {
                    //取出单条记录数据
                    string TaskId = r.Id;
                    result.Result = TaskHelper.GetById(TaskId);
                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.Message = ex.Message;
                }
                return Response.AsJson(result);
            };

            //列表查询接口
            Post["/PostQuery"] = r =>
            {
                QueryCondition condition = this.Bind<QueryCondition>();
                return Response.AsJson(TaskHelper.Query(condition));
            };

            //保存数据
            Post["/"] = r =>
            {
                TaskUtil TaskUtil = this.Bind<TaskUtil>();
                return Response.AsJson(TaskHelper.SaveTask(TaskUtil));
            };
            //更新数据
            Put["/"] = r =>
            {
                TaskUtil TaskUtil = this.Bind<TaskUtil>();
                return Response.AsJson(TaskHelper.SaveTask(TaskUtil));
            };
            //删除任务接口
            Delete["/{Id}"] = r =>
            {
                JsonBaseModel<string> result = new JsonBaseModel<string>();
                try
                {
                    string TaskId = r.Id;
                    TaskHelper.DeleteById(TaskId);
                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.Message = ex.Message;
                }
                return Response.AsJson(result);
            };

            //更新任务运行状态
            Put["/{Id}/{Status:int}"] = r =>
            {
                JsonBaseModel<string> result = new JsonBaseModel<string>();
                try
                {
                    TaskStatus Status = Enum.ToObject(typeof(TaskStatus), r.Status);
                    string TaskId = r.Id;
                    TaskHelper.UpdateTaskStatus(TaskId, Status);
                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.Message = ex.Message;
                }
                return Response.AsJson(result);
            };
            #endregion
        }
    }
}