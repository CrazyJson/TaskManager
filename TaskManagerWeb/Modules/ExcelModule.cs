/*
 * 模块名: Excel导出模块
 * 作者: 杜冬军
 * 创建日期: 2016/2/23 8:50:22 
 * 博客地址：http://yanweidie.cnblogs.com
 */
using Ywdsoft.Utility;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ywdsoft.Utility.Excel;
using Newtonsoft.Json;
using Nancy.Helpers;

namespace Ywdsoft.Modules
{
    public class ExcelModule : NancyModule
    {
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

                return new Response() {
                    Contents = stream => { stream.Write(msbyte, 0, msbyte.Length); },
                    ContentType= "application/msexcel",
                    StatusCode= HttpStatusCode.OK,
                    Headers=new Dictionary<string, string> {
                        { "Content-Disposition", string.Format("attachment;filename={0}", HttpUtility.UrlPathEncode(info.FileName)) },
                        {"Content-Length",  msbyte.Length.ToString()}
                    }
                };
            };
        }
    }
}