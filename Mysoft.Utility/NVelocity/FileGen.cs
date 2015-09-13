using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NVelocity.App;
using Commons.Collections;
using NVelocity.Runtime;
using NVelocity;
using NVelocity.Context;
using System.IO;
using System.Collections;

namespace Mysoft.Utility
{
    /// <summary>
    /// NVelocity读取模版文件生成
    /// </summary>
    public class FileGen
    {
        /// <summary>
        /// 通过模版文件路径读取文件内容
        /// </summary>
        /// <param name="path">模版文件路径</param>
        /// <param name="ht">模版文件的参数</param>
        /// <returns>StringWriter对象</returns>
        public static StringWriter GetFileText(string path, Hashtable ht)
        {
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("模版文件路径为空！");
            }
            try
            {
                string tmpPath = path.Substring(0,path.LastIndexOf(@"\"));
                string filePath = path.Substring(path.LastIndexOf(@"\")+1);
                //创建NVelocity引擎的实例对象
                VelocityEngine velocity = new VelocityEngine();
                ExtendedProperties props = new ExtendedProperties();
                props.AddProperty(RuntimeConstants.RESOURCE_LOADER, "file");
                props.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, tmpPath);
                props.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_CACHE, true);
                props.AddProperty(RuntimeConstants.INPUT_ENCODING, "utf-8");
                props.AddProperty(RuntimeConstants.OUTPUT_ENCODING, "utf-8");
                velocity.Init(props);
                //从文件中读取模板
                Template temp = velocity.GetTemplate(filePath);
                IContext context = new VelocityContext();
                foreach (string key in ht.Keys)
                {
                    context.Put(key, ht[key]);
                }
                //合并模板
                StringWriter writer = new StringWriter();
                temp.Merge(context, writer);
                return writer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 通过模版文件路径读取文件内容
        /// </summary>
        /// <param name="path">模版文件路径</param>
        /// <param name="ht">模版文件的参数</param>
        /// <param name="strOutputPath">生成文件的输出路径，如c:\1.txt</param>
        /// <returns>TextWriter对象</returns>
        public static void GetFile(string path, Hashtable ht,string strOutputPath)
        {
            if (String.IsNullOrEmpty(strOutputPath))
            {
                throw new ArgumentNullException("模版文件输出路径为空！");
            }
            try
            {
                StringWriter stringW=GetFileText(path, ht);
                StreamWriter sw = new StreamWriter(strOutputPath,false,Encoding.UTF8);
                sw.Write(stringW.ToString());
                sw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
