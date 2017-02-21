/*
 * 模块名: 全局错误模块
 * 作者: 杜冬军
 * 创建日期: 2016/2/23 8:50:22 
 * 博客地址：http://yanweidie.cnblogs.com
 */

using Nancy;

namespace Ywdsoft.Modules
{
    public class ErrorModule : NancyModule
    {
        public ErrorModule():base("Error")
        {
            //404
            Get["/NotFound"] = r =>
            {
                return View["NotFound"];
            };

            //500
            Get["/ISE"] = r =>
            {
                var model = "我是 Razor 引擎";
                return View["ISE", model];
            };
        }
    }
}