using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Ywdsoft.Model.RabbitMQ;
using Ywdsoft.Utility.ConfigHandler;
using Ywdsoft.Utility.Mef;

namespace Ywdsoft.Utility.RabbitMQ
{
    /// <summary>
    /// 全局RabbitMQ消息处理
    /// </summary>
    public class RabbitMQClient : IDisposable
    {
        /// <summary>
        /// RabbitMQ连接对象
        /// </summary>
        public static IConnection RbConnection = null;

        /// <summary>
        /// 初始化MQ连接 和消息接收
        /// </summary>
        public static void InitClient()
        {
            try
            {
                InitMQConnection();

                //接收消息
                MessageTransmit messageTransmit = MefConfig.TryResolve<MessageTransmit>();
                if (messageTransmit.Actions != null && messageTransmit.Actions.Count() > 0)
                {
                    ReciveMessage((routingKey, message) =>
                    {
                        //消息分发处理
                        messageTransmit.Transmit(message, routingKey);
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("RabbitMQ连接异常,可能由于主机IP用户名密码相关信息配置错误", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 初始化MQ连接
        /// </summary>
        private static void InitMQConnection()
        {
            __loop:
            if (RbConnection == null || !RbConnection.IsOpen)
            {
                ConnectionFactory FactoryFactory = new ConnectionFactory();
                FactoryFactory.HostName = RabbitMQConfig.Host;
                FactoryFactory.UserName = RabbitMQConfig.UserName;
                FactoryFactory.Password = RabbitMQConfig.Password;
                FactoryFactory.Port = RabbitMQConfig.Port;
                try
                {
                    RbConnection = FactoryFactory.CreateConnection();
                }
                catch(Exception ex)
                {
                    LogHelper.WriteLog("MQ初始化连接异常", ex);
                    // 休眠3秒后再启动连接
                    Random r = new Random();
                    Thread.Sleep(r.Next(100, 5000));
                    goto __loop;
                }
            }
        }

        #region  "接收MQ消息"
        /// <summary>
        /// 开启线程,接受MQ消息
        /// </summary>
        /// <param name="callback">回调处理事件</param>
        private static void ReciveMessage(Action<string, string> callback)
        {
            var Thread = new Thread(new ThreadStart(delegate ()
            {
                Dequeue(callback);
            }));
            Thread.Start();
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="callback"></param>
        protected static void Dequeue(Action<string, string> callback)
        {
            __loop:
            try
            {
                using (var channel = RbConnection.CreateModel())
                {
                    channel.QueueDeclare(RabbitMQConfig.QueueName, RabbitMQConfig.Durable, false, false, new Dictionary<string, object>
                    {
                        { "x-queue-mode","lazy"}
                    });
                    //输入1，那如果接收一个消息，但是没有应答，则客户端不会收到下一个消息
                    channel.BasicQos(0, 1, false);

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(RabbitMQConfig.QueueName, !RabbitMQConfig.Ack, consumer);

                    // 发生错误或者异常时,是否重新连接
                    bool reconnect = false;
                    //消息主体
                    string message = string.Empty;
                    while (true)
                    {
                        try
                        {
                            var ea = consumer.Queue.Dequeue();//接收消息并出列    
                            message = Encoding.UTF8.GetString(ea.Body);
                            callback(ea.RoutingKey, message);
                            //回复确认
                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                        catch (Exception ex)
                        {
                            if (ex is EndOfStreamException)
                            {
                                LogHelper.WriteLog("RabbitMQ消息接收异常", ex);
                                // 发生异常后,跳出,并准备重新连接.
                                reconnect = true;
                                break;
                            }
                            else
                            {
                                LogHelper.WriteLog("RabbitMQ消息分发处理事件异常,消息内容:" + message, ex);
                            }
                        }
                    }

                    if (reconnect)
                    {                      
                        // 休眠3秒后再启动连接
                        Random r = new Random();
                        Thread.Sleep(r.Next(100, 5000));
                        InitMQConnection();
                        goto __loop;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("RabbitMQ连接异常,可能由于主机IP用户名密码相关信息配置错误", ex);
                // 休眠3秒后再启动连接
                Random r = new Random();
                Thread.Sleep(r.Next(100, 5000));
                InitMQConnection();
                goto __loop;
            }
        }

        #endregion

        #region "发送消息接口"
        /// <summary>
        /// 异步发送消息到MQ
        /// </summary>
        /// <param name="message">消息</param>
        public static void SendMessage(string message)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                if (!string.IsNullOrEmpty(message))
                {
                    try
                    {
                        InitMQConnection();
                        using (var channel = RbConnection.CreateModel())
                        {
                            channel.QueueDeclare(RabbitMQConfig.QueueName, RabbitMQConfig.Durable, false, false, new Dictionary<string, object>
                            {
                                { "x-queue-mode","lazy"}
                            });
                            //消息内容主体
                            byte[] bytes = Encoding.UTF8.GetBytes(message);
                            //设置消息持久化
                            IBasicProperties properties = channel.CreateBasicProperties();
                            properties.DeliveryMode = 2;
                            channel.BasicPublish("", RabbitMQConfig.QueueName, properties, bytes);
                            LogHelper.WriteLog(string.Format("RabbitMQ发送消息,消息内容如下:{0}", message));
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog("RabbitMQ发送消息异常", ex);
                    }
                }
            });
        }

        /// <summary>
        /// 异步发送消息到MQ
        /// </summary>
        /// <typeparam name="T">泛型消息</typeparam>
        /// <param name="message">消息</param>
        public static void SendMessage<T>(MQMessage<T> message) where T : BaseMessage
        {
            if (message.Data == null || message.Data.Count == 0)
            {
                return;
            }
            SendMessage(JsonConvert.SerializeObject(message));
        }


        /// <summary>
        /// MQ消息发送重载方法,只需传递业务类型 ，路由键 和业务数据ID即可
        /// </summary>
        /// <param name="ListBusiness">业务数据</param>
        public static void SendMessage(Dictionary<string, string> ListBusiness)
        {
            //设备信息变更MQ消息发送通知
            MQMessage<BaseMessage> message = new MQMessage<BaseMessage>();
            message.Data = new List<BaseMessage>();
            foreach (var key in ListBusiness.Keys)
            {
                message.Data.Add(new BaseMessage { Id = key, ChangeType = ListBusiness[key] });
            }
            SendMessage(message);
        }

        #endregion

        /// <summary>
        /// 释放连接资源
        /// </summary>
        public void Dispose()
        {
            if (RbConnection != null)
            {
                RbConnection.Dispose();
                RbConnection = null;
            }
        }
    }
}
