using CsharpHttpHelper;
using HtmlAgilityPack;
using Mysoft.Task.Utils;
using Mysoft.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Mysoft.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigInit.InitConfig();
            //ParseExpressCode();
            //ExpressUtil.HandleProecssInfo(ExpressUtil.GetCorrectIP(""));
            //MessageHelper.SendMessage(new Guid("6282AA73-2A58-E511-8D70-00155D0C740D"));
            //new Mysoft.Task.TaskSet.SendMessageJob().Execute(null);
        }

        /// <summary>
        /// 代理使用示例
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetUrltoHtml(string Url, string type)
        {
            try
            {
                System.Net.WebRequest wReq = System.Net.WebRequest.Create(Url);

                WebProxy myProxy = new WebProxy("192.168.15.11", 8015);
                //建议连接（代理需要身份认证，才需要用户名密码）
                myProxy.Credentials = new NetworkCredential("admin", "123456");
                //设置请求使用代理信息
                wReq.Proxy = myProxy;
                // Get the response instance.
                System.Net.WebResponse wResp = wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();
                // Dim reader As StreamReader = New StreamReader(respStream)
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding(type)))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
                //errorMsg = ex.Message;
            }
            return "";
        }



        /// <summary>
        /// 获取快递公司列表
        /// </summary>
        private static void ParseExpressCode()
        {
            string HTML = GetHTML("http://m.kuaidi100.com/all/", "61.234.249.107:8118");
            var doc = new HtmlDocument();
            doc.LoadHtml(HTML);
            var coms = doc.DocumentNode.SelectNodes(@"//dl[@id='comList']/dd/a");
            List<ExpressCom> list = new List<ExpressCom>();
            HtmlAttribute atr = null;
            foreach (var node in coms)
            {
                ExpressCom item = new ExpressCom();
                atr = node.Attributes["data-code"];
                if (atr != null)
                {
                    item.CompanyCode = atr.Value.Trim();
                }

                item.CompanyName = node.InnerText.Trim();
                list.Add(item);
            }
            if (list.Count > 0)
            {
                SQLHelper.ExecuteNonQuery("Truncate TABLE p_ExpressCompany");
                SQLHelper.BatchSaveData(list, "p_ExpressCompany");
            }
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
                HttpHelper http = new HttpHelper();
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
                return "<HTML></HTML>";
            }
        }
    }

    public class ExpressCom
    {
        public string CompanyName { get; set; }

        public string CompanyCode { get; set; }
    }
}
