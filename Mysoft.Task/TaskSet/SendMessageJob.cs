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
    public class SendMessageJob : IJob
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
                    DateTime start = DateTime.Now;
                    LogHelper.WriteLog("\r\n\r\n\r\n\r\n------------------发送信息任务开始执行 " + start.ToString("yyyy-MM-dd HH:mm:ss") + " BEGIN-----------------------------\r\n\r\n");

                    //取出所有当前待发送的消息
                    List<Message> listWait = SQLHelper.ToList<Message>(strSQL);
                    bool isSucess = false;
                    if (listWait == null || listWait.Count == 0)
                    {
                        LogHelper.WriteLog("当前没有等待发送的消息!");
                    }
                    else
                    {
                        foreach (var item in listWait)
                        {
                            isSucess = MessageHelper.SendMessage(item);
                            LogHelper.WriteLog(string.Format("接收人:{0},类型:{1},内容:“{2}”的消息发送{3}", item.Receiver, item.Type.ToString(), item.Content, isSucess ? "成功" : "失败"));
                        }
                    }
                    DateTime end = DateTime.Now;
                    LogHelper.WriteLog("\r\n\r\n------------------发送信息任务完成:" + end.ToString("yyyy-MM-dd HH:mm:ss") + ",本次共耗时(分):" + (end - start).TotalMinutes + " END------------------------\r\n\r\n\r\n\r\n");
                    isRun = false;
                }
            }
            catch (Exception ex)
            {
                JobExecutionException e2 = new JobExecutionException(ex);
                LogHelper.WriteLog("发送信息任务异常", ex);
                isRun = false;
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
    }
}
