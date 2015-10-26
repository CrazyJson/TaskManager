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
    /// 发送消息任务
    /// </summary>
    ///<remarks>DisallowConcurrentExecution属性标记任务不可并行，要是上一任务没运行完即使到了运行时间也不会运行</remarks>
    [DisallowConcurrentExecution]
    public class SendMessageJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                DateTime start = DateTime.Now;
                TaskLog.SendMessageLogInfo.WriteLogE("\r\n\r\n\r\n\r\n------------------发送信息任务开始执行 " + start.ToString("yyyy-MM-dd HH:mm:ss") + " BEGIN-----------------------------\r\n\r\n");

                //取出所有当前待发送的消息
                List<Message> listWait = SQLHelper.ToList<Message>(strSQL2);
                bool isSucess = false;
                if (listWait == null || listWait.Count == 0)
                {
                    TaskLog.SendMessageLogInfo.WriteLogE("当前没有等待发送的消息!");
                }
                else
                {
                    foreach (var item in listWait)
                    {
                        isSucess = MessageHelper.SendMessage(item);
                        TaskLog.SendMessageLogInfo.WriteLogE(string.Format("接收人:{0},类型:{1},内容:“{2}”的消息发送{3}", item.Receiver, item.Type.ToString(), item.Content, isSucess ? "成功" : "失败"));
                    }
                }
                DateTime end = DateTime.Now;
                TaskLog.SendMessageLogInfo.WriteLogE("\r\n\r\n------------------发送信息任务完成:" + end.ToString("yyyy-MM-dd HH:mm:ss") + ",本次共耗时(分):" + (end - start).TotalMinutes + " END------------------------\r\n\r\n\r\n\r\n");
            }
            catch (Exception ex)
            {
                JobExecutionException e2 = new JobExecutionException(ex);
                TaskLog.SendMessageLogError.WriteLogE("发送信息任务异常", ex);
                //1.立即重新执行任务 
                e2.RefireImmediately = true;
            }
        }

        /// <summary>
        /// 取带发送消息(与历史表进行对比,超过3分钟最长的第一条)
        /// </summary>
        private static readonly string strSQL = @"
                SELECT MessageGuid,Receiver,Content,Subject,Type,CreatedOn FROM (       
	                SELECT *,ROW_NUMBER() OVER ( PARTITION BY Receiver ORDER BY Interval DESC ) AS RowNum FROM 
	                (
		                SELECT A.*,DATEDIFF(MINUTE,ISNULL(B.SendOn,'1900-01-01'),A.CreatedOn) AS Interval FROM dbo.p_Message AS A
		                LEFT JOIN 
		                (       
			                SELECT  
				                Receiver ,SendOn,
				                ROW_NUMBER() OVER ( PARTITION BY Receiver ORDER BY SendOn DESC ) AS Num
			                FROM    dbo.p_MessageHistory 
		                )AS B
		                ON A.Receiver = B.Receiver AND B.Num=1
	                )AS C
                )AS D
                WHERE D.RowNum=1";

        /// <summary>
        /// 取带发送消息(与数据库时间进行对比,超过3分钟最长的第一条)
        /// </summary>
        private static readonly string strSQL1 = @"
            SELECT MessageGuid,Receiver,Content,Subject,Type,CreatedOn FROM (
	            SELECT  * ,
			            ROW_NUMBER() OVER ( PARTITION BY Receiver ORDER BY CreatedOn DESC ) AS RowNum
	            FROM    dbo.p_Message
	            WHERE   DATEDIFF(MINUTE, CreatedOn, GETDATE()) > 3
            )AS A
            WHERE A.RowNum=1";

        /// <summary>
        /// 取出p_Message表里面所有数据进行发送
        /// </summary>
        private static readonly string strSQL2 = @"SELECT MessageGuid,Receiver,Content,Subject,Type,CreatedOn FROM dbo.p_Message ";
    }
}
