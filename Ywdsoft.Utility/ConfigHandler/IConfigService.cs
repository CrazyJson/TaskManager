/*
 * Model: 模块名称
 * Desctiption: 描述
 * Author: 杜冬军
 * Created: 2016/3/22 9:26:12 
 */

using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Ywdsoft.Utility.ConfigHandler
{
    /// <summary>
    /// 系统配置接口
    /// </summary>
    public interface IConfigService
    {
        /// <summary>
        /// 获取所有配置项
        /// </summary>
        /// <returns>所有配置项</returns>
        List<Options> GetAllOptions(string GroupType = "");
    }

    [Export(typeof(IConfigService))]
    public class ConfigService : IConfigService
    {
        /// <summary>
        /// 获取所有配置项
        /// </summary>
        /// <returns>所有配置项</returns>
        public List<Options> GetAllOptions(string GroupType = "")
        {
            string strSQL = "select * from Configuration_Options";
            if (!string.IsNullOrEmpty(GroupType))
            {
                strSQL += string.Format(" where OPTIONTYPE='{0}'", GroupType);
            }
            return SQLHelper.ToList<Options>(strSQL);
        }
    }
}