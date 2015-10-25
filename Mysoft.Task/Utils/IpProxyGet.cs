using CsharpHttpHelper;
using HtmlAgilityPack;
using Mysoft.Task.Utils;
using Mysoft.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Web;

namespace Mysoft.Task
{
    /// <summary>
    /// 从ip代理站点(http://www.xicidaili.com/nn/6)获取所有代理ip
    /// </summary>
    public static class IpProxyGet
    {
        /// <summary>
        /// CPU数量
        /// </summary>
        private static readonly int CPUCount = Convert.ToInt32(Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS"));

        //创建Httphelper对象
        private static HttpHelper http = new HttpHelper();

        public static List<IPProxy> ParseProxy(ProxyParam Param)
        {
            if (string.IsNullOrEmpty(Param.IPUrl))
            {
                throw new ArgumentNullException("ParseProxy函数参数空异常");
            }

            //总页数
            int total = GetTotalPage(Param.IPUrl, Param.ProxyIp);

            //返回结果
            List<IPProxy> list = new List<IPProxy>();

            //多线程进行解析获取
            List<Thread> listThread = new List<Thread>();

            //每个线程需要解析的页面数量
            int threadPqgeSize = (total / CPUCount) + 1;
            int count = 0;
            //为每个线程准备参数
            List<Hashtable> threadParams = new List<Hashtable>();
            int start, end;
            Hashtable table = null;
            for (int i = 0; i < CPUCount; i++)
            {
                start = i * threadPqgeSize + 1;
                if (i == CPUCount - 1)
                {
                    end = total;
                }
                else
                {
                    end = start + threadPqgeSize;
                }
                table = new Hashtable();
                table.Add("start", start);
                table.Add("end", end);
                table.Add("list", list);
                table.Add("param", Param);
                threadParams.Add(table);

                count += threadPqgeSize;
            }

            for (int i = 1; i < CPUCount; i++)
            {
                Thread thread = new Thread(DoWork);
                thread.IsBackground = true;
                thread.Name = "PageParse #" + i.ToString();
                listThread.Add(thread);
                thread.Start(threadParams[i]);
            }

            // 为当前线程指派生成任务。
            DoWork(threadParams[0]);

            // 等待所有的编译线程执行线束。
            foreach (Thread thread in listThread)
            {
                thread.Join();
            }
            if (list.Count == 0)
            {
                TaskLog.IpProxyLogInfo.WriteLogE("爬虫-代理ip任务", new Exception("没有获取到数据,可能当前ip(" + Param.ProxyIp + ")已被服务器封锁"));
            }
            else
            {
                BatchSaveData(list);
            }
            return list;
        }

        /// <summary>
        /// 解析每一页数据
        /// </summary>
        /// <param name="param"></param>
        private static void DoWork(object param)
        {
            //参数还原
            Hashtable table = param as Hashtable;
            int start = Convert.ToInt32(table["start"]);
            int end = Convert.ToInt32(table["end"]);
            List<IPProxy> list = table["list"] as List<IPProxy>;
            ProxyParam Param = table["param"] as ProxyParam;

            //页面地址
            string url = string.Empty;
            string ip = string.Empty;
            IPProxy item = null;
            HtmlNodeCollection nodes = null;
            HtmlNode node = null;
            HtmlAttribute atr = null;
            for (int i = start; i <= end; i++)
            {
                TaskLog.IpProxyLogInfo.WriteLogE(string.Format("开始解析,页码{0}~{1},当前页码{2}", start, end, i));
                url = string.Format("{0}/{1}", Param.IPUrl, i);
                var doc = new HtmlDocument();
                doc.LoadHtml(GetHTML(url, Param.ProxyIp));
                //获取所有数据节点tr
                var trs = doc.DocumentNode.SelectNodes(@"//table[@id='ip_list']/tr");
                if (trs != null && trs.Count > 1)
                {
                    TaskLog.IpProxyLogInfo.WriteLogE(string.Format("当前页码{0},请求地址{1},共{2}条数据", i, url, trs.Count));
                    for (int j = 1; j < trs.Count; j++)
                    {
                        nodes = trs[j].SelectNodes("td");
                        if (nodes != null && nodes.Count > 9)
                        {
                            ip = nodes[2].InnerText.Trim();
                            if (Param.IsPingIp && !Ping(ip))
                            {
                                continue;
                            }
                            //有效的IP才添加
                            item = new IPProxy();

                            node = nodes[1].FirstChild;
                            if (node != null)
                            {
                                atr = node.Attributes["alt"];
                                if (atr != null)
                                {
                                    item.Country = atr.Value.Trim();
                                }
                            }

                            item.IP = ip;
                            item.Port = nodes[3].InnerText.Trim();
                            item.ProxyIp = GetIP(item.IP, item.Port);
                            item.Position = nodes[4].InnerText.Trim();
                            item.Anonymity = nodes[5].InnerText.Trim();
                            item.Type = nodes[6].InnerText.Trim();

                            node = nodes[7].SelectSingleNode("div[@class='bar']");
                            if (node != null)
                            {
                                atr = node.Attributes["title"];
                                if (atr != null)
                                {
                                    item.Speed = atr.Value.Trim();
                                }
                            }

                            node = nodes[8].SelectSingleNode("div[@class='bar']");
                            if (node != null)
                            {
                                atr = node.Attributes["title"];
                                if (atr != null)
                                {
                                    item.ConnectTime = atr.Value.Trim();
                                }
                            }
                            item.VerifyTime = nodes[9].InnerText.Trim();
                            list.Add(item);
                        }
                    }
                    TaskLog.IpProxyLogInfo.WriteLogE(string.Format("当前页码{0},共{1}条数据", i, trs.Count));
                }
                TaskLog.IpProxyLogInfo.WriteLogE(string.Format("结束解析,页码{0}~{1},当前页码{2}", start, end, i));
            }
        }

        /// <summary>
        /// 获取总页数
        /// </summary>
        /// <returns>总页数</returns>
        private static int GetTotalPage(string IPURL, string ProxyIp)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(GetHTML(IPURL, ProxyIp));
            var res = doc.DocumentNode.SelectNodes(@"//div[@class='pagination']/a");
            if (res != null && res.Count > 2)
            {
                int page;
                if (int.TryParse(res[res.Count - 2].InnerText, out page))
                {
                    return page;
                }
            }
            return 1;
        }

        /// <summary>
        /// 获取页面html内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetHTML(string url, string ProxyIp)
        {
            try
            {
                //创建Httphelper参数对象
                HttpItem item = new HttpItem()
                {
                    URL = url,//URL     必需项    
                    Method = "get",//可选项 默认为Get   
                    ContentType = "text/html",//返回类型    可选项有默认值 
                    ProxyIp = ProxyIp
                };
                //请求的返回值对象
                HttpResult result = http.GetHtml(item);
                return result.Html;
            }
            catch (Exception ex)
            {
                TaskLog.IpProxyLogError.WriteLogE(string.Format("url:{0},ip:{1}获取HTML内容出错", url, ProxyIp), ex);
                return "<HTML></HTML>";
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="list"></param>
        private static void BatchSaveData(List<IPProxy> list)
        {
            try
            {
                SQLHelper.ExecuteNonQuery("Truncate TABLE p_IPProxy");
                SQLHelper.BatchSaveData(list, "p_IPProxy");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>  
        /// 是否能 Ping 通指定的主机  
        /// </summary>  
        /// <param name="ip">ip 地址或主机名或域名</param>  
        /// <returns>true 通，false 不通</returns>  
        private static bool Ping(string ip)
        {
            Ping p = new Ping();
            int timeout = 1000;
            PingReply reply = p.Send(ip, timeout);
            return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
        }

        /// <summary>
        /// 通过IP和端口号组成代理ip
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private static string GetIP(string ip, string port)
        {
            return string.Format("{0}:{1}", ip, port);
        }
        /// <summary>
        /// 获取正确的代理ip
        /// </summary>
        /// <param name="Param">爬取参数</param>
        /// <returns>正确的代理ip</returns>
        public static string GetCorrectIP(ProxyParam Param)
        {
            string ProxyIp = string.Empty;
            string tempProxyIp = string.Empty;
            //当前页
            int currentPage = 1;
            int PageSize = 100;
            while (string.IsNullOrEmpty(ProxyIp))
            {
                //从数据库取
                string strSQL = string.Format(@"SELECT IP,Port FROM (
                                    SELECT IP,Port,ROW_NUMBER() OVER(ORDER BY Speed) AS Num FROM dbo.p_IPProxy WHERE Type='HTTP' AND ProxyIp NOT IN(SELECT ProxyIP FROM dbo.p_ProxyIPUseHistory WHERE Type='IpProxyJob' AND CreateDay=CONVERT(VARCHAR(10),GETDATE(),120))
                                 ) AS A
                                WHERE  Num BETWEEN {0} AND {1}", (currentPage - 1) * PageSize + 1, currentPage * PageSize);
                DataTable dt = SQLHelper.FillDataTable(strSQL);
                if (dt==null || dt.Rows.Count == 0)
                {
                    break;
                }
                foreach (DataRow dr in dt.Rows)
                {
                    tempProxyIp = GetIP(dr["IP"].ToString(), dr["Port"].ToString());
                    TaskLog.IpProxyLogInfo.WriteLogE("当前IP:" + tempProxyIp);
                    if (Ping(dr["IP"].ToString()) && GetTotalPage(Param.IPUrl, tempProxyIp) > 1)
                    {
                        ProxyIp = tempProxyIp;
                        break;
                    }
                }
                currentPage++;
            }
            if (string.IsNullOrEmpty(ProxyIp))
            {
                ProxyIp = Param.DefaultProxyIp;
            }
            return ProxyIp;
        }
    }

    public class IPProxy
    {
        /// <summary>
        /// 国家简称
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// ip代理地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 代理端口
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// ip代理地址(包含端口)
        /// </summary>
        public string ProxyIp { get; set; }

        /// <summary>
        /// ip位置
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 匿名类型
        /// </summary>
        public string Anonymity { get; set; }

        /// <summary>
        /// http类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 连接速度
        /// </summary>
        public string Speed { get; set; }

        /// <summary>
        /// 连接时间
        /// </summary>
        public string ConnectTime { get; set; }

        /// <summary>
        /// 验证时间
        /// </summary>
        public string VerifyTime { get; set; }

    }

    /// <summary>
    /// 执行参数
    /// </summary>
    public class ProxyParam
    {
        /// <summary>
        /// 提取代理ip站点地址
        /// </summary>
        public string IPUrl { get; set; }

        /// <summary>
        /// 请求站点默认使用的代理ip信息
        /// </summary>
        public string DefaultProxyIp { get; set; }

        /// <summary>
        /// 请求站点使用的代理ip信息
        /// </summary>
        public string ProxyIp { get; set; }

        /// <summary>
        /// 是否对获取的代理ip进行ping命令处理,确定该代理是否有效
        /// </summary>
        public bool IsPingIp { get; set; }
    }
}