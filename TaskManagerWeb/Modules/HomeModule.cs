/*
 * 模块名: 主页模块
 * 描述: 主页系统框架
 * 作者: 杜冬军
 * 创建日期: 2016/2/23 8:50:22 
 * 博客地址：http://yanweidie.cnblogs.com
 */
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ywdsoft.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            //主页
            Get["/"] = r =>
            {
                return Response.AsRedirect("/Home/Index");
            };

            //主页
            Get["/Home/Index"] = r =>
            {
                var model = "我是 Razor 引擎";
                return View["index", model];
            };

            ///桌面
            Get["/DestTop"] = r =>
            {
                return View["DestTop"];
            };
        }
    }
}