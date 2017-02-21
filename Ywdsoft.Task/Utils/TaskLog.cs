using Ywdsoft.Utility;

namespace Ywdsoft.Task.Utils
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
    }
}
