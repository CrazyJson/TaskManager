/*
 * 模块名: Excel导出模块
 * 作者: 杜冬军
 * 创建日期: 2016/2/23 8:50:22 
 * 博客地址：http://yanweidie.cnblogs.com
 */
using Nancy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ywdsoft.Utility.Excel;
using Newtonsoft.Json;
using Nancy.Helpers;
using Ywdsoft.Utility.Mef;
using Ywdsoft.Utility;
using Ywdsoft.Utility.Http;
using System.Web;

namespace Ywdsoft.Modules
{
    public class ExcelModule : BaseModule
    {
        private static IEnumerable<ExcelImport> AllImports { get; set; }

        public ExcelModule() : base("Excel")
        {
            //任务列表
            Post["/GridExport"] = r =>
            {
                string excelParam = Request.Form["excelParam"];
                if (string.IsNullOrEmpty(excelParam))
                {
                    return "";
                }

                ExcelInfo info = JsonConvert.DeserializeObject<ExcelInfo>(excelParam);
                if (string.IsNullOrEmpty(info.FileName))
                {
                    info.FileName = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ".xls";
                }
                else
                {
                    if (!info.FileName.EndsWith(".xls"))
                    {
                        info.FileName = info.FileName + ".xls";
                    }
                }
                MemoryStream ms = info.ExportExeclStream();
                byte[] msbyte = ms.GetBuffer();
                ms.Dispose();
                ms = null;

                return new Response()
                {
                    Contents = stream => { stream.Write(msbyte, 0, msbyte.Length); },
                    ContentType = "application/msexcel",
                    StatusCode = HttpStatusCode.OK,
                    Headers = new Dictionary<string, string> {
                        { "Content-Disposition", string.Format("attachment;filename={0}", System.Web.HttpUtility.UrlPathEncode(info.FileName)) },
                        {"Content-Length",  msbyte.Length.ToString()}
                    }
                };
            };


            /// <summary>
            /// 导出Excel模版
            /// </summary>
            /// <param name="type">业务类型</param>
            /// <param name="FunctionCode">对应功能模块Code</param>
            /// <returns></returns>
            Get["/DownLoadTemplate"] = r =>
            {
                if (AllImports == null)
                {
                    AllImports = MefConfig.ResolveMany<ExcelImport>();
                }
                string strType = Request.Query["type"];
                ExcelImportType type = EnumHelper.StringToEnum<ExcelImportType>(strType);
                var handler = AllImports.FirstOrDefault(e => e.Type == type);
                if (handler == null)
                {
                    throw new Exception("未找到“" + type.ToString() + "”相应处理模块");
                }

                string path = ExcelImporMapper.GetTemplatePath(type);
                if (File.Exists(path))
                {
                    try
                    {
                        string FileName = Path.GetFileName(path);

                        return new Response()
                        {
                            Contents = stream => { handler.GetExportTemplate(path, stream); },
                            ContentType = MimeHelper.GetMineType(path),
                            StatusCode = HttpStatusCode.OK,
                            Headers = new Dictionary<string, string> {
                                { "Content-Disposition", string.Format("attachment;filename={0}", System.Web.HttpUtility.UrlPathEncode(FileName)) }
                            }
                        };

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    throw new Exception("未找到“" + type.ToString() + "”对应模版文件");
                }
            };

            /// <summary>
            /// 导出Excel模版
            /// </summary>
            /// <returns></returns>
            Post["/ImportTemplate"] = r =>
            {
                ImportResult result = new ImportResult();
                try
                {
                    if (AllImports == null)
                    {
                        AllImports = MefConfig.ResolveMany<ExcelImport>();
                    }
                    string ywType = Request.Query["type"];
                    if (string.IsNullOrEmpty(ywType))
                    {
                        throw new ArgumentNullException("ywType");
                    }
                    //业务类型
                    ExcelImportType type = EnumHelper.StringToEnum<ExcelImportType>(ywType);

                    //文件
                    HttpFile file = Request.Files.First();

                    var handler = AllImports.FirstOrDefault(e => e.Type == type);
                    if (handler == null)
                    {
                        throw new Exception("未找到“" + type.ToString() + "”相应处理模块");
                    }

                    result = handler.ImportTemplate(file.Value, file.Name, null);
                    if (result.IsSuccess)
                    {
                        //是否获取详细数据，决定后台是否返回 result.ExtraInfo
                        string ReturnDetailData = Request.Query["ReturnDetailData"];
                        if (string.IsNullOrEmpty(ReturnDetailData) || ReturnDetailData != "1")
                        {
                            result.ExtraInfo = null;
                        }
                    }
                    else
                    {
                        //设置错误模版http路径
                        result.Message = Request.Url.SiteBase + result.Message;
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = ex.Message;
                    LogHelper.WriteLog("Excel导入异常", ex);
                }
                return Response.AsJson(result);
            };
        }
    }
}