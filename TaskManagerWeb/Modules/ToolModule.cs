/*
 * 模块名: 工具模块
 * 描述: 工具模块
 * 作者: 杜冬军
 * 创建日期: 2016/2/23 8:50:22 
 * 博客地址：http://yanweidie.cnblogs.com
 */

using Nancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Ywdsoft.Utility;
using Ywdsoft.Utility.Command;
using Ywdsoft.Utility.EntityCode;
using Ywdsoft.Utility.Http;
using YwdSoft.Service;

namespace Ywdsoft.Modules
{
    public class ToolModule : BaseModule
    {
        public ToolModule() : base("Tool")
        {
            //数据库表
            Get["/TableList"] = r =>
            {
                ViewBag.TableList = JsonConvert.SerializeObject(TableListService.GetAllTable());
                return View["TableList"];
            };

            //CronExpress表达式生成器
            Get["/CronExpress"] = r =>
            {
                return View["CronExpress"];
            };

            //命令行执行工具
            Get["/CommandLine"] = r =>
            {
                return View["CommandLine"];
            };

            //系统日志
            Get["/SysLog"] = r =>
            {
                return View["SysLog"];
            };

            //异常信息
            Get["/ExceptionLog"] = r =>
            {
                return View["ExceptionLog", LogHelper.GetExceptionMsg()];
            };
            #region "接口"

            //生成实体代码
            Post["/QuickCode"] = r =>
            {
                string strTableList = Request.Form["TableList"];
                if (string.IsNullOrEmpty(strTableList))
                {
                    return null;
                }
                string FileName = string.Format("EntityCode-{0}.zip", DateTime.Now.ToString("yyyyMMddHHmmss"));
                var res = new Response()
                {
                    Contents = stream => { EntityCodeHelper.QuickCode(TableListService.GetTableInfo(strTableList.Split(',')), stream); },
                    ContentType = MimeHelper.GetMineType(FileName),
                    StatusCode = HttpStatusCode.OK,
                    Headers = new Dictionary<string, string> {
                                { "Content-Disposition", string.Format("attachment;filename={0}", System.Web.HttpUtility.UrlPathEncode(FileName)) }
                            }
                };
                return res;
            };

            //计算表达式最近五次运行时间
            Get["/CalcRunTime"] = r =>
            {
                string CronExpressionString = Request.Query["CronExpression"];
                if (string.IsNullOrEmpty(CronExpressionString))
                {
                    return "[]";
                }
                else
                {
                    JsonSerializerSettings setting = new JsonSerializerSettings();
                    setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    return JsonConvert.SerializeObject(QuartzHelper.GetTaskeFireTime(CronExpressionString, 5), setting);
                }
            };

            //获取命令行返回结果
            Post["/Cmd"] = r =>
            {
                string strCmd = Request.Form["cmd"];
                return CommandHelp.ExcuteCommad(strCmd);
            };

            //删除异常消息
            Get["/DeleteException"] = r =>
            {
                string type = Request.Query["Type"];
                string Index = Request.Query["Index"];
                if ("Delete".Equals(type, StringComparison.CurrentCultureIgnoreCase))
                {
                    long key;
                    if(long.TryParse(Index,out key))
                    {
                        LogHelper.RemoveExceptionMsg(key);
                    }
                }
                else if ("Clear".Equals(type, StringComparison.CurrentCultureIgnoreCase))
                {
                    LogHelper.ClearExceptionMsg();
                }
                return string.Empty;
            };
            #endregion
        }
    }
}