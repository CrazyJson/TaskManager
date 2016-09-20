/*
 * Model: 配置管理
 * Desctiption: 描述
 * Author: 杜冬军
 * Created: 2016/3/21 16:12:58 
 * Copyright：武汉中科通达高新技术股份有限公司
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using Ywdsoft.Utility;

namespace Ywdsoft.Utility.ConfigHandler
{
    /// <summary>
    /// 配置管理,提供配置初始化，配置信息读取保存方法
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ConfigManager
    {
        private static IEnumerable<ConfigOption> _allConfig;
        /// <summary>
        /// 参数配置服务
        /// </summary>
        [Import]
        public IConfigService ConfigService { get; set; }

        /// <summary>
        /// 系统所有配置信息
        /// </summary>
        [ImportMany(typeof(ConfigOption))]
        public IEnumerable<ConfigOption> AllConfig
        {
            get
            {
                return _allConfig;
            }
            set
            {
                if (_allConfig == null)
                {
                    _allConfig = value;
                }
            }
        }

        /// <summary>
        /// 初始化系统参数配置信息
        /// </summary>
        public void Init()
        {
            //所有选项值
            List<Options> listOption = ConfigService.GetAllOptions();

            ConfigDescription desc = null;
            //代码现有配置项
            foreach (ConfigOption item in AllConfig)
            {
                //反射读取配置项ConfigTypeAttribute  ConfigAttribute 信息
                desc = ConfigDescriptionCache.GetTypeDiscription(item.GetType());

                //设置当前配置项的GroupType
                desc.GroupTypePropertyInfo.SetValue(item, Convert.ChangeType(desc.Group, desc.GroupTypePropertyInfo.PropertyType), null);

                //每项值信息
                List<Options> itemOptions = listOption.Where(e => e.OptionType.Equals(desc.Group, StringComparison.OrdinalIgnoreCase)).ToList();
                Options op = null;
                ConfigAttribute ca = null;
                foreach (PropertyInfo prop in desc.StaticPropertyInfo)
                {
                    op = itemOptions.FirstOrDefault(e1 => e1.Key.Equals(prop.Name, StringComparison.OrdinalIgnoreCase));
                    ca = desc.MemberDict[prop.Name];
                    if (op == null)
                    {
                        //设置默认值
                        prop.SetValue(null, Convert.ChangeType(ca.DefaultValue, prop.PropertyType), null);
                    }
                    else
                    {
                        prop.SetValue(null, Convert.ChangeType(op.Value, prop.PropertyType), null);
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有配置信息
        /// </summary>
        /// <returns>所有配置信息</returns>
        public List<OptionViewModel> GetAllOption(string GroupType = "")
        {
            //所有选项值
            List<Options> listOption = ConfigService.GetAllOptions(GroupType);
            //所有分组
            List<OptionGroup> listOptionGroup = new List<OptionGroup>();
            IEnumerable<ConfigOption> listConfigs = AllConfig;
            if (!string.IsNullOrEmpty(GroupType))
            {
                listConfigs = AllConfig.Where(e => e.GroupType.Equals(GroupType, StringComparison.OrdinalIgnoreCase));
            }

            ConfigDescription desc = null;

            //分组信息
            OptionGroup optionGroup = null;
            Options op = null;
            ConfigAttribute ca = null;

            List<OptionViewModel> result = new List<OptionViewModel>();
            OptionViewModel itemOptionViewModel = null;

            //代码现有配置项
            foreach (ConfigOption item in listConfigs)
            {
                //反射读取配置项ConfigTypeAttribute  ConfigAttribute 信息
                desc = ConfigDescriptionCache.GetTypeDiscription(item.GetType());

                itemOptionViewModel = new OptionViewModel();

                optionGroup = new OptionGroup { GroupType = desc.Group, GroupName = desc.GroupCn, ImmediateUpdate = desc.ImmediateUpdate };
                listOptionGroup.Add(optionGroup);

                itemOptionViewModel.Group = optionGroup;
                itemOptionViewModel.ListOptions = new List<Options>();

                //每项值信息
                List<Options> itemOptions = listOption.Where(e => e.OptionType.Equals(desc.Group, StringComparison.OrdinalIgnoreCase)).ToList();

                foreach (PropertyInfo prop in desc.StaticPropertyInfo)
                {
                    op = itemOptions.FirstOrDefault(e1 => e1.Key.Equals(prop.Name, StringComparison.OrdinalIgnoreCase));
                    ca = desc.MemberDict[prop.Name];

                    if (op == null)
                    {
                        op = new Options { OptionType = desc.Group, OptionName = ca.Name, Key = prop.Name, Value = Convert.ToString(ca.DefaultValue), ValueType = Convert.ToInt32(ca.ValueType).ToString() };
                    }
                    //必填设置
                    op.Required = ca.Required;
                    //校验规则
                    op.ValidateRule = ca.ValidateRule;
                    //悬浮title
                    op.Title = ca.Title;
                    itemOptionViewModel.ListOptions.Add(op);
                }
                result.Add(itemOptionViewModel);
            }
            return result.OrderBy(e => e.Group.GroupType).ToList(); ;
        }

        /// <summary>
        /// 获取指定项配置信息
        /// </summary>
        /// <param name="GroupType">分组项</param>
        /// <returns>所有配置信息</returns>
        public OptionViewModel GetOptionByGroup(string GroupType)
        {
            List<OptionViewModel> list = GetAllOption(GroupType);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return null;
        }


        /// <summary>
        /// 获取指定项配置信息
        /// </summary>
        /// <param name="GroupType">分组项</param>
        /// <returns>所有配置信息</returns>
        public Options GetOptionByGroupAndKey(string GroupType, string key)
        {
            List<OptionViewModel> list = GetAllOption(GroupType);
            if (list != null && list.Count > 0)
            {
                return list[0].ListOptions.FirstOrDefault(e => e.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
            }
            return null;
        }

        /// <summary>
        /// 更新指定配置项的值
        /// </summary>
        /// <param name="GroupType">分组</param>
        /// <param name="key">key</param>
        /// <param name="value">更新的值</param>
        public static void UpdateValueByKey(string GroupType, string key, string value)
        {
            SQLHelper.ExecuteNonQuery("UPDATE Configuration_Options set Value=@NewValue WHERE OptionType=@OptionType and [Key]=@OptionKey",
                new { OptionType = GroupType, OptionKey = key, NewValue = value });
        }

        /// <summary>
        /// 保存配置信息
        /// </summary>
        /// <param name="value">配置信息</param>
        public JsonBaseModel<string> Save(OptionViewModel value)
        {
            JsonBaseModel<string> result = new JsonBaseModel<string>();
            result.HasError = true;
            string GroupType = value.Group.GroupType;
            if (value.Group == null || string.IsNullOrEmpty(GroupType) || value.ListOptions == null)
            {
                result.Message = "保存参数配置时发生参数空异常";
                return result;
            }
            //调用保存前处理事件
            ConfigOption curConfigOption = AllConfig.First(e => e.GroupType.Equals(GroupType, StringComparison.OrdinalIgnoreCase));
            if (curConfigOption == null)
            {
                //如果没有找到匹配项
                result.Message = string.Format("当前保存配置信息{0}不对应后台的任务配置类", GroupType);
                return result;
            }
            if (!curConfigOption.BeforeSave(value))
            {
                result.Message = "当前配置项不允许保存";
                return result;
            }

            //保存数据

            try
            {
                //删除原有数据
                SQLHelper.ExecuteNonQuery("Delete from Configuration_Options WHERE OptionType=@OptionType", new { OptionType = GroupType });
                //保存数据
                foreach (var item in value.ListOptions)
                {
                    item.OptionId = Guid.NewGuid().ToString("N");
                    SQLHelper.ExecuteNonQuery(@"INSERT INTO Configuration_Options(OptionId,OptionType,OptionName,[Key],Value,ValueType) 
                    select @OptionId,@OptionType,@OptionName,@Key,@Value,@ValueType", item);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("系统参数配置保存异常", ex);
                result.Message = ex.Message;
                return result;
            }

            //对当前配置项进行赋值
            SetValue(curConfigOption, value.ListOptions);

            //调用保存后处理事件
            curConfigOption.AfterSave(curConfigOption);
            result.HasError = false;
            return result;
        }

        /// <summary>
        /// 保存时 对当前配置项进行赋值
        /// </summary>
        /// <param name="item">当前配置项</param>
        /// <param name="ListOptions">配置项值</param>
        public void SetValue(ConfigOption item, List<Options> ListOptions)
        {
            var desc = ConfigDescriptionCache.GetTypeDiscription(item.GetType());
            Options option = null;
            foreach (PropertyInfo prop in desc.StaticPropertyInfo)
            {
                option = ListOptions.First(e => e.Key.Equals(prop.Name, StringComparison.OrdinalIgnoreCase));
                if (option == null)
                {
                    //不存在该配置项，则清空当前值
                    prop.SetValue(null, Convert.ChangeType(null, prop.PropertyType), null);
                }
                else
                {
                    prop.SetValue(null, Convert.ChangeType(option.Value, prop.PropertyType), null);
                }
            }
        }

        private static T ConvertScalar<T>(object obj)
        {
            if (obj == null || DBNull.Value.Equals(obj))
                return default(T);

            if (obj is T)
                return (T)obj;

            Type targetType = typeof(T);

            if (targetType == typeof(object))
                return (T)obj;

            return (T)Convert.ChangeType(obj, targetType);
        }
    }
}