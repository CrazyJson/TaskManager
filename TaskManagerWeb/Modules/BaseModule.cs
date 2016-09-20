/*
 * 模块名: Excel导出模块
 * 作者: 杜冬军
 * 创建日期: 2016/3/31 10:47:22 
 * 博客地址：http://yanweidie.cnblogs.com
 */

using Nancy;
using Ywdsoft.Utility.ConfigHandler;

namespace Ywdsoft.Modules
{
    public class BaseModule: NancyModule
    {
        public BaseModule()
        {
            Init();
        }

        public BaseModule(string modulePath): base(modulePath)
        {
            Init();
        }

        private void Init()
        {
            Before += ctx => {
                //静态资源版本
                ViewBag.Version = SystemConfig.StaticVersion;
                return null;
            };
        }
    }
}