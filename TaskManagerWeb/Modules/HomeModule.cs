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
using Ywdsoft.Utility.Mef;

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

            //修改密码
            Post["/Home/ChgPwd"] = r =>
            {
                ApiResult<string> result = new ApiResult<string>();
                string PasswordOne = this.Request.Form.PasswordOne;
                string PasswordTwo = this.Request.Form.PasswordTwo;
                IUserService UserService = MefConfig.TryResolve<IUserService>();
                if (string.IsNullOrEmpty(PasswordOne) || string.IsNullOrEmpty(PasswordTwo) || !PasswordOne.Equals(PasswordTwo))
                {
                    result.HasError = true;
                    result.Message = "两次密码不一致";
                }
                else
                {
                    UserService.ChgPwd(UserAccountInfo.UserGUID, DESEncrypt.Encrypt(PasswordOne));
                }
                return result;
            };
        }
    }
}