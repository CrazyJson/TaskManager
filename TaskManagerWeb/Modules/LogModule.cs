/*
 * 模块名: 日志模块
 * 描述: 日志模块
 * 作者: 杜冬军
 * 创建日期: 2016/2/23 8:50:22 
 * 博客地址：http://yanweidie.cnblogs.com
 */

using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ywdsoft.Utility;
using Ywdsoft.Utility.Command;
using Ywdsoft.Utility.ConfigHandler;
using Ywdsoft.Utility.Http;

namespace Ywdsoft.Modules
{
    public class LogModule : NancyModule
    {
        private static int BUFFER_SIZE = 0x10000;

        public LogModule() : base("Log")
        {
            //下载日志
            Get["/Download"] = r =>
            {
                string FileName = string.Format("log-{0}.zip", DateTime.Now.ToString("yyyyMMddHHmmss"));

                return new Response()
                {
                    Contents = stream => { LogCmdHelper.DowloadLog(FileName, stream); },
                    ContentType = MimeHelper.GetMineType(FileName),
                    StatusCode = HttpStatusCode.OK,
                    Headers = new Dictionary<string, string> {
                                { "Content-Disposition", string.Format("attachment;filename={0}", System.Web.HttpUtility.UrlPathEncode(FileName)) }
                            }
                };
            };

            //查看单个日志文件详情
            Get["/LogDetail"] = r =>
            {
                string filePath = Request.Query["name"];
                if (string.IsNullOrEmpty(filePath))
                {
                    return Response.AsText("文件路径为空", "text/plain;charset=UTF-8");
                }
                else
                {
                    if (File.Exists(filePath))
                    {
                        string filename = Path.GetFileName(filePath);
                        string newFileName = Guid.NewGuid().ToString("N") + filename;
                        string newFilePath = filePath.Substring(0, filePath.LastIndexOf("\\") + 1) + newFileName;
                        try
                        {
                            File.Copy(filePath, newFilePath, true);
                            string text = string.Empty;
                            using (StreamReader sr = new StreamReader(newFilePath, Encoding.Default))
                            {
                                text = sr.ReadToEnd();
                            }
                            File.Delete(newFilePath);
                            return Response.AsText(text, "text/plain;charset=UTF-8");
                        }
                        catch (Exception ex)
                        {
                            return Response.AsText(ex.ToString(), "text/plain;charset=UTF-8");
                        }
                        finally
                        {
                            if (File.Exists(newFilePath))
                            {
                                File.Delete(newFilePath);
                            }
                        }
                    }
                    else
                    {
                        return Response.AsText("文件不存在", "text/plain;charset=UTF-8");
                    }
                }
            };

            //下载单个日志文件
            Get["/SingleDownload"] = r =>
            {
                string filePath = Request.Query["name"];
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    return Response.AsText("文件不存在", "text/plain;charset=UTF-8");
                }
                string FileName = Path.GetFileName(filePath);

                return new Response()
                {
                    Contents = stream =>
                    {
                        byte[] m_buffer = new byte[BUFFER_SIZE];
                        int count = 0;
                        using (FileStream fs = File.OpenRead(filePath))
                        {
                            do
                            {
                                count = fs.Read(m_buffer, 0, BUFFER_SIZE);
                                stream.Write(m_buffer, 0, count);
                            } while (count == BUFFER_SIZE);
                        }
                    },
                    ContentType = MimeHelper.GetMineType(FileName),
                    StatusCode = HttpStatusCode.OK,
                    Headers = new Dictionary<string, string> {
                                { "Content-Disposition", string.Format("attachment;filename={0}", System.Web.HttpUtility.UrlPathEncode(FileName)) }
                    }
                };
            };

            //下载指定日志文件
            Post["/DownloadSomeLog"] = r =>
            {
                string FileName = string.Format("log-{0}.zip", DateTime.Now.ToString("yyyyMMddHHmmss"));
                string strFileList = Request.Form["FileList"];
                if (string.IsNullOrEmpty(strFileList))
                {
                    return "";
                }
                List<string> list = strFileList.Split('|').ToList();

                return new Response()
                {
                    Contents = stream => { LogCmdHelper.DowloadLog(FileName, list, stream); },
                    ContentType = MimeHelper.GetMineType(FileName),
                    StatusCode = HttpStatusCode.OK,
                    Headers = new Dictionary<string, string> {
                                { "Content-Disposition", string.Format("attachment;filename={0}", System.Web.HttpUtility.UrlPathEncode(FileName)) }
                            }
                };
            };

            #region "前端接口"

            //日志列表查询接口
            Post["/PostQuery"] = r =>
            {
                QueryCondition condition = this.Bind<QueryCondition>();

                //进行过滤
                string searchPattern = "*";
                if (condition.FilterList != null && condition.FilterList.Count > 0)
                {
                    var LogTypeFilter = condition.FilterList.FirstOrDefault(e => e.FieldName.Equals("LogType", StringComparison.CurrentCultureIgnoreCase));
                    if (LogTypeFilter != null && !string.IsNullOrEmpty(LogTypeFilter.FieldValue))
                    {
                        if (LogTypeFilter.FieldValue == "0")
                        {
                            searchPattern = "log-*";
                        }
                        else if (LogTypeFilter.FieldValue == "1")
                        {
                            searchPattern = "error-*";
                        }
                    }
                }

                var files = Directory.GetFiles(FileHelper.GetAbsolutePath("Logs"), searchPattern, SearchOption.AllDirectories);

                ApiResult<List<dynamic>> result = new ApiResult<List<dynamic>>();
                List<dynamic> list = new List<dynamic>();
                FileInfo fi = null;
                foreach (var item in files)
                {
                    fi = new FileInfo(item);
                    var file = new
                    {
                        FileName = Path.GetFileName(item),
                        FileSize = Math.Round(fi.Length / (1024 * 1.0), 2),
                        CreatedOn = fi.CreationTime,
                        FilePath = item,
                        Link = string.Format("http://127.0.0.1:{0}/{1}", SystemConfig.WebPort, item.Substring(AppDomain.CurrentDomain.BaseDirectory.Length).Replace("\\", "/"))
                    };
                    list.Add(file);
                }
                //条件过滤
                if (condition.FilterList != null && condition.FilterList.Count > 0)
                {
                    foreach (var filter in condition.FilterList)
                    {
                        if (!string.IsNullOrEmpty(filter.FieldValue))
                        {
                            switch (filter.FieldName)
                            {
                                case "FileName":
                                    list = list.Where(e => e.FileName.Contains(filter.FieldValue)).ToList();
                                    break;
                                case "FileSizeStart":
                                    list = list.Where(e => e.FileSize >= Convert.ToDouble(filter.FieldValue)).ToList();
                                    break;
                                case "FileSizeEnd":
                                    list = list.Where(e => e.FileSize <= Convert.ToDouble(filter.FieldValue)).ToList();
                                    break;
                                case "CreatedOnStart":
                                    list = list.Where(e => e.CreatedOn >= Convert.ToDateTime(filter.FieldValue)).ToList();
                                    break;
                                case "CreatedOnEnd":
                                    list = list.Where(e => e.CreatedOn <= Convert.ToDateTime(filter.FieldValue)).ToList();
                                    break;
                            }
                        }
                    }
                }
                //排序
                if (string.IsNullOrEmpty(condition.SortField))
                {
                    condition.SortField = "CreatedOn";
                    condition.SortOrder = "desc";
                }
                if (condition.SortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    switch (condition.SortField)
                    {
                        case "FileName":
                            list = list.OrderByDescending(e => e.FileName).ToList();
                            break;
                        case "FileSize":
                            list = list.OrderByDescending(e => e.FileSize).ToList();
                            break;
                        case "CreatedOn":
                            list = list.OrderByDescending(e => e.CreatedOn).ToList();
                            break;
                    }
                }
                else
                {
                    switch (condition.SortField)
                    {
                        case "FileName":
                            list = list.OrderBy(e => e.FileName).ToList();
                            break;
                        case "FileSize":
                            list = list.OrderBy(e => e.FileSize).ToList();
                            break;
                        case "CreatedOn":
                            list = list.OrderBy(e => e.CreatedOn).ToList();
                            break;
                    }
                }
                //分页查询
                list = list.Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToList();

                result.Result = list;
                result.TotalCount = Convert.ToInt32(list.Count);
                result.TotalPage = result.CalculateTotalPage(condition.PageSize, result.TotalCount.Value, condition.IsPagination);
                return Response.AsJson(result);
            };

            #endregion
        }
    }
}