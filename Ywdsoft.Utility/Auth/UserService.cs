using System;
using System.ComponentModel.Composition;
using Ywdsoft.Model.Common;

namespace Ywdsoft.Utility
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="UserGUID">用户GUID</param>
        /// <returns>用户信息</returns>
        UserAccount GetUserInfo(string UserGUID);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="UserCode">用户Code</param>
        /// <param name="Password">密码</param>
        /// <returns>用户信息</returns>
        UserAccount GetUserInfo(string UserCode, string Password);

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="UserGUID">用户Id</param>
        /// <param name="Pwd">用户密码</param>
        void ChgPwd(string UserGUID, string Pwd);
    }

    /// <summary>
    /// 用户服务
    /// </summary>
    [Export(typeof(IUserService))]
    public class UserService : IUserService
    {
        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="UserGUID">用户Id</param>
        /// <param name="Pwd">用户密码</param>
        public void ChgPwd(string UserGUID, string Pwd)
        {
            string strSQL = "update p_User set Password=@Password WHERE UserGUID = @UserGUID";
            SQLHelper.ExecuteNonQuery(strSQL, new { UserGUID = UserGUID, Password = Pwd });
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="UserGUID">用户GUID</param>
        /// <returns>用户信息</returns>
        public UserAccount GetUserInfo(string UserGUID)
        {
            string strSQL = "SELECT  * from p_User WHERE UserGUID = @UserGUID limit 1 offset 0";
            return SQLHelper.Single<UserAccount>(strSQL, new { UserGUID = UserGUID });
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="UserCode">用户Code</param>
        /// <param name="Password">密码</param>
        /// <returns>用户信息</returns>
        public UserAccount GetUserInfo(string UserCode, string Password)
        {
            string strSQL = "SELECT  * from p_User WHERE UserCode = @UserCode and Password=@Password limit 1 offset 0";
            return SQLHelper.Single<UserAccount>(strSQL, new { UserCode = UserCode, Password = Password });
        }
    }
}
