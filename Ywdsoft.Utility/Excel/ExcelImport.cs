using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Ywdsoft.Utility.Auth;

namespace Ywdsoft.Utility.Excel
{
    /// <summary>
    /// excel导入处理
    /// </summary>
    public abstract class ExcelImport
    {
        private static int BUFFER_SIZE = 0x10000;

        /// <summary>
        /// 获取相应业务类型，系统会根据该类型寻找对应模块的处理类
        /// </summary>
        /// <returns>业务类型</returns>
        public abstract ExcelImportType Type { get; }

        /// <summary>
        /// Excel字段映射及校验缓存
        /// </summary>
        /// <returns>字段映射</returns>
        public abstract Dictionary<string, ImportVerify> DictFields { get; }

        /// <summary>
        /// 起始行索引-标题行
        /// </summary>
        /// <returns>起始行索引</returns>
        public virtual int StartRowIndex
        {
            get
            {
                return 5;
            }
        }

        /// <summary>
        ///返回对应的导出模版数据
        /// </summary>
        /// <param name="FilePath">模版的路径</param>
        /// <param name="s">响应流</param>
        /// <returns>模版MemoryStream</returns>
        public virtual void GetExportTemplate(string FilePath, Stream s)
        {
            byte[] m_buffer = new byte[BUFFER_SIZE];
            int count = 0;
            using (FileStream fs = File.OpenRead(FilePath))
            {
                do
                {
                    count = fs.Read(m_buffer, 0, BUFFER_SIZE);
                    s.Write(m_buffer, 0, count);
                } while (count == BUFFER_SIZE);
            }
        }

        /// <summary>
        ///从上传文件流中读取数据 保存为datatable
        /// </summary>
        /// <param name="ins">输入流</param>
        /// <param name="datasheet">数据得sheet表格</param>
        /// <returns>数据</returns>
        public virtual DataTable GetDataFromExcel(Stream ins, out ISheet datasheet)
        {
            return NPOIHelper.GetDataFromExcel(ins, out datasheet);
        }

        /// <summary>
        ///返回对应的导出模版数据
        /// </summary>
        /// <param name="ins">导入文件流</param>
        /// <param name="fileName">文件名</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns>ImportResult</returns>
        public virtual ImportResult ImportTemplate(Stream ins, string fileName, UserInfo userInfo)
        {
            if (DictFields == null)
            {
                throw new ArgumentNullException("Excel字段映射及校验缓存字典DictFields空异常");
            }
            //1.读取数据
            ISheet datasheet = null;
            DataTable dt = GetDataFromExcel(ins, out datasheet);

            //2.校验列是否正确
            //相同列数
            int equalCount = (from p in GetColumnList(dt)
                              join q in DictFields.Keys
                              on p equals q
                              select p).Count();
            if (equalCount < DictFields.Keys.Count)
            {
                throw new Exception(string.Format("模版列和规定的不一致,正确的列为（{0}）", string.Join(",", DictFields.Keys)));
            }


            //2.改变列名为英文字段名
            ImportVerify objVerify = null;
            List<string> columns = new List<string>();
            List<string> removeColumns = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                if (DictFields.TryGetValue(dc.ColumnName, out objVerify))
                {
                    if (objVerify != null)
                    {
                        dc.ColumnName = objVerify.FieldName;
                        columns.Add(objVerify.FieldName);
                        continue;
                    }
                }
                removeColumns.Add(dc.ColumnName);
            }
            //3.删除无效列
            foreach (string remove in removeColumns)
            {
                dt.Columns.Remove(remove);
            }

            //4.获取校验所需额外参数
            Dictionary<string, object> extraInfo = GetExtraInfo(columns, dt);

            // 英文字段名到中文列名映射关系
            Dictionary<string, ImportVerify> DictColumnFields = DictFields.Values.ToDictionary(e => e.FieldName, e => e);

            //5.开始校验
            ImportResult result = Verify(dt, datasheet, extraInfo, userInfo, fileName, DictColumnFields);

            if (result.IsSuccess)
            {
                //校验完成后进行数据类型转换
                ImportVerify iv = null;
                Type columnType = null;
                DataTable dtNew = dt.Clone();
                foreach (DataColumn dc in dtNew.Columns)
                {
                    if (DictColumnFields != null && DictColumnFields.TryGetValue(dc.ColumnName, out iv))
                    {
                        if (iv.DataType != null)
                        {
                            columnType = iv.DataType;
                        }
                        else
                        {
                            columnType = dc.DataType;
                        }
                    }
                    else
                    {
                        columnType = typeof(string);
                    }
                    dc.DataType = columnType;
                }
                //复制数据到克隆的datatable里  
                try
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dtNew.ImportRow(dr);
                    }
                }
                catch { }

                //3.保存数据
                result.ExtraInfo = SaveImportData(dtNew, extraInfo, userInfo);
                result.Message = string.Format("成功导入{0}条数据", dtNew.Rows.Count);
            }
            return result;
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
        public virtual Dictionary<string, object> GetExtraInfo(List<string> listColumn, DataTable dt)
        {
            return null;
        }

        /// <summary>
        /// 校验数据是否正常
        /// </summary>
        /// <param name="dt">数据集</param>
        /// <param name="outputStream">输出流</param>
        /// <param name="sheet">数据sheet</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="DictColumnFields">英文字段名到中文列名映射关系</param>
        /// <returns>ImportResult</returns>
        public virtual ImportResult Verify(DataTable dt, ISheet sheet, Dictionary<string, object> extraInfo, UserInfo userInfo, string fileName, Dictionary<string, ImportVerify> DictColumnFields)
        {
            IWorkbook wb = sheet.Workbook;
            ImportResult result = new ImportResult();

            string[] arrErrorMsg = null;
            string errorMsg = string.Empty;
            int columnCount = dt.Columns.Count;
            string columnName = string.Empty;
            ImportVerify objVerify = null;
            ImportVerifyParam objVerifyParam = new ImportVerifyParam { DTExcel = dt, CellValue = null, ColName = columnName, ColumnIndex = 0, RowIndex = 0 };
            DataRow row = null;
            object objExtra = null;
            bool isCorrect = true;

            //错误数据行样式
            var cellErrorStyle = NPOIHelper.GetErrorCellStyle(wb);
            ICell errorCell = null;
            IRow sheetRow = null;

            for (int i = 0, rLength = dt.Rows.Count; i < rLength; i++)
            {
                row = dt.Rows[i];
                arrErrorMsg = new string[columnCount];
                for (int j = 0; j < columnCount; j++)
                {
                    columnName = dt.Columns[j].ColumnName;
                    if (DictColumnFields.TryGetValue(columnName, out objVerify))
                    {
                        if (objVerify.VerifyFunc != null)
                        {
                            objVerifyParam.CellValue = row[j];
                            objVerifyParam.ColumnIndex = j;
                            objVerifyParam.RowIndex = i;
                            objVerifyParam.ColName = objVerify.ColumnName;
                            if (extraInfo != null)
                            {
                                extraInfo.TryGetValue(columnName, out objExtra);
                            }
                            arrErrorMsg[j] = objVerify.VerifyFunc(objVerifyParam, objExtra);
                        }
                    }
                }
                errorMsg = string.Join("，", arrErrorMsg.Where(e => !string.IsNullOrEmpty(e)));
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    isCorrect = false;
                    //设置错误信息
                    sheetRow = sheet.GetRow(StartRowIndex + 1 + i);
                    errorCell = sheetRow.GetCell(columnCount);
                    if (errorCell == null)
                    {
                        errorCell = sheetRow.CreateCell(columnCount);
                    }
                    errorCell.CellStyle = cellErrorStyle;
                    errorCell.SetCellValue(errorMsg);
                }
            }

            //输出错误信息模版
            if (!isCorrect)
            {
                sheetRow = sheet.GetRow(StartRowIndex);
                errorCell = sheetRow.GetCell(columnCount);
                if (errorCell == null)
                {
                    errorCell = sheetRow.CreateCell(columnCount);
                }
                ICellStyle copyStyle = sheetRow.GetCell(columnCount - 1).CellStyle;
                ICellStyle style = NPOIHelper.GetErrorHeadCellStyle(wb);
                IFont font = style.GetFont(wb);
                IFont copyfont = copyStyle.GetFont(wb);
                font.FontHeight = copyfont.FontHeight;
                font.FontName = copyfont.FontName;
                style.FillForegroundColor = copyStyle.FillForegroundColor;
                style.BorderBottom = copyStyle.BorderBottom;
                style.BorderLeft = copyStyle.BorderLeft;
                style.BorderRight = copyStyle.BorderRight;
                style.BorderTop = copyStyle.BorderTop;
                errorCell.CellStyle = style;
                errorCell.SetCellValue("错误信息");

                //自适应列宽度
                sheet.AutoSizeColumn(columnCount);
                int width = sheet.GetColumnWidth(columnCount) + 2560;
                sheet.SetColumnWidth(columnCount, width > NPOIHelper.MAX_COLUMN_WIDTH ? NPOIHelper.MAX_COLUMN_WIDTH : width);

                result.Message = ExcelImportHelper.GetErrorExcel(wb, fileName);
            }
            else
            {
                result.IsSuccess = true;
            }
            return result;
        }

        /// <summary>
        /// 批量保存数据
        /// </summary>
        /// <param name="dt">数据，可以调用CPQuery.MultiInsert(strSQL,dt)方法进行批量保存</param>
        /// <param name="extraInfo">额外参数</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns>返回的额外数据信息，用于导入查询后台返回excel数据使用</returns>
        public abstract object SaveImportData(DataTable dt, Dictionary<string, object> extraInfo, UserInfo userInfo);

        /// <summary>
        /// 获取DateTable列名List集合
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>列名List集合</returns>
        public List<string> GetColumnList(DataTable dt)
        {
            List<string> columns = new List<string>();
            foreach (DataColumn column in dt.Columns)
            {
                columns.Add(column.ColumnName);
            }
            return columns;
        }
    }

    /// <summary>
    /// 字段校验及映射信息
    /// </summary>
    public class ImportVerify
    {
        /// <summary>
        /// Excel列名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 数据库字段名
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 定义列的数据类型 typeof(System.DateTime)
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// 字段校验函数
        /// </summary>
        public Func<ImportVerifyParam, object, string> VerifyFunc { get; set; }
    }

    /// <summary>
    /// Excel导入公共类
    /// </summary>
    public class ExcelImportHelper
    {
        /// <summary>
        /// 判断对象是否为空
        /// </summary>
        /// <param name="obj">待判断对象</param>
        /// <returns>bool</returns>
        public static bool ObjectIsNullOrEmpty(object obj)
        {
            return obj == null || string.IsNullOrEmpty(obj.ToString());
        }

        /// <summary>
        /// 获取错误信息Excel
        /// </summary>
        /// <param name="wb">excel对象</param>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        public static string GetErrorExcel(IWorkbook wb, string fileName)
        {
            string ext = Path.GetExtension(fileName);
            string name = Path.GetFileNameWithoutExtension(fileName);

            string dirPath = FileHelper.GetAbsolutePath("/TempFile/ErrorExcel");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            string relativePath = string.Format("/TempFile/ErrorExcel/{0}{1}{2}", name, DateTime.Now.ToString("MMddHHmmss"), ext);
            string path = FileHelper.GetAbsolutePath(relativePath);

            using (FileStream fs = File.OpenWrite(path))
            {
                wb.Write(fs);
            }
            return relativePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellValue"></param>
        /// <param name="colName"></param>
        /// <param name="MaxLength"></param>
        /// <param name="Required"></param>
        /// <param name="isNChar"></param>
        /// <returns></returns>
        public static string GetCellMsg(object cellValue, string colName, int MaxLength = 0, bool Required = false, bool isNChar = false)
        {
            bool empty = ObjectIsNullOrEmpty(cellValue);
            if (Required && empty)
            {
                return colName + "必填";
            }
            if (MaxLength > 0 && !empty)
            {
                int length = isNChar ? cellValue.ToString().Length : GetLength(cellValue.ToString(), 3);
                if (length > MaxLength)
                {
                    return colName + "最大长度为" + MaxLength;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取字符串长度。与string.Length不同的是，该方法将中文作 x 个字符计算。
        /// </summary>
        /// <param name="str">目标字符串</param>
        /// <param name="chinaLength">中文作x个字符 默认为2个</param>
        /// <returns>实际长度</returns>
        public static int GetLength(string str, int chinaLength = 2)
        {
            if (str == null || str.Length == 0) { return 0; }

            int l = str.Length;
            int realLen = l;

            #region 计算长度
            int clen = 0;//当前长度
            while (clen < l)
            {
                //每遇到一个中文，则将实际长度加一。
                if ((int)str[clen] > 128) { realLen++; }
                clen++;
            }
            #endregion

            return realLen;
        }

    }

    /// <summary>
    /// 校验参数信息
    /// </summary>
    public class ImportVerifyParam
    {
        /// <summary>
        /// Excel数据源
        /// </summary>
        public DataTable DTExcel { get; set; }

        /// <summary>
        /// 行索引
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 列索引
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColName { get; set; }

        /// <summary>
        /// 列值
        /// </summary>
        public object CellValue { get; set; }
    }

    /// <summary>
    /// excel导入结果
    /// </summary>
    public class ImportResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 提示消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 额外数据信息
        /// </summary>
        public object ExtraInfo { get; set; }
    }
}
