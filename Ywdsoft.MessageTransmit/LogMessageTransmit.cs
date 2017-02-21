using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using Ywdsoft.Model.RabbitMQ;
using Ywdsoft.Utility;
using Ywdsoft.Utility.RabbitMQ;

namespace Ywdsoft.MessageTransfers
{
    [Export(typeof(MessageTransmitAction))]
    public class LogMessageTransmit : MessageTransmitAction
    {
        private static List<string> listType = new List<string> { "LogTest" };

        public override List<string> MessageDataType
        {
            get
            {
                return listType;
            }
        }

        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="message"></param>
        public override void Action(string message)
        {
            var mqMessage = JsonConvert.DeserializeObject<MQMessage<LogMessage>>(message);
            //模拟进行业务处理
            foreach (var item in mqMessage.Data)
            {
                //item.Text
            }
            //延时1s
            Thread.Sleep(1000);
            LogHelper.WriteLog("接收到MQ日志消息，消息内容：" + message);
        }
    }
}
