/*
 * 模块名: 主页模块
 * 描述: 主页系统框架
 * 作者: 杜冬军
 * 创建日期: 2016/2/23 8:50:22 
 * 博客地址：http://yanweidie.cnblogs.com
 */
using AutoMapper;
using Nancy;
using Nancy.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using Ywdsoft.Model.Common;
using Ywdsoft.Utility;
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
                manager = MefConfig.TryResolve<ConfigManager>();
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
                ITagService tagService = MefConfig.TryResolve<ITagService>();
                IEnumerable<Tags> listTags = tagService.GetTags(TagsSourceType.ConfigHandler);

                var list = manager.GetAllOption();
                List<ConfigViewModel> listDto = new List<ConfigViewModel>();
                ConfigViewModel configModel = null;
                foreach (var item in list)
                {
                    configModel = Mapper.DynamicMap<OptionViewModel, ConfigViewModel>(item);
                    configModel.TagList = listTags.Where(e => e.SourceId == item.Group.GroupType).ToList();
                    listDto.Add(configModel);
                }
                return Response.AsJson(listDto);
            };

            ///// <summary>
            ///// 获取指定项配置信息
            ///// </summary>
            ///// <param name="GroupType">分组项</param>
            ///// <returns>所有配置信息</returns>
            Get["/GetOptionByGroup"] = r =>
            {
                string GroupType = Request.Query["GroupType"].ToString();
                var model = manager.GetOptionByGroup(GroupType);
                ConfigViewModel configModel = Mapper.DynamicMap<OptionViewModel, ConfigViewModel>(model);
                ITagService tagService = MefConfig.TryResolve<ITagService>();
                configModel.TagList = tagService.GetTags(TagsSourceType.ConfigHandler, GroupType);
                return Response.AsJson(configModel);
            };

            ///// <summary>
            ///// 保存数据
            ///// </summary>
            Post["/"] = r =>
            {
                ConfigViewModel value = this.Bind<ConfigViewModel>();
                ITagService tagService = MefConfig.TryResolve<ITagService>();
                //保存标签信息
                if (value.TagList == null || value.TagList.Count == 0)
                {
                    //删除标签
                    tagService.DeleteTags(TagsSourceType.ConfigHandler, value.Group.GroupType);
                }
                else
                {
                    //保存标签
                    tagService.SaveTags(value.TagList, TagsSourceType.ConfigHandler, value.Group.GroupType, "admin");
                }
                return Response.AsJson(manager.Save(Mapper.DynamicMap<ConfigViewModel, OptionViewModel>(value)));
            };

            #endregion
        }
    }

    /// <summary>
    /// 自定义标签参数配置项
    /// </summary>
    public class ConfigViewModel : OptionViewModel
    {
        /// <summary>
        /// 自定义标签列表
        /// </summary>
        public List<Tags> TagList { get; set; }
    }
}