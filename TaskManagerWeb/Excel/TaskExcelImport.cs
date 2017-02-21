using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;
using System.ComponentModel.Composition;
using Ywdsoft.Utility.Excel;
using Ywdsoft.Utility.Auth;
using Ywdsoft.Utility;

namespace Ywdsoft.Excel
{
    /// <summary>
    /// 设备批量注册服务
    /// </summary>
    [Export(typeof(ExcelImport))]
    public class TaskExcelImport : ExcelImport
    {
        /// <summary>
        /// 任务状态缓存字典
        /// </summary>
        private static Dictionary<string, string> GetStatusDict()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            //任务状态类型下拉选择
            dict["运行"] = "0";
            dict["停止"] = "1";
            return dict;
        }

        /// <summary>
        ///下拉选项校验
        /// </summary>
        /// <param name="e">校验参数</param>
        /// <returns>错误信息</returns>
        private static string SelectVerify(ImportVerifyParam e, object extra)
        {
            string result = "";
            result = ExcelImportHelper.GetCellMsg(e.CellValue, e.ColName, 0, true);
            if (string.IsNullOrEmpty(result))
            {
                var dict = extra as Dictionary<string, string>;
                if (dict != null)
                {
                    if (!dict.ContainsKey(e.CellValue.ToString()))
                    {
                        result += e.ColName + "下拉选项" + e.CellValue + "不存在";
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///Cron表达式校验
        /// </summary>
        /// <param name="e">校验参数</param>
        /// <returns>错误信息</returns>
        private static string CronVerify(ImportVerifyParam e, object extra)
        {
            string result = "";
            result = ExcelImportHelper.GetCellMsg(e.CellValue, e.ColName, 200, true, true);
            if (string.IsNullOrEmpty(result))
            {
                if (!QuartzHelper.ValidExpression(e.CellValue.ToString()))
                {
                    result += "Cron表达式格式不正确";
                }
            }
            return result;
        }

        /// <summary>
        /// Excel字段映射
        /// </summary>
        private static Dictionary<string, ImportVerify> dictFields = new List<ImportVerify> {
            new ImportVerify{ ColumnName= "任务名称",FieldName="TaskName",VerifyFunc =(e,extra)=> ExcelImportHelper.GetCellMsg(e.CellValue,e.ColName,300,true,true)},
            new ImportVerify{ ColumnName="Cron表达式",FieldName="CronExpressionString",VerifyFunc =CronVerify},
            new ImportVerify{ ColumnName="表达式说明",FieldName="CronRemark",VerifyFunc=(e,extra)=> ExcelImportHelper.GetCellMsg(e.CellValue,e.ColName,300,true,true) },
            new ImportVerify{ ColumnName="任务状态",FieldName="Status" ,VerifyFunc =SelectVerify},
            new ImportVerify{ ColumnName="程序集名称",FieldName="Assembly",VerifyFunc =(e,extra)=> ExcelImportHelper.GetCellMsg(e.CellValue,e.ColName,150,true,true)},
            new ImportVerify{ ColumnName="类名(包含命名空间)",FieldName="Class" ,VerifyFunc =(e,extra)=> ExcelImportHelper.GetCellMsg(e.CellValue,e.ColName,150,true,true)},
            new ImportVerify{ ColumnName="备注",FieldName="Remark",VerifyFunc =(e,extra)=>ExcelImportHelper.GetCellMsg(e.CellValue, e.ColName, 1000, false,true)}
        }.ToDictionary(e => e.ColumnName, e => e);

        #region "override方法"
        /// <summary>
        /// 业务类型
        /// </summary>
        public override ExcelImportType Type
        {
            get
            {
                return ExcelImportType.Task;
            }
        }

        /// <summary>
        /// Excel字段映射及校验缓存
        /// </summary>
        /// <returns>字段映射</returns>
        public override Dictionary<string, ImportVerify> DictFields
        {
            get
            {
                return dictFields;
            }
        }

        /// <summary>
        ///返回对应的导出模版数据
        /// </summary>
        /// <param name="FilePath">模版的路径</param>
        /// <param name="s">响应流</param>
        /// <returns>模版MemoryStream</returns>
        public override void GetExportTemplate(string FilePath, Stream s)
        {
            //写入下拉框值 任务状态
            var sheet = NPOIHelper.GetFirstSheet(FilePath);

            string[] taskStatus = GetStatusDict().Keys.ToArray();

            int dataRowIndex = StartRowIndex + 1;
            NPOIHelper.SetHSSFValidation(sheet, taskStatus, dataRowIndex, 3);

            sheet.Workbook.Write(s);
        }

        /// <summary>
        /// 获取额外的校验所需信息
        /// </summary>
        /// <param name="listColumn">所有列名集合</param>
        /// <param name="dt">dt</param>
        /// <returns>额外信息</returns>
        /// <remarks>
        /// 例如导入excel中含有下拉框 导入时需要判断选项值是否还存在，可以通过该方法查询选项值
        /// </remarks>
        public override Dictionary<string, object> GetExtraInfo(List<string> listColumn, DataTable dt)
        {
            Dictionary<string, object> extraInfo = new Dictionary<string, object>();
            foreach (string name in listColumn)
            {
                switch (name)
                {
                    case "Status":
                        extraInfo[name] = GetStatusDict();
                        break;
                    default:
                        break;
                }
            }
            return extraInfo;
        }

        /// <summary>
        /// 批量保存数据
        /// </summary>
        /// <param name="dt">数据</param>
        /// <param name="extraInfo">额外参数</param>
        /// <param name="userInfo">用户信息</param>
        public override object SaveImportData(DataTable dt, Dictionary<string, object> extraInfo, UserInfo userInfo)
        {
            string columnName = string.Empty;
            object objExtra = null;
            Dictionary<string, string> dict = null;
            object objCellValue = null;

            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    columnName = dc.ColumnName;
                    if (extraInfo.TryGetValue(columnName, out objExtra))
                    {
                        dict = objExtra as Dictionary<string, string>;
                        if (dict != null)
                        {
                            objCellValue = dr[columnName];
                            if (!ExcelImportHelper.ObjectIsNullOrEmpty(objCellValue))
                            {
                                dr[columnName] = dict[objCellValue.ToString()];
                            }
                        }
                    }
                }
            }

            try
            {
                //保存任务数据
                List<TaskUtil> list = dt.ToList<TaskUtil>();
                foreach (var item in list)
                {
                    TaskHelper.SaveTask(item);
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
