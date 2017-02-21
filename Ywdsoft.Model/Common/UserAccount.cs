using System;

namespace Ywdsoft.Model.Common
{
    /// <summary>
    /// 用户基础信息
    /// </summary>
    [Serializable]
    public class UserAccount
    {
        /// <summary>
        /// 用户GUID
        /// </summary>
        public string UserGUID { get; set; }

        /// <summary>
        /// 用户代码
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
