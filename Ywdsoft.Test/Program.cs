using CsharpHttpHelper;
using HtmlAgilityPack;
using Ywdsoft.Utility;
using Nancy.Hosting.Self;
using Owin_Nancy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using Ywdsoft.Utility.Mef;
using Ywdsoft.Utility.ConfigHandler;

namespace Ywdsoft.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            AdminRun.Run();
            //1.MEF初始化
            MefConfig.Init();

            //2.
            ConfigInit.InitConfig();

            //3.系统参数配置初始化
            ConfigManager configManager = MefConfig.TryResolve<ConfigManager>();
            configManager.Init();


            QuartzHelper.InitScheduler();
            QuartzHelper.StartScheduler();
            try
            {
                //启动站点
                using (NancyHost host = Startup.Start(SystemConfig.WebPort))
                {
                    //调用系统默认的浏览器   
                    string url = string.Format("http://127.0.0.1:{0}", SystemConfig.WebPort);
                    Process.Start(url);
                    Console.WriteLine("系统监听站点地址:{0}", url);
                    Console.WriteLine("程序已启动,按任意键退出");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



            //ParseExpressCode();
            //ExpressUtil.HandleProecssInfo("");
            //MessageHelper.SendMessage(new Guid("6282AA73-2A58-E511-8D70-00155D0C740D"));
            //new Ywdsoft.Task.TaskSet.SendMessageJob().Execute(null);
            Console.Read();
        }



        /// <summary>
        /// 代理使用示例
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetUrltoHtml(string Url, string type = "UTF-8")
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(Url);
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
                WebProxy myProxy = new WebProxy("192.168.15.11", 8015);
                //建议连接（代理需要身份认证，才需要用户名密码）
                myProxy.Credentials = new NetworkCredential("admin", "123456");
                //设置请求使用代理信息
                request.Proxy = myProxy;
                // Get the response instance.
                System.Net.WebResponse wResp = request.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();
                // Dim reader As StreamReader = New StreamReader(respStream)
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding(type)))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception)
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
            catch (Exception)
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
