/*
 * 模块名: 主页模块
 * 描述: 主页系统框架
 * 作者: 杜冬军
 * 创建日期: 2016/2/23 8:50:22 
 * 博客地址：http://yanweidie.cnblogs.com
 */
using Nancy;
using Nancy.ModelBinding;
using Ywdsoft.Utility.ConfigHandler;
using Ywdsoft.Utility.Mef;

namespace Ywdsoft.Modules
{
    public class ConfigModule : BaseModule
    {
        private static ConfigManager manager { get; set; }

        public ConfigModule() : base("Config")
        {
            //列表
            Get["/Grid"] = r =>
            {
                manager= MefConfig.TryResolve<ConfigManager>();
                return View["Grid"];
            };

            //取数接口API
            #region

            ///// <summary>
            ///// 获取所有配置信息
            ///// </summary>
            ///// <returns>所有配置信息</returns>
            Get["/GetAllOption"] = r =>
            {
                return Response.AsJson(manager.GetAllOption());
            };

            ///// <summary>
            ///// 获取指定项配置信息
            ///// </summary>
            ///// <param name="GroupType">分组项</param>
            ///// <returns>所有配置信息</returns>
            Get["/GetOptionByGroup"] = r =>
            {
                string GroupType = Request.Query["GroupType"].ToString();
                return Response.AsJson(manager.GetOptionByGroup(GroupType));
            };

            ///// <summary>
            ///// 保存数据
            ///// </summary>
            Post["/"] = r =>
            {
                OptionViewModel value = this.Bind<OptionViewModel>();
                return Response.AsJson(manager.Save(value));
            };
          
            #endregion
        }
    }
}