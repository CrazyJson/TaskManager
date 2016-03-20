/*
 * Model: 模块名称
 * Desctiption: 描述
 * Author: 杜冬军
 * Created: 2016/2/23 9:00:30 
 * Copyright：武汉中科通达高新技术股份有限公司
 */

using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Responses;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ywdsoft.Utility;

namespace Ywdsoft
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            pipelines.AfterRequest += ctx =>
            {
                // 如果返回状态吗码为 Unauthorized 跳转到登陆界面
                if (ctx.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ctx.Response = new RedirectResponse("/login?returnUrl=" + Uri.EscapeDataString(ctx.Request.Path));
                }
                else if (ctx.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    ctx.Response = new RedirectResponse("/Error/NotFound?returnUrl=" + Uri.EscapeDataString(ctx.Request.Path));
                }
            };

            pipelines.OnError += Error;
        }

        protected override IRootPathProvider RootPathProvider
        {
            get { return new CustomRootPathProvider(); }
        }

        /// <summary>
        /// 配置静态文件访问权限
        /// </summary>
        /// <param name="conventions"></param>
        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);

            ///静态文件夹访问 设置 css,js,image
            conventions.StaticContentsConventions.AddDirectory("Content");
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            //替换默认序列化方式
            container.Register<ISerializer, CustomJsonNetSerializer>();
        }

        private dynamic Error(NancyContext context, Exception ex)
        {
            //可以使用log4net记录异常 ex 这里直接返回异常信息
            LogHelper.WriteLog("Web站点请求异常", ex);
            return ex.Message;
        }
    }

    internal class CustomRootPathProvider : IRootPathProvider
    {
        private static readonly string ROOT_PATH = AppDomain.CurrentDomain.BaseDirectory;
        public string GetRootPath()
        {
            return ROOT_PATH;
        }
    }

    /// <summary>
    /// 使用Newtonsoft.Json 替换Nancy默认的序列化方式
    /// </summary>
    public class CustomJsonNetSerializer : JsonSerializer, ISerializer
    {
        public CustomJsonNetSerializer()
        {
            ContractResolver = new DefaultContractResolver();
            DateFormatHandling = DateFormatHandling.IsoDateFormat;
            Formatting = Formatting.None;
            NullValueHandling = NullValueHandling.Ignore;
        }

        public bool CanSerialize(string contentType)
        {
            return contentType.Equals("application/json", StringComparison.OrdinalIgnoreCase);
        }

        public void Serialize<TModel>(string contentType, TModel model, Stream outputStream)
        {
            using (var streamWriter = new StreamWriter(outputStream))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                Serialize(jsonWriter, model);
            }
        }

        public IEnumerable<string> Extensions { get { yield return "json"; } }
    }
}