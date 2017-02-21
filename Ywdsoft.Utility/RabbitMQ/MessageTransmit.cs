using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Ywdsoft.Utility.RabbitMQ
{
    [Export]
    public class MessageTransmit
    {
        [ImportMany(typeof(MessageTransmitAction))]
        public IEnumerable<MessageTransmitAction> Actions
        {
            get;
            set;
        }

        /// <summary>
        /// 消息分发
        /// </summary>
        /// <param name="message">消息类容</param>
        /// <param name="routingKey">路由键</param>
        public void Transmit(string message, string routingKey)
        {
            JObject rabbitMQMessage = JObject.Parse(message);
            string datatype = rabbitMQMessage["DataType"].ToString();

            foreach (var item in Actions)
            {
                if (item.GetKey().Contains(datatype))
                {
                    try
                    {
                        item.Action(message);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog("Rabbit消息接收处理异常", ex);
                    }
                }
            }
        }
    }
}
