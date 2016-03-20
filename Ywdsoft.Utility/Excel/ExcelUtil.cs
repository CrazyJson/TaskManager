using CsharpHttpHelper;
using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;

namespace Ywdsoft.Utility.Excel
{
    /// <summary>
    /// Execl操作帮助类
    /// </summary>
    public static class ExcelUtil
    {
        private static HttpHelper http = new HttpHelper();

        /// <summary>
        /// 拓展方法,生成EXECL
        /// </summary>
        /// <param name="info">EXECL相关信息</param>
        /// <returns>Execl路径</returns>
        public static string ExportExecl(this ExcelInfo info)
        {
            //1.获取列表对应数据

            //2.创建Execl文档

            //3.列映射

            //4.保存Execl
            return "/temp/" + info.FileName + ".xls";
        }

        /// <summary>
        /// 拓展方法,生成EXECL
        /// </summary>
        /// <param name="info">EXECL相关信息</param>
        /// <param name="token">用户认证令牌</param>
        /// <returns>Execl路径</returns>
        public static MemoryStream ExportExeclStream(this ExcelInfo info)
        {
            //1.获取列表对应数据
            DataTable dt = GetGirdData(info);
            //2.创建Execl文档
            return NPOIHelper.Export(dt, info.ColumnInfoList);
        }


        /// <summary>
        /// 从WebAPI中获取列表数据
        /// </summary>
        /// <returns></returns>
        private static DataTable GetGirdData(ExcelInfo info)
        {
            if (info.IsExportSelectData)
            {
                if (info.Data == null)
                {
                    info.Data = new DataTable();
                }
                return info.Data;
            }
            try
            {
                if (info.Type.Equals("post", StringComparison.OrdinalIgnoreCase))
                {
                    //创建Httphelper参数对象
                    HttpItem item = new HttpItem()
                    {
                        URL = info.Api,//URL     必需项    
                        Method = "post",//可选项 默认为Get   
                        ContentType = "application/json; charset=utf-8",//返回类型
                        Postdata = JsonConvert.SerializeObject(info.Condition)
                    };

                    //请求的返回值对象
                    HttpResult result = http.GetHtml(item);
                    var responseJson = JsonConvert.DeserializeObject<JsonBaseModel<DataTable>>(result.Html);
                    if (!responseJson.HasError && responseJson.Message != "logout")
                    {
                        return responseJson.Result;
                    }
                    else
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add(info.ColumnInfoList[0].Field);
                        DataRow dr = dt.NewRow();
                        //接口报错
                        if (responseJson.HasError)
                        {
                            dr[0] = responseJson.Message;
                        }
                        if (responseJson.Message == "logout")
                        {
                            dr[0] = "登录超时,请刷新页面重试";
                        }
                        dt.Rows.Add(dr);
                        return dt;
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("不支持Get协议获取数据");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
