using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using CsharpHttpHelper;
using Newtonsoft.Json.Linq;

namespace Mysoft.Utility
{
    /// <summary>
    /// 消息工具类
    /// </summary>
    public class MessageHelper
    {
        /// <summary>
        /// 新增一条消息提醒
        /// </summary>
        /// <param name="Receiver">接收人（支持多个接收者逗号分隔）</param>
        /// <param name="Content">内容</param>
        /// <param name="Subject">邮件主题</param>
        /// <param name="Type">消息类型</param>
        /// <param name="FromType">消息来源类型</param>
        /// <param name="FkGUID">消息来源GUID</param>
        public static int AddMessage(string Receiver, string Content, string Subject,string FromType,Guid FkGUID, MessageType Type = MessageType.SMS)
        {
            if (string.IsNullOrEmpty("Receiver") || string.IsNullOrEmpty("Content"))
            {
                throw new ArgumentNullException("参数空异常");
            }
            //多个接收者逗号分隔
            string[] Receivers = Receiver.Split(new char[] { ',' });
            StringBuilder sb = new StringBuilder();
            foreach (var item in Receivers)
            {
                sb.AppendFormat(@"INSERT INTO dbo.p_Message(Receiver,Content,Subject,Type,FromType,FkGUID) SELECT '{0}',@Content,@Subject,@Type,@FromType,@FkGUID;", item);
            }
            return SQLHelper.ExecuteNonQuery(sb.ToString(), new { Content = Content, Subject = Subject, FromType = FromType, FkGUID = FkGUID,Type = EnumHelper.EnumToInt<MessageType>(Type) });
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="MessageGuid">消息GUID</param>
        /// <returns>状态</returns>
        public static bool SendMessage(Guid MessageGuid)
        {
            Message message = GetMessageById(MessageGuid);
            return SendMessage(message);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>状态</returns>
        public static bool SendMessage(Message message, int count = 1)
        {
            if (message == null)
            {
                throw new ArgumentNullException("参数message空异常");
            }
            bool isSuccess = false;
            SMSCode code = SMSCode.Exception;
            switch (message.Type)
            {
                //短信
                case MessageType.SMS:
                    code = SmsHelper.SendMessage(message.Receiver, message.Content);
                    break;
                case MessageType.EMAIL:
                    code = MailHelper.SendMessage(message.Receiver, message.Subject, message.Content);
                    break;
            }
            isSuccess = (code == SMSCode.Success);
            while (!isSuccess && count < 3)
            {
                //如果第一次没有发送成功再重新发送一遍
                count++;
                isSuccess = SendMessage(message, count);
            }
            RemoveMessage(message.MessageGuid, isSuccess ? "" :message.Type.GetDescription()+"重复发送三次失败,失败信息:" + code.GetDescription());
            return isSuccess;
        }

        /// <summary>
        /// 将消息移到发送历史表
        /// </summary>
        /// <param name="MessageGuid">消息GUID</param>
        /// <param name="isSuccess">是否发送成功</param>
        public static void RemoveMessage(Guid MessageGuid, string remark)
        {
            int Staue = string.IsNullOrEmpty(remark) ? 0 : 1;
            string strSQL = @"INSERT INTO dbo.p_MessageHistory(MessageGuid,Receiver,Type,Content,Subject,FromType,FkGUID,CreatedOn,Staue,Remark)
            SELECT  MessageGuid,Receiver,Type,Content,Subject,FromType,FkGUID,CreatedOn,@Staue,@Remark FROM dbo.p_Message WHERE MessageGuid=@MessageGuid;
            DELETE FROM dbo.p_Message WHERE MessageGuid=@MessageGuid;";
            SQLHelper.ExecuteNonQuery(strSQL, new { MessageGuid = MessageGuid, Staue = Staue, Remark = remark });
        }

        /// <summary>
        /// 通过消息GUID获取消息数据
        /// </summary>
        /// <param name="MessageGuid">消息GUID</param>
        /// <returns>消息</returns>
        public static Message GetMessageById(Guid MessageGuid)
        {
            return SQLHelper.Single<Message>("SELECT * FROM dbo.p_Message WHERE MessageGuid=@MessageGuid", new { MessageGuid = MessageGuid });
        }
    }

    /// <summary>
    /// 消息
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 消息GUID
        /// </summary>
        public Guid MessageGuid { get; set; }

        /// <summary>
        /// 消息接收人
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// 邮件主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        /// 消息创建日期
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// 消息来源类型
        /// </summary>
        public string FromType { get; set; }

        /// <summary>
        /// 消息来源GUID
        /// </summary>
        public Guid FkGUID { get; set; }
    }

    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 短信消息
        /// </summary>
        [Description("短信消息")]
        SMS = 0,
        /// <summary>
        /// 邮件消息
        /// </summary>
        [Description("邮件消息")]
        EMAIL = 1
    }
}
