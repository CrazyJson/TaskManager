/*
 * 模块名: Excel导出模块
 * 作者: 杜冬军
 * 创建日期: 2016/3/31 10:47:22 
 * 博客地址：http://yanweidie.cnblogs.com
 */

using Nancy;
using Nancy.Security;
using Ywdsoft.Model.Common;
using Ywdsoft.Utility.ConfigHandler;

namespace Ywdsoft.Modules
{
    public class BaseModule : NancyModule
    {
        public UserAccount UserAccountInfo = null;

        public BaseModule()
        {
            this.RequiresAuthentication();
            Init();
        }

        public BaseModule(string modulePath) : base(modulePath)
        {
            this.RequiresAuthentication();
            Init();
        }

        private void Init()
        {
            Before += ctx =>
            {
                //静态资源版本
                ViewBag.Version = SystemConfig.StaticVersion;
                UserAccountInfo = Session["UserInfo"] as UserAccount;
                return null;
            };
        }
    }
}