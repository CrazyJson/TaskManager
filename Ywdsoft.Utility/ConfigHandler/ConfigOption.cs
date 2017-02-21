/*
 * Model: 系统参数配置接口
 * Desctiption: 描述
 * Author: 杜冬军
 * Created: 2016/3/21 9:03:25 
 */


using System.Collections.Generic;

namespace Ywdsoft.Utility.ConfigHandler
{  /// <summary>
   /// 系统配置参数基类
   /// </summary>
    public abstract class ConfigOption
    {
        /// <summary>
        /// 当前配置项所属分组
        /// </summary>
        public virtual string GroupType { get; set; }

        /// <summary>
        /// 参数配置项保存前处理逻辑
        /// </summary>
        /// <param name="value">当前保存的参数</param>
        public virtual VerifyResult BeforeSave(OptionViewModel value)
        {
            return new VerifyResult();
        }

        /// <summary>
        /// 参数配置项保存后处理逻辑，一般用于重要配置项 发消息通知其他系统进行配置项更新
        /// </summary>
        /// <param name="listOptions">当前保存的参数</param>
        public virtual void AfterSave(List<Options> listOptions)
        {

        }

        /// <summary>
        /// 动态设置属性的默认值，初始化时调用
        /// </summary>
        /// <param name="ca">属性</param>
        /// <remarks>用于解决默认值是经过其它条件计算出来，不能初始化设置的问题</remarks>
        public virtual void SetDefaultValue(ConfigAttribute ca)
        {

        }
    }

    /// <summary>
    /// 保存前校验结果
    /// </summary>
    public class VerifyResult
    {
        public VerifyResult()
        {
            IsSusscess = true;
        }

        /// <summary>
        /// 是否校验成功
        /// </summary>
        public bool IsSusscess { get; set; }

        /// <summary>
        /// 校验错误，对应的错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}