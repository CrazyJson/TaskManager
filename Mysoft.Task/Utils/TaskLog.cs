using Mysoft.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mysoft.Task.Utils
{
    /// <summary>
    /// 获取任务记录日志ILog
    /// </summary>
    public class TaskLog
    {
        /// <summary>
        /// 快递进度普通日志
        /// </summary>
        public static LogHelper ExpressProgressLogInfo = new LogHelper("ExpressProgressJob", "info");

        /// <summary>
        /// 快递进度异常日志
        /// </summary>
        public static LogHelper ExpressProgressLogError = new LogHelper("ExpressProgressJob", "error");

        /// <summary>
        /// 代理IP任务普通日志
        /// </summary>
        public static LogHelper IpProxyLogInfo = new LogHelper("IpProxyJob", "info");

        /// <summary>
        /// 代理IP任务异常日志
        /// </summary>
        public static LogHelper IpProxyLogError = new LogHelper("IpProxyJob", "error");

        /// <summary>
        /// 发送消息任务普通日志
        /// </summary>
        public static LogHelper SendMessageLogInfo = new LogHelper("SendMessageJob", "info");

        /// <summary>
        /// 发送消息任务异常日志
        /// </summary>
        public static LogHelper SendMessageLogError = new LogHelper("SendMessageJob", "error");

        /// <summary>
        /// 配置任务普通日志
        /// </summary>
        public static LogHelper ConfigLogInfo = new LogHelper("ConfigJob", "info");

        /// <summary>
        /// 配置任务异常日志
        /// </summary>
        public static LogHelper ConfigLogError = new LogHelper("ConfigJob", "error");
    }
}
