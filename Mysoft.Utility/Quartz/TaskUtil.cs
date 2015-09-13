using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Mysoft.Utility
{
    /// <summary>
    /// 任务实体
    /// </summary>
    public class TaskUtil
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public string TaskID { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 任务执行参数
        /// </summary>
        public string TaskParam { get; set; }

        /// <summary>
        /// 运行频率设置
        /// </summary>
        public string CronExpressionString { get; set; }

        /// <summary>
        /// 任务所在DLL对应的程序集名称
        /// </summary>
        public string Assembly { get; set; }

        /// <summary>
        /// 任务所在类
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// 任务是否启动
        /// </summary>
        public bool IsExcute { get; set; }
    }

    /// <summary>
    /// 任务帮助类
    /// </summary>
    public class TaskHelper
    {
        /// <summary>
        /// 配置文件路径
        /// </summary>
        private static readonly string configPath = FileHelper.GetAbsolutePath(@"\TaskConfig.xml");

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <returns></returns>
        public static List<TaskUtil> ReadConfig()
        {
            List<TaskUtil> list = new List<TaskUtil>();
            TaskUtil taskUtil = null;
            string text = string.Empty;
            XmlDocument dom = new XmlDocument();
            dom.Load(configPath);
            XmlNodeList nodeList = dom.SelectNodes("//TaskSet/Task");
            foreach (XmlNode node in nodeList)
            {
                taskUtil = new TaskUtil();
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    text = childNode.InnerText;
                    switch (childNode.Name)
                    {
                        case "TaskID":
                            taskUtil.TaskID = text;
                            break;
                        case "TaskName":
                            taskUtil.TaskName = text;
                            break;
                        case "TaskParam":
                            taskUtil.TaskParam = text;
                            break;
                        case "CronExpressionString":
                            taskUtil.CronExpressionString = text;
                            break;
                        case "Assembly":
                            taskUtil.Assembly = text;
                            break;
                        case "Class":
                            taskUtil.Class = text;
                            break;
                        case "IsExcute":
                            taskUtil.IsExcute = Convert.ToBoolean(text);
                            break;
                        default: break;
                    }
                }
                list.Add(taskUtil);
            }
            return list;
        }

        /// <summary>
        /// 系统当前运行的任务集合
        /// </summary>
        public static List<TaskUtil> CurrentTaskList = new List<TaskUtil>();
    }
}
