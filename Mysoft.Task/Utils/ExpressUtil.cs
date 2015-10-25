using CsharpHttpHelper;
using Mysoft.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace Mysoft.Task.Utils
{
    /// <summary>
    /// 快递工具类
    /// </summary>
    public class ExpressUtil
    {
        private static HttpHelper http = new HttpHelper();

        /// <summary>
        /// 快递公司列表
        /// </summary>
        private static List<ExpressCompany> ListExpressCompany = null;

        /// <summary>
        /// 返回所有需要查询的快递单号信息
        /// </summary>
        /// <returns>快递单号</returns>
        private static List<ExpressInfo> GetAllExpressInfo()
        {
            return SQLHelper.ToList<ExpressInfo>("SELECT * FROM p_ExpressInfo");
        }

        /// <summary>
        /// 返回所有快递公司信息
        /// </summary>
        /// <returns>快递公司</returns>
        public static List<ExpressCompany> GetAllExpressCompany()
        {
            if (ListExpressCompany == null)
            {
                ListExpressCompany = SQLHelper.ToList<ExpressCompany>("SELECT * FROM p_ExpressCompany");
            }
            return ListExpressCompany;
        }

        /// <summary>
        /// 获取快递进度信息
        /// </summary>
        /// <returns>是否处理成功</returns>
        public static bool HandleProecssInfo(string ProxyIp)
        {
            List<ExpressInfo> listExpressInfo = GetAllExpressInfo();
            HttpHelper http = new HttpHelper();
            string url = string.Empty;
            string html = string.Empty;
            int SuccessCount = 0;
            foreach (var ExpressInfo in listExpressInfo)
            {
                try
                {
                    //进度查询URL
                    url = string.Format("http://www.kuaidi100.com/query?type={0}&postid={1}&id=1&valicode=&temp=0.7078260387203143", ExpressInfo.ExpressCompanyCode, ExpressInfo.ExpressNo);
                    html = GetHTML(url, ProxyIp);
                    if (html == null)
                    {
                        TaskLog.ExpressProgressLogInfo.WriteLogE(string.Format("url:{0},ip:{1}获取快递进度内容出错", url, ProxyIp));
                        continue;
                    }
                    TaskLog.ExpressProgressLogInfo.WriteLogE(string.Format("开始查询快递单号{0}进度信息", ExpressInfo.ExpressNo));
                    if (ParseExpressInfo(html, ExpressInfo))
                    {
                        SuccessCount++;
                        TaskLog.ExpressProgressLogInfo.WriteLogE(string.Format("结束快递单号{0}进度信息查询", ExpressInfo.ExpressNo));
                    }
                    //等待10s再次查询其它单号信息,避免间隔太小ip被封
                    Thread.Sleep(10000);
                }
                catch (Exception ex)
                {
                    TaskLog.ExpressProgressLogError.WriteLogE(string.Format("url:{0},ip:{1}获取快递进度内容出错", url, ProxyIp), ex);
                }
            }
            return SuccessCount == listExpressInfo.Count;
        }

        /// <summary>
        /// 解析快递信息
        /// </summary>
        /// <param name="json">服务器返回json数据</param>
        /// <param name="ExpressInfo">快递单号信息</param>
        /// <returns>是否成功</returns>
        private static bool ParseExpressInfo(string json, ExpressInfo ExpressInfo)
        {
            try
            {
                JObject jo = JObject.Parse(json);
                JToken value = null;

                //快递单当前的状态
                int state=-1;
                if (jo.TryGetValue("state", out value))
                {
                    if (!Int32.TryParse(value.ToString(), out state))
                    {
                        return false;
                    }
                }

                //查询结果状态 
                if (jo.TryGetValue("status", out value))
                {
                    if (value.ToString().Equals("200"))
                    {
                        //快递单号
                        string ExpressNo = string.Empty;

                        if (jo.TryGetValue("nu", out value))
                        {
                            ExpressNo = value.ToString();
                        }
                        if (string.IsNullOrEmpty(ExpressNo))
                        {
                            return false;
                        }
                        if (state==3 || state==4 || state==6)
                        {
                            //3：签收，收件人已签收；4：退签，即货物由于用户拒签、超区等原因退回，而且发件人已经签收；
                            //6：退回，货物正处于退回发件人的途中；
                            //快递周期已结束,则将当前快递单号移入到历史表中,不在产生任何短信信息
                            SaveExpressHistoryInfo(ExpressNo, state);
                        }

                        if (jo.TryGetValue("data", out value))
                        {
                            List<ExpressProcessDetail> list = JsonConvert.DeserializeObject<List<ExpressProcessDetail>>(value.ToString());
                            if (list != null && list.Count > 0)
                            {
                                list.Reverse();
                                int length = list.Count;
                                int maxGroupNo = GetExpressDetailMaxGroupNo(ExpressNo);
                                ExpressProcessDetail item = null;
                                for (int i = maxGroupNo; i < length; i++)
                                {
                                    item = list[i];
                                    item.ExpressNo = ExpressNo;
                                    item.GroupNo = i + 1;
                                    //相对准确，但不是完全准确的一个状态
                                    item.State = GetExpressDetailState(item,maxGroupNo==0 && i==0);
                                }


                                //产生了新的快递进度信息,准备发送短信通知
                                if (length > maxGroupNo)
                                {
                                    var saveList=list.Where(e => e.GroupNo > maxGroupNo).ToList();
                                    //保存新的进度信息
                                    SQLHelper.BatchSaveData(saveList, "p_ExpressProcessDetail");
                                    //插入短信通知，等待任务轮训时发送短信
                                    var lastDetail = list[length - 1];
                                    string[] Receivers = ExpressInfo.Receiver.Split(new char[] { ',' });
                                    foreach (string Receiver in Receivers)
                                    {
                                        if (RegexHelper.IsMobile(Receiver))
                                        {
                                            //短信最大长度为40字节,20汉字
                                            string content = string.Format("亲快件*{0}!{1}", ExpressNo.Substring(ExpressNo.Length - 4 > 0 ? ExpressNo.Length - 4 : 0), lastDetail.Context.Replace(" ", ""));
                                            //由于短信接口只有20条免费短信的限制，所以改为邮件提醒
                                            //MessageHelper.AddMessage(Receiver, content.FormatStringLength(40), "");
                                        }
                                        else if (RegexHelper.IsEmail(Receiver))
                                        {
                                            bool isAddMessage = false;
                                            //修改消息添加规则,有变更就添加可能一天收很多邮件,让人厌烦
                                            //现在规则修改为快递状态为1:揽件 3:签收 5:派件 这几种状态必发
                                            //然后每天其它状态的最多发送一条     
                                            var importantDetail = saveList.Where(e => e.State == 1 || e.State == 3 || e.State == 5);
                                            if ( importantDetail!= null && importantDetail.Count()>0)
                                            {
                                                isAddMessage = true;
                                            }
                                            else
                                            {
                                                if (!HasSendMessage(ExpressInfo.ExpressGUID))
                                                {
                                                    isAddMessage = true;
                                                }
                                            }
                                            if (isAddMessage)
                                            {
                                                Hashtable ht = new Hashtable();
                                                List<ExpressCompany> listExpressCompany = GetAllExpressCompany();
                                                string ExpressCompany = listExpressCompany.FirstOrDefault(e => e.CompanyCode == ExpressInfo.ExpressCompanyCode).CompanyName;
                                                ht["TotalTime"] = lastDetail.Time.GetDayAndHours(list[0].Time);
                                                ht["T"] = list;
                                                ht["V"] = ExpressInfo;
                                                ht["ExpressCompany"] = ExpressCompany;
                                                ht["Status"] = state;
                                                string content = FileGen.GetFileText(FileHelper.GetAbsolutePath("Temples/ExpressDetail.vm"), ht).ToString();
                                                //添加邮件消息提醒
                                                MessageHelper.AddMessage(Receiver, content, "快递进度变更", "快递进度", ExpressInfo.ExpressGUID, MessageType.EMAIL);
                                            }
                                        }
                                        else
                                        {
                                            TaskLog.ExpressProgressLogInfo.WriteLogE(string.Format("快递单号“{0}”的接收人“{1}”无法识别,不为邮件/手机号任何一种，请检查！", ExpressInfo.ExpressNo, ExpressInfo.Receiver));
                                        }
                                    }
                                }
                            }
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 判断今天是否已经已经发送过邮件提醒
        /// </summary>
        /// <param name="ExpressGUID">快递单GUID</param>
        /// <returns>bool</returns>
        private static bool HasSendMessage(Guid ExpressGUID)
        {
            string strsQL = @"SELECT SUM(Total) 
                    FROM (        
	                    SELECT COUNT(1) AS Total FROM dbo.p_Message  WHERE Type=1 AND FromType='快递进度' AND DATEDIFF(DAY,CreatedOn,GETDATE())=0 AND FkGUID=@FkGUID
	                    UNION ALL       
	                    SELECT COUNT(1) AS Total FROM dbo.p_MessageHistory WHERE Type=1 AND FromType='快递进度' AND DATEDIFF(DAY,SendOn,GETDATE())=0 AND FkGUID=@FkGUID
                    )AS A";
            return SQLHelper.ExecuteScalar<int>(strsQL, new { FkGUID = ExpressGUID }) > 0;
        }

        /// <summary>
        /// 计算快递进度明细对应的状态
        /// </summary>
        /// <param name="info">快递进度信息</param>
        /// <param name="isFirst">是否第一条进度信息 如果为第一条则状态为1(揽件，货物已由快递公司揽收并且产生了第一条跟踪信息)</param>
        /// <returns>快递状态</returns>
        private static int GetExpressDetailState(ExpressProcessDetail info, bool isFirst = false)
        {
            int State = 0;
            if (isFirst)
            {
                State = 1;
            }
            else
            {
                if (info.Context.Contains("派件人"))
                {
                    State = 5;
                }
                else if (info.Context.Contains("签收人"))
                {
                    State = 3;
                }
            }
            return State;
        }

        /// <summary>
        /// 保存快递单历史信息
        /// </summary>
        /// <param name="ExpressNo">快递单号</param>
        /// <param name="state">快递最终状态</param>
        private static void SaveExpressHistoryInfo(string ExpressNo, int state)
        {
            SQLHelper.ExecuteNonQuery(@"INSERT INTO dbo.p_ExpressHistoryInfo(ExpressGUID,ExpressNo,ExpressCompanyCode,Receiver,State,CreatedOn)
                SELECT  ExpressGUID ,ExpressNo ,ExpressCompanyCode ,Receiver ,@State,CreatedOn FROM dbo.p_ExpressInfo WHERE ExpressNo=@ExpressNo;
                DELETE FROM dbo.p_ExpressInfo WHERE ExpressNo=@ExpressNo;", new { State = state, ExpressNo = ExpressNo });
        }

        /// <summary>
        /// 获取当前快递进度明细最大组内编号
        /// </summary>
        /// <param name="ExpressNo">快递单号</param>
        /// <returns>当前最大进度编号</returns>
        private static int GetExpressDetailMaxGroupNo(string ExpressNo)
        {
            return SQLHelper.ExecuteScalar<int>(@"SELECT ISNULL(MAX(GroupNo),0) FROM dbo.p_ExpressProcessDetail WHERE ExpressNo=@ExpressNo", new { ExpressNo = ExpressNo });
        }

        private static string GetHTML(string url, string ProxyIp)
        {
            //创建Httphelper参数对象
            HttpItem item = new HttpItem()
            {
                URL = url,//URL     必需项    
                Method = "get",//可选项 默认为Get   
                ContentType = "text/plain",//返回类型
                ProxyIp = ProxyIp
            };
            //请求的返回值对象
            HttpResult result = http.GetHtml(item);
            if (result.Html.Equals("无法连接到远程服务器"))
            {
                return null;
            }
            try
            {
                JObject jo = JObject.Parse(result.Html);
                JToken value = null;
                //查询结果状态 
                if (jo.TryGetValue("status", out value))
                {
                    if (value.ToString().Equals("200"))
                    {
                        return result.Html;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取正确的代理ip
        /// </summary>
        /// <param name="DefaultProxyIp">默认代理IP可为空</param>
        /// <returns>正确的代理ip</returns>
        public static string GetCorrectIP(string DefaultProxyIp)
        {
            string ProxyIp = string.Empty;
            string tempProxyIp = string.Empty;
            string url = "http://www.kuaidi100.com/query?type=yuantong&postid=880373857190629830&id=1&valicode=&temp=0.7078260387203143";
            //当前页
            int currentPage = 1;
            int PageSize = 100;
            while (string.IsNullOrEmpty(ProxyIp))
            {
                //从数据库取
                string strSQL = string.Format(@"SELECT IP,Port FROM (
                                    SELECT IP,Port,ROW_NUMBER() OVER(ORDER BY Speed) AS Num FROM dbo.p_IPProxy WHERE Type='HTTP' AND ProxyIp NOT IN(SELECT ProxyIP FROM dbo.p_ProxyIPUseHistory WHERE Type='ExpressProgressJob' AND CreateDay=CONVERT(VARCHAR(10),GETDATE(),120))
                                 ) AS A
                                WHERE  Num BETWEEN {0} AND {1}", (currentPage - 1) * PageSize + 1, currentPage * PageSize);
                DataTable dt = SQLHelper.FillDataTable(strSQL);
                if (dt == null || dt.Rows.Count == 0)
                {
                    break;
                }
                foreach (DataRow dr in dt.Rows)
                {
                    tempProxyIp = dr["IP"].ToString() + ":" + dr["Port"].ToString();
                    TaskLog.ExpressProgressLogInfo.WriteLogE("当前IP:" + tempProxyIp);
                    if (Ping(dr["IP"].ToString()) && GetHTML(url, tempProxyIp) != null)
                    {
                        ProxyIp = tempProxyIp;
                        break;
                    }
                }
                currentPage++;
            }
            if (string.IsNullOrEmpty(ProxyIp))
            {
                ProxyIp = DefaultProxyIp;
            }
            return ProxyIp;
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
    }

    ///<summary>
    ///快递单号信息
    ///</summary>
    internal sealed class ExpressInfo
    {

        ///<summary>
        ///单号GUID
        /// </summary>
        public Guid ExpressGUID
        {
            get;
            set;
        }

        ///<summary> 
        ///快递单号
        /// </summary>
        public string ExpressNo
        {
            get;
            set;
        }

        ///<summary>
        ///快递公司简称
        /// </summary>
        public string ExpressCompanyCode
        {
            get;
            set;
        }

        ///<summary> 
        ///短信接收人
        /// </summary>
        public string Receiver
        {
            get;
            set;
        }

        ///<summary>
        ///创建日期
        /// </summary>
        public DateTime CreatedOn
        {
            get;
            set;
        }
    }

    ///<summary>
    ///快递单号进度明细信息
    ///</summary>
    internal sealed class ExpressProcessDetail
    {
        ///<summary> 
        ///组内编号
        /// </summary>
        public int GroupNo
        {
            get;
            set;
        }

        ///<summary> 
        ///快递单号
        /// </summary>
        public string ExpressNo
        {
            get;
            set;
        }

        ///<summary> 
        ///每条跟踪信息的时间
        /// </summary>
        public DateTime Time
        {
            get;
            set;
        }

        ///<summary>
        ///每条跟综信息的描述
        /// </summary>
        public string Context
        {
            get;
            set;
        }

        ///<summary>
        ///当前进度所对应快递状态
        /// </summary>
        public int State
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 快递公司信息
    /// </summary>
    public class ExpressCompany
    {
        public Guid CompanyGUID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
