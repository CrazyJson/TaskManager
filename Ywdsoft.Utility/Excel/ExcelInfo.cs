using System.Collections.Generic;
using System.Data;

namespace Ywdsoft.Utility.Excel
{
    /// <summary>
    /// Execl相关信息
    /// </summary>
    public class ExcelInfo
    {
        /// <summary>
        /// Execl列信息
        /// </summary>
        public List<ColumnInfo> ColumnInfoList { get; set; }

        /// <summary>
        /// Execl文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 应用程序根目录
        /// </summary>
        public string RootPath { get; set; }

        /// <summary>
        ///获取数据源的方法信息
        /// </summary>
        public string Api { get; set; }

        /// <summary>
        /// 获取数据请求类型 post get
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 查询条件
        /// </summary>
        public QueryCondition Condition { get; set; }

        /// <summary>
        /// 需要导出的数据
        /// </summary>
        public DataTable Data { get; set; }

        private bool isExportSelectData = false;
        /// <summary>
        /// 是否为导出当前选中数据
        /// 如果wei true 则不进行远程查询
        /// </summary>
        public bool IsExportSelectData
        {
            get { return isExportSelectData; }
            set { isExportSelectData = value; }
        }
    }
}
