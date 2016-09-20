/*
 * Model:列表查询条件基类
 * Desctiption: 描述
 * Author: 杜冬军
 * Created: 2015/12/11 11:08:57 
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ywdsoft.Utility
{
    /// <summary>
    /// 列表页面进行查询的参数
    /// </summary>
    public class QueryCondition
    {
        private int pageSize;

        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize
        {
            get { return pageSize; }
            set
            {
                if (value < 0)
                {
                    IsPagination = false;
                }
                else
                {
                    IsPagination = true;
                }
                pageSize = value;
            }
        }

        private int page;
        public int Page
        {
            get
            {
                return page;
            }
            set
            {
                page = value;
                PageIndex = value - 1;
            }
        }

        /// <summary>
        /// 当前页数
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortField { get; set; }

        /// <summary>
        /// 升序或者降序
        /// </summary>
        public string SortOrder { get; set; }

        /// <summary>
        /// 是否进行分页查询
        /// </summary>
        public bool IsPagination { get; set; }

        private bool isCalcTotal = true;

        /// <summary>
        /// 分页查询时是否从数据库查询总条数
        /// </summary>
        public bool IsCalcTotal
        {
            get
            {
                return isCalcTotal;
            }
            set
            {
                isCalcTotal = value;
            }
        }

        /// <summary>
        /// 过滤条件集合
        /// </summary>
        public string FilterListStr
        {
            get
            {
                return null;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    FilterList = JsonConvert.DeserializeObject<List<Filter>>(value);
                }
            }
        }

        /// <summary>
        /// 固定的过滤条件,用于参数化查询
        /// </summary>
        public string FixedFilterListStr {
            get
            {
                return null;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    FixedFilterList = JsonConvert.DeserializeObject<List<FixedFilter>>(value);
                }
            }
        }

        /// <summary>
        /// 过滤条件集合
        /// </summary>
        public List<Filter> FilterList { get; set; }

        /// <summary>
        /// 固定的过滤条件,用于参数化查询
        /// </summary>
        public List<FixedFilter> FixedFilterList { get; set; }
    }

    /// <summary>
    /// 固定的参数化查询SQL
    /// </summary>
    public class FixedFilter
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParamName { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string ParamValue { get; set; }

        /// <summary>
        /// 字段是否为DateTime类型,如果为DateTime类型则需将FieldValue转换成DateTime进行参数化查询
        /// </summary>
        public bool IsDateTime { get; set; }

    }

    /// <summary>
    /// 列表页面进行查询的过滤条件
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// 过滤条件字段名
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 过滤条件值
        /// </summary>
        public string FieldValue { get; set; }

        /// <summary>
        /// 过滤类型 Like = > ....
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 字段是否为DateTime类型,如果为DateTime类型则需将FieldValue转换成DateTime进行参数化查询
        /// </summary>
        public bool IsDateTime { get; set; }


        private bool isAutoPercent = true;

        /// <summary>
        /// 字段是否为自动添加%匹配,对字符串类型查询时IsAutoPercent为true则会在字符串前后加字符串进行匹配 为false则不进行处理，主要处理通行记录时车牌号码查询记录过大速度慢
        /// </summary>
        public bool IsAutoPercent
        {
            get { return isAutoPercent; }
            set
            {
                isAutoPercent = value;
            }
        }
    }

    public static class QueryConditionExtend
    {
        public static string HandleSortField(string SortField)
        {
            SortField = SortField.Replace(" asc", "").Replace(" desc", "").Trim();
            if (SortField.EndsWith(","))
            {
                SortField = SortField.Substring(0, SortField.Length - 1);
            }
            return SortField;
        }

        /// <summary>
        /// 处理jqgrid 层级列表传输的查询条件排序字段不符合规则
        /// </summary>
        /// <param name="Condition">查询条件</param>
        public static void HandleSortField(this QueryCondition Condition)
        {
            if (Condition != null && !string.IsNullOrEmpty(Condition.SortField))
            {
                Condition.SortField = HandleSortField(Condition.SortField);
            }
        }
    }
}
