using System.Collections.Concurrent;

namespace Ywdsoft.Utility.Excel
{
    /// <summary>
    /// Excel导入数据对应类型枚举
    /// </summary>
    public enum ExcelImportType
    {
        /// <summary>
        /// 任务批量添加
        /// </summary>
        Task = 0,   
    }

    public class ExcelImporMapper
    {
        /// <summary>
        /// 业务类型模板文件路径缓存
        /// </summary>
        private static ConcurrentDictionary<ExcelImportType, string> _fileMappingDict = null;

        /// <summary>
        /// 根据业务类型获取模版文件路径
        /// </summary>
        /// <param name="type">业务类型</param>
        /// <returns>模版文件路径</returns>
        public static string GetTemplatePath(ExcelImportType type)
        {
            InitMapping();
            return _fileMappingDict[type];
        }


        /// <summary>
        /// 初始化模版文件路径缓存
        /// </summary>
        private static void InitMapping()
        {
            if (_fileMappingDict == null)
            {
                _fileMappingDict = new ConcurrentDictionary<ExcelImportType, string>();
                _fileMappingDict.TryAdd(ExcelImportType.Task, FileHelper.GetAbsolutePath("/Template/Excel/任务批量导入.xls"));
            }
        }
    }
}
