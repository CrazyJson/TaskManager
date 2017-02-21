/*
 * 模块名: 主页模块
 * 描述: 主页系统框架
 * 作者: 杜冬军
 * 创建日期: 2016/2/23 8:50:22 
 * 博客地址：http://yanweidie.cnblogs.com
 */
using Nancy;
using Ywdsoft.Utility;
using Ywdsoft.Utility.ConfigHandler;

namespace Ywdsoft.Modules
{
    public class HomeModule : BaseModule
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
                return View["index", new { UserName = UserAccountInfo.UserName, Title = SystemConfig.SystemTitle, ProgramName = SystemConfig.ProgramName }];
            };

            ///桌面
            Get["/DestTop"] = r =>
            {
                return View["DestTop", MachineNumber.GetMachineInfo()];
            };
        }
    }
}