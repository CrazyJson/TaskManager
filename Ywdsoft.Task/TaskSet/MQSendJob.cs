using Ywdsoft.Utility;
using Quartz;
using System;
using Ywdsoft.Utility.RabbitMQ;
using System.Collections.Generic;
using Ywdsoft.Model.RabbitMQ;

namespace Ywdsoft.Task.TaskSet
{
    /// <summary>
    ///  
    /// </summary>
    ///<remarks>DisallowConcurrentExecution属性标记任务不可并行，要是上一任务没运行完即使到了运行时间也不会运行</remarks>
    [DisallowConcurrentExecution]
    public class MQSendJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                MQMessage<LogMessage> messageList = new MQMessage<LogMessage>();
                messageList.DataType = "LogTest";
                messageList.Data = new List<LogMessage>();
                messageList.Data.Add(new LogMessage
                {
                    ChangeType = ChangeType.Insert,
                    Id = Guid.NewGuid().ToString("N"),
                    Text = "我是测试日志消息，报告当前时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
                RabbitMQClient.SendMessage(messageList);
            }
            catch (Exception ex)
            {
                JobExecutionException e2 = new JobExecutionException(ex);
                LogHelper.WriteLog("消息队列MQ测试任务异常", ex);
                //1.立即重新执行任务 
                e2.RefireImmediately = true;
            }
        }
    }
}
