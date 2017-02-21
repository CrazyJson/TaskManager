/*
 * 模块名: 登录模块
 * 描述: 用于登录 退出登录
 * 作者: 杜冬军
 * 创建日期: 2016/2/23 8:50:22 
 * 博客地址：http://yanweidie.cnblogs.com
 */

using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Extensions;
using System;
using Ywdsoft.Model.Common;
using Ywdsoft.Utility;
using Ywdsoft.Utility.ConfigHandler;
using Ywdsoft.Utility.Mef;

namespace Ywdsoft.Modules
{
    public class LoginModule : NancyModule
    {
        public LoginModule() : base("Login")
        {
            Get["/"] = r =>
            {
                var ErrorMsg = this.Request.Query.ErrorMsg;
                var UserCode = this.Request.Query.UserCode;
                return View["index", new { ErrorMsg = ErrorMsg, UserCode = UserCode, Title = SystemConfig.SystemTitle, ProgramName = SystemConfig.ProgramName }];
            };

            Post["/"] = r =>
            {
                string UserCode = this.Request.Form.UserCode;
                string Password = this.Request.Form.Password;
                IUserService UserService = MefConfig.TryResolve<IUserService>();
                if (string.IsNullOrEmpty(UserCode) || string.IsNullOrEmpty(Password))
                {

                    return this.Context.GetRedirect("~/Login?ErrorMsg=" + Uri.EscapeDataString("用户名或密码不能为空") + "&UserCode=" + UserCode);
                }
                UserAccount account = UserService.GetUserInfo(UserCode, DESEncrypt.Encrypt(Password));
                if (account == null)
                {
                    return this.Context.GetRedirect("~/Login?ErrorMsg=" + Uri.EscapeDataString("用户名或密码错误") + "&UserCode=" + UserCode);
                }
                else
                {
                    Guid guid = Guid.ParseExact(account.UserGUID, "N");
                    Session["UserInfo"] = account;
                    return this.Login(guid, DateTime.Now.AddMinutes(30));
                }
            };

            //退出登录
            Get["/Exit"] = r =>
            {
                Session.DeleteAll();
                return this.Context.GetRedirect("~/Login");
            };
        }
    }
}