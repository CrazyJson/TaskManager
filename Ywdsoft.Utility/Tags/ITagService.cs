/*
 * Model: 自定义标签服务
 * Desctiption: 描述
 * Author: 杜冬军
 * Created: 2016/3/15 9:33:45 
 * Copyright：武汉中科通达高新技术股份有限公司
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Ywdsoft.Model.Common;

namespace Ywdsoft.Utility
{
    /// <summary>
    /// 自定义标签服务接口
    /// </summary>
    public interface ITagService
    {
        /// <summary>
        /// 通过来源类型获取自定义标签
        /// </summary>
        /// <param name="SourceType">来源类型 <seealso cref="TagsSourceType"/></param>
        /// <returns>自定义标签</returns>
        List<Tags> GetTags(string SourceType);

        /// <summary>
        /// 通过来源类型获取自定义标签
        /// </summary>
        /// <param name="SourceType">来源类型 <seealso cref="TagsSourceType"/></param>
        /// <param name="SourceId">来源Id</param>
        /// <returns>自定义标签</returns>
        List<Tags> GetTags(string SourceType, string SourceId);

        /// <summary>
        /// 保存同一类标签
        /// </summary>
        /// <param name="tagsList">标签列表</param>
        /// <param name="SourceType">来源类型 <seealso cref="TagsSourceType"/></param>
        /// <param name="operatorCode">操作人</param>
        /// <returns>影响条数</returns>
        int SaveTags(List<Tags> tagsList, string SourceType, string operatorCode);

        /// <summary>
        /// 保存同一类标签
        /// </summary>
        /// <param name="tagsList">标签列表</param>
        /// <param name="SourceType">来源类型 <seealso cref="TagsSourceType"/></param>
        /// <param name="SourceId">来源Id</param>
        /// <param name="operatorCode">操作人</param>
        /// <returns>影响条数</returns>
        int SaveTags(List<Tags> tagsList, string SourceType, string SourceId, string operatorCode);

        /// <summary>
        /// 删除某个类别所有标签
        /// </summary>
        /// <param name="SourceType">来源类型 <seealso cref="TagsSourceType"/></param>
        /// <returns>影响条数</returns>
        int DeleteTags(string SourceType);

        /// <summary>
        /// 删除某个类别所有标签
        /// </summary>
        /// <param name="SourceType">来源类型 <seealso cref="TagsSourceType"/></param>
        /// <param name="SourceId">来源Id</param>
        /// <returns>影响条数</returns>
        int DeleteTags(string SourceType, string SourceId);

        /// <summary>
        /// 更新标签热度
        /// </summary>
        /// <param name="TagGUID">Tag主键</param>
        /// <returns>更新结果</returns>
        int UpdateHeat(string TagGUID);
    }

    /// <summary>
    /// 自定义标签服务
    /// </summary>
    [Export(typeof(ITagService))]
    public class TagService : ITagService
    {
        /// <summary>
        /// 通过来源类型获取自定义标签
        /// </summary>
        /// <param name="SourceType">来源类型 <seealso cref="TagsSourceType"/></param>
        /// <returns>自定义标签</returns>
        public List<Tags> GetTags(string SourceType)
        {
            return GetTags(SourceType, null);
        }

        /// <summary>
        /// 通过来源类型获取自定义标签
        /// </summary>
        /// <param name="SourceType">来源类型 <seealso cref="TagsSourceType"/></param>
        /// <param name="SourceId">来源Id</param>
        /// <returns>自定义标签</returns>
        public List<Tags> GetTags(string SourceType, string SourceId)
        {
            if (string.IsNullOrEmpty(SourceType))
            {
                return new List<Tags>();
            }
            string strSQL = "SELECT * FROM p_Tags WHERE TagType=@SourceType  AND 1=1 ORDER BY TagHeat DESC";

            if (!string.IsNullOrEmpty(SourceId))
            {
                strSQL = strSQL.Replace("1=1", " SourceId = @SourceId ");
            }

            return SQLHelper.ToList<Tags>(strSQL, new { SourceType = SourceType, SourceId = SourceId });
        }

        /// <summary>
        /// 保存同一类标签
        /// </summary>
        /// <param name="tagsList">标签列表</param>
        /// <param name="SourceType">来源类型 <seealso cref="TagsSourceType"/></param>
        /// <param name="operatorCode">操作人</param>
        public int SaveTags(List<Tags> tagsList, string SourceType, string operatorCode)
        {
            return SaveTags(tagsList, SourceType, null, operatorCode);
        }

        /// <summary>
        /// 保存同一类标签
        /// </summary>
        /// <param name="tagsList">标签列表</param>
        /// <param name="SourceType">来源类型 <seealso cref="TagsSourceType"/></param>
        /// <param name="SourceId">来源Id</param>
        /// <param name="operatorCode">操作人</param>
        public int SaveTags(List<Tags> tagsList, string SourceType, string SourceId, string operatorCode)
        {
            if (string.IsNullOrEmpty(SourceType) || tagsList == null || tagsList.Count == 0)
            {
                return -1;
            }
            var allTagList = GetTags(SourceType);
            List<Tags> list = null;
            if (!string.IsNullOrEmpty(SourceId))
            {
                list = GetTags(SourceType, SourceId);
            }
            else
            {
                list = allTagList;
            }

            //删除的数据
            var queryRemove = (from p in list
                               where !(from q in tagsList select q.TagGUID).Contains(p.TagGUID)
                               select p.TagGUID);
            //新增的数据
            var queryAdd = tagsList.Where(e => string.IsNullOrEmpty(e.TagGUID)).ToList();
            var dict = new Dictionary<string, int>();
            foreach (var item in allTagList)
            {
                dict[item.TagName] = item.TagHeat.Value;
            }
            int TagHeat = 0;
            foreach (var item in queryAdd)
            {
                if (!dict.TryGetValue(item.TagName, out TagHeat))
                {
                    TagHeat = 0;
                }
                item.TagGUID = Guid.NewGuid().ToString("N");
                item.TagType = SourceType;
                item.SourceId = SourceId;
                item.TagHeat = TagHeat;
                item.CreatedTime = DateTime.Now;
                item.Creator = operatorCode;
            }
            int change = 0;
            SQLHelper.BatchSaveData<Tags>(queryAdd, "p_Tags");
            change = SQLHelper.ExecuteNonQuery("DELETE FROM p_Tags WHERE TagGUID in('" + string.Join("','", queryRemove) + "')");
            return change;
        }

        /// <summary>
        /// 删除某个类别所有标签
        /// </summary>
        /// <param name="SourceType">来源类型 <seealso cref="TagsSourceType"/></param>
        /// <returns>影响条数</returns>
        public int DeleteTags(string SourceType)
        {
            return DeleteTags(SourceType, null);
        }

        /// <summary>
        /// 删除某个类别所有标签
        /// </summary>
        /// <param name="SourceType">来源类型 <seealso cref="TagsSourceType"/></param>
        /// <param name="SourceId">来源Id</param>
        /// <returns>影响条数</returns>
        public int DeleteTags(string SourceType, string SourceId)
        {
            if (string.IsNullOrEmpty(SourceType))
            {
                return -1;
            }
            string strSQL = @"DELETE FROM p_Tags WHERE TagType=@SourceType";
            if (!string.IsNullOrEmpty(SourceId))
            {
                strSQL += " AND SourceId=@SourceId";
            }
            return SQLHelper.ExecuteNonQuery(strSQL, new { SourceType = SourceType, SourceId = SourceId });
        }

        /// <summary>
        /// 更新标签热度
        /// </summary>
        /// <param name="TagGUID">Tag主键</param>
        /// <returns>更新结果</returns>
        public int UpdateHeat(string TagGUID)
        {
            string strSQL = @"UPDATE p_Tags SET TAGHEAT=TAGHEAT+1 WHERE EXISTS (
                                SELECT 1 FROM p_Tags A WHERE A.TAGGUID =@TagGUID
                                 AND p_Tags.TAGTYPE = A.TAGTYPE AND p_Tags.TAGNAME = A.TAGNAME
                              ) ";
            return SQLHelper.ExecuteNonQuery(strSQL, new { TagGUID = TagGUID });
        }
    }
}
