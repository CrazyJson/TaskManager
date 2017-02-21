using System.Collections.Generic;

namespace Ywdsoft.Utility.RabbitMQ
{
    public abstract class MessageTransmitAction
    {
        /// <summary>
        /// 监听多个数据类型
        /// </summary>
        public abstract List<string> MessageDataType
        {
            get;
        }

        /// <summary>
        /// 处理方法
        /// </summary>
        /// <param name="message">消息内容</param>
        public abstract void Action(string message);

        /// <summary>
        /// 获取监听的数据类型
        /// </summary>
        /// <returns></returns>
        public List<string> GetKey()
        {
            return MessageDataType;
        }
    }
}
