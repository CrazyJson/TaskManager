/*
 * 模块名: 登录模块
 * 描述: 用于登录 退出登录
 * 作者: 杜冬军
 * 创建日期: 2016/2/23 8:50:22 
 * 博客地址：http://yanweidie.cnblogs.com
 */

using Nancy;

namespace Ywdsoft.Modules
{
    public class LoginModule : NancyModule
    {
        public LoginModule() : base("Login")
        {
            Get["/"] = r =>
            {
                var model = "我是 Razor 引擎2";
                return View["index", model];
            };

            //退出登录
            Get["/Exit"] = r =>
            {
                var model = "我是 Razor 引擎";
                return View["inde2x", model];
            };
        }
    }
}