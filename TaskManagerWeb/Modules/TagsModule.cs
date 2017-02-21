/*
 * 模块名: 标签模块
 * 描述: 标签模块
 * 作者: 杜冬军
 * 创建日期: 2016/2/23 8:50:22 
 * 博客地址：http://yanweidie.cnblogs.com
 */
using Ywdsoft.Utility;
using Nancy;
using Ywdsoft.Utility.Mef;

namespace Ywdsoft.Modules
{
    public class TagsModule : BaseModule
    {
        public TagsModule() : base("Tags")
        {

            #region "取数接口API"

            //立即运行一次任务
            Get["/UpdateTagHeat"] = r =>
            {
                string TagGUID = Request.Query["TagGUID"].ToString();
                ITagService tagService = MefConfig.TryResolve<ITagService>();
                return Response.AsText(tagService.UpdateHeat(TagGUID).ToString());
            };


            #endregion
        }
    }
}