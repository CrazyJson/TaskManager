using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace Ywdsoft.Utility.Excel
{
    /// <summary>
    /// NPOI操作EXECL帮助类
    /// </summary>
    public static class NPOIHelper
    {
        /// <summary>
        /// EXECL最大列宽
        /// </summary>
        private static readonly int MAX_COLUMN_WIDTH = 100 * 256;
        public static void ExportToFile(DataSet dataSet, string fileFullPath)
        {
            List<DataTable> dts = new List<DataTable>();
            foreach (DataTable dt in dataSet.Tables) dts.Add(dt);
            ExportToFile(dts, fileFullPath);
        }
        public static void ExportToFile(DataTable dataTable, string fileFullPath)
        {
            List<DataTable> dts = new List<DataTable>();
            dts.Add(dataTable);
            ExportToFile(dts, fileFullPath);
        }

        /// <summary>
        /// 生成EXECL文件，通过读取DataTable和列头映射信息
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="ColumnInfoList">列字段映射信息</param>
        /// <returns>文件流</returns>
        public static MemoryStream Export(DataTable dt, List<ColumnInfo> ColumnInfoList)
        {
            if (dt == null || ColumnInfoList == null)
            {
                throw new ArgumentNullException();
            }
            int rowHeight = 20;
            //每个标签页最多行数
            int sheetRow = 65536;
            HSSFWorkbook workbook = new HSSFWorkbook();

            //文本样式
            ICellStyle centerStyle = workbook.CreateCellStyle();
            centerStyle.VerticalAlignment = VerticalAlignment.CENTER;
            centerStyle.Alignment = HorizontalAlignment.CENTER;

            ICellStyle leftStyle = workbook.CreateCellStyle();
            leftStyle.VerticalAlignment = VerticalAlignment.CENTER;
            leftStyle.Alignment = HorizontalAlignment.LEFT;

            ICellStyle rightStyle = workbook.CreateCellStyle();
            rightStyle.VerticalAlignment = VerticalAlignment.CENTER;
            rightStyle.Alignment = HorizontalAlignment.RIGHT;

            //寻找列头和DataTable之间映射关系
            foreach (DataColumn col in dt.Columns)
            {
                ColumnInfo info = ColumnInfoList.FirstOrDefault<ColumnInfo>(e => e.Field.Equals(col.ColumnName, StringComparison.OrdinalIgnoreCase));
                if (info != null)
                {
                    switch (info.Align.ToLower())
                    {
                        case "left":
                            info.Style = leftStyle;
                            break;
                        case "center":
                            info.Style = centerStyle;
                            break;
                        case "right":
                            info.Style = rightStyle;
                            break;
                    }
                    info.IsMapDT = true;
                }
            }

            int sheetNum = (int)Math.Ceiling(dt.Rows.Count * 1.0 / 65536);
            //最多生成5个标签页的数据
            sheetNum = sheetNum > 3 ? 3 : (sheetNum == 0 ? 1 : sheetNum);
            ICell cell = null;
            object cellValue = null;
            for (int sheetIndex = 0; sheetIndex < sheetNum; sheetIndex++)
            {
                ISheet sheet = workbook.CreateSheet();
                sheet.CreateFreezePane(0, 1, 0, 1);
                //输出表头
                IRow headerRow = sheet.CreateRow(0);
                //设置行高
                headerRow.HeightInPoints = rowHeight;
                //首行样式
                ICellStyle HeaderStyle = workbook.CreateCellStyle();
                HeaderStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                HeaderStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
                IFont font = workbook.CreateFont();
                font.Boldweight = short.MaxValue;
                HeaderStyle.SetFont(font);
                HeaderStyle.VerticalAlignment = VerticalAlignment.CENTER; ;
                HeaderStyle.Alignment = HorizontalAlignment.CENTER;


                //输出表头信息 并设置表头样式
                int i = 0;
                foreach (var data in ColumnInfoList)
                {
                    cell = headerRow.CreateCell(i);
                    cell.SetCellValue(data.Header.Trim());
                    cell.CellStyle = HeaderStyle;
                    i++;
                }

                //开始循环所有行
                int iRow = 1;

                int startRow = sheetIndex * (sheetRow - 1);
                int endRow = (sheetIndex + 1) * (sheetRow - 1);
                endRow = endRow <= dt.Rows.Count ? endRow : dt.Rows.Count;

                for (int rowIndex = startRow; rowIndex < endRow; rowIndex++)
                {
                    IRow row = sheet.CreateRow(iRow);
                    row.HeightInPoints = rowHeight;
                    i = 0;
                    foreach (var item in ColumnInfoList)
                    {
                        cell = row.CreateCell(i);
                        if (item.IsMapDT)
                        {
                            cellValue = dt.Rows[rowIndex][item.Field];
                            cell.SetCellValue(cellValue != DBNull.Value ? cellValue.ToString() : string.Empty);
                            cell.CellStyle = item.Style;
                        }
                        i++;
                    }
                    iRow++;
                }

                //自适应列宽度
                for (int j = 0; j < ColumnInfoList.Count; j++)
                {
                    sheet.AutoSizeColumn(j);
                    int width = sheet.GetColumnWidth(j) + 2560;
                    sheet.SetColumnWidth(j, width > MAX_COLUMN_WIDTH ? MAX_COLUMN_WIDTH : width);
                }
            }

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            return ms;
        }

        public static void ExportToFile(IEnumerable<DataTable> dataTables, string fileFullPath)
        {
            IWorkbook workbook = new HSSFWorkbook();
            int i = 0;
            foreach (DataTable dt in dataTables)
            {
                string sheetName = string.IsNullOrEmpty(dt.TableName)
                    ? "Sheet " + (++i).ToString()
                    : dt.TableName;
                ISheet sheet = workbook.CreateSheet(sheetName);

                IRow headerRow = sheet.CreateRow(0);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string columnName = string.IsNullOrEmpty(dt.Columns[j].ColumnName)
                        ? "Column " + j.ToString()
                        : dt.Columns[j].ColumnName;
                    headerRow.CreateCell(j).SetCellValue(columnName);
                }

                for (int a = 0; a < dt.Rows.Count; a++)
                {
                    DataRow dr = dt.Rows[a];
                    IRow row = sheet.CreateRow(a + 1);
                    for (int b = 0; b < dt.Columns.Count; b++)
                    {
                        row.CreateCell(b).SetCellValue(dr[b] != DBNull.Value ? dr[b].ToString() : string.Empty);
                    }
                }
            }

            using (FileStream fs = File.Create(fileFullPath))
            {
                workbook.Write(fs);
            }
        }

        /// <summary>
        /// 从EXECL中读取数据 转换成DataTable
        /// 每个sheet页对应一个DataTable
        /// </summary>
        /// <param name="xlsxFile">文件路径</param>
        /// <returns></returns>
        public static List<DataTable> GetDataTablesFrom(string xlsxFile)
        {
            if (!File.Exists(xlsxFile))
                throw new FileNotFoundException("文件不存在");

            List<DataTable> result = new List<DataTable>();
            Stream stream = new MemoryStream(File.ReadAllBytes(xlsxFile));
            IWorkbook workbook = new HSSFWorkbook(stream);
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                DataTable dt = new DataTable();
                ISheet sheet = workbook.GetSheetAt(i);
                IRow headerRow = sheet.GetRow(0);

                int cellCount = headerRow.LastCellNum;
                for (int j = headerRow.FirstCellNum; j < cellCount; j++)
                {
                    DataColumn column = new DataColumn(headerRow.GetCell(j).StringCellValue);
                    dt.Columns.Add(column);
                }

                int rowCount = sheet.LastRowNum;
                for (int a = (sheet.FirstRowNum + 1); a < rowCount; a++)
                {
                    IRow row = sheet.GetRow(a);
                    if (row == null) continue;

                    DataRow dr = dt.NewRow();
                    for (int b = row.FirstCellNum; b < cellCount; b++)
                    {
                        if (row.GetCell(b) == null) continue;
                        dr[b] = row.GetCell(b).ToString();
                    }

                    dt.Rows.Add(dr);
                }
                result.Add(dt);
            }
            stream.Close();

            return result;
        }


        #region "私有方法"

        #endregion
    }

    /// <summary>
    /// NPOI拓展方法
    /// </summary>
    public static class NPOIExtend
    {
        /// <summary>
        /// 获取RGB对应NPOI颜色值
        /// </summary>
        /// <param name="workbook">当前wb</param>
        /// <param name="R"></param>
        /// <param name="G"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static short GetXLColour(this HSSFWorkbook workbook, int R, int G, int B)
        {
            short s = 0;
            HSSFPalette XlPalette = workbook.GetCustomPalette();
            HSSFColor XlColour = XlPalette.FindColor((byte)R, (byte)G, (byte)B);
            if (XlColour == null)
            {
                if (NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE < 255)
                {
                    if (NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE < 64)
                    {
                        NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE = 64;
                        NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE += 1;
                        XlColour = XlPalette.AddColor((byte)R, (byte)G, (byte)B);
                    }
                    else
                    {
                        XlColour = XlPalette.FindSimilarColor((byte)R, (byte)G, (byte)B);
                    }

                    s = XlColour.GetIndex();
                }
            }
            else
            {
                s = XlColour.GetIndex();
            }
            return s;
        }

        /// <summary>
        /// 冻结表格
        /// </summary>
        /// <param name="sheet">sheet</param>
        /// <param name="colCount">冻结的列数</param>
        /// <param name="rowCount">冻结的行数</param>
        /// <param name="startCol">右边区域可见的首列序号，从1开始计算</param>
        /// <param name="startRow">下边区域可见的首行序号，也是从1开始计算</param>
        /// <example>
        /// sheet1.CreateFreezePane(0, 1, 0, 1); 冻结首行
        /// sheet1.CreateFreezePane(1, 0, 1, 0);冻结首列
        /// </example>
        public static void FreezePane(this ISheet sheet, int colCount, int rowCount, int startCol, int startRow)
        {
            sheet.CreateFreezePane(colCount, rowCount, startCol, startRow);
        }
    }
}

