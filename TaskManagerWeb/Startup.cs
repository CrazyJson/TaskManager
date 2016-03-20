using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ywdsoft.Utility;

namespace Owin_Nancy
{
    public class Startup
    {
        private static NancyHost _host = null;
        /// <summary>
        /// 监听端口 启动站点
        /// </summary>
        /// <param name="urls">监听ip端口列表</param>
        public static NancyHost Start(int port)
        {
            try
            {
                _host = new NancyHost(new Uri(string.Format("http://localhost:{0}", port)));
                _host.Start();
                LogHelper.WriteLog("Web管理站点启动成功,请打开 http://127.0.0.1:" + port + "进行浏览");
                return _host;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Web管理站点启动失败", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 回收资源
        /// </summary>
        public static void Dispose()
        {
            //回收资源
            if (_host != null)
            {
                _host.Stop();
                _host.Dispose();
                _host = null;
            }
        }
    }
}
