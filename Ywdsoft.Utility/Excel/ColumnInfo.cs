using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ywdsoft.Utility.Excel
{
    /// <summary>
    /// DataGrid信息
    /// </summary>
    public class ColumnInfo
    {
        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public string Align { get; set; }

        /// <summary>
        /// 标题头
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// 绑定列
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 内容对齐方式
        /// </summary>
        public ICellStyle Style { get; set; }

        /// <summary>
        /// 是否在数据集中存在对应列
        /// </summary>
        public bool IsMapDT { get; set; }
    }
}
