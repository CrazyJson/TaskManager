using System;
using System.Linq;
using Newtonsoft.Json;
using Ywdsoft.Utility.ConfigHandler;

namespace Ywdsoft.Utility
{
    /// <summary>
    /// 使用SMTP发送邮件,需要添加发送人邮件 用户名密码进行认证才能发送成功
    /// </summary>
    public class MailHelper
    {
        /// <summary>
        /// SMTP客户端实例
        /// </summary>
        private static System.Net.Mail.SmtpClient client = null;

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="Receiver">邮件接收人</param>
        /// <param name="Subject">邮件主题</param>
        /// <param name="content">邮件内容</param>
        /// <returns>发送状态</returns>
        public static SMSCode SendMessage(string Receiver, string Subject, string content)
        {

            if (string.IsNullOrEmpty(Receiver) || string.IsNullOrEmpty(Subject)
                || string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException("SendMessage参数空异常！");
            }
            if (string.IsNullOrEmpty(EmailConfig.Host) || string.IsNullOrEmpty(EmailConfig.SendMail) ||
                string.IsNullOrEmpty(EmailConfig.UserName) || string.IsNullOrEmpty(EmailConfig.Password))
            {
                throw new ArgumentNullException("Email配置信息错误,相关信息为空！");
            }

            if (client == null)
            {
                try
                {
                    client = new System.Net.Mail.SmtpClient();
                    client.Host = EmailConfig.Host;
                    client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = true;
                    client.Credentials = new System.Net.NetworkCredential(EmailConfig.UserName, EmailConfig.Password);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            try
            {
                System.Net.Mail.MailMessage Message = new System.Net.Mail.MailMessage();
                Message.SubjectEncoding = System.Text.Encoding.UTF8;
                Message.BodyEncoding = System.Text.Encoding.UTF8;
                Message.Priority = System.Net.Mail.MailPriority.High;

                Message.From = new System.Net.Mail.MailAddress(EmailConfig.SendMail, EmailConfig.DisplayName);
                //添加邮件接收人地址
                string[] receivers = Receiver.Split(new char[] { ',' });
                Array.ForEach(receivers.ToArray(), ToMail => { Message.To.Add(ToMail); });

                Message.Subject = Subject;
                Message.Body = content;

                Message.IsBodyHtml = true;
                client.Send(Message);
                return SMSCode.Success;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("邮件发送失败", ex);
                return SMSCode.Exception;
            }
        }
    }
}
