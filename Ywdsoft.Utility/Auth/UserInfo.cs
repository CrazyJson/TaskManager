using System;
using System.Collections.Generic;

namespace Ywdsoft.Utility.Auth
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [Serializable]
    public class UserInfo
    {
        /// <summary>
        /// 当前登录用户GUID
        /// </summary>
        public string UserGUID { get; set; }

        /// <summary>
        /// 当前登录用户Code
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// 当前登录用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 是否超级管理员
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 当前登录用户部门GUID
        /// </summary>
        public string DepartmentID { get; set; }

        /// <summary>
        /// 用户拥有的权限
        /// </summary>
        public List<Permission> PermissionList{ get; set; }
    }

    /// <summary>
    /// 权限明细
    /// </summary>
    [Serializable]
    public class Permission
    {
        public string ActionCode
        {
            get;
            set;
        }
        public string FunctionCode
        {
            get;
            set;
        }
    }
}
