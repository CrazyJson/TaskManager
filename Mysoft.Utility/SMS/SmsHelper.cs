using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using CsharpHttpHelper;
using Newtonsoft.Json.Linq;

namespace Mysoft.Utility
{
    /// <summary>
    /// 短信发送,采用乐云短信平台每天可以免费发送100条,同一接收人5分钟内只能发送两条
    /// http://apistore.baidu.com/apiworks/servicedetail/774.html
    /// </summary>
    public class SmsHelper
    {
        /// <summary>
        /// 发送接口地址
        /// </summary>
        private static readonly string SmsAPI = "http://apis.baidu.com/hunanlehuotechnologyco/sms/api";

        /// <summary>
        /// APIKEY
        /// </summary>
        private static readonly string Apikey = "63728eb0cc4807c8b4fc87084eab90e0";

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="receiver">短信接收人手机号码</param>
        /// <param name="content">短信内容</param>
        /// <returns>发送状态</returns>
        public static SMSCode SendMessage(string receiver, string content)
        {
            try
            {
                //创建Httphelper对象
                HttpHelper http = new HttpHelper();
                //创建Httphelper参数对象
                HttpItem item = new HttpItem()
                {
                    URL = string.Format("{0}?phone={1}&content=验证码:{2}", SmsAPI, receiver, content),//URL     必需项    
                    Method = "get",//可选项 默认为Get   
                    ContentType = "text/plain"//返回类型    可选项有默认值 ,
                };
                item.Header.Add("apikey", Apikey);
                //请求的返回值对象
                HttpResult result = http.GetHtml(item);
                JObject jo = JObject.Parse(result.Html);
                JToken value = null;
                if (jo.TryGetValue("result", out value))
                {
                    return EnumHelper.IntToEnum<SMSCode>(Convert.ToInt32(value.ToString()));
                }
                return SMSCode.SystemBusy;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("短信发送失败", ex);
                return SMSCode.Exception;
            }
        }
    }

    /// <summary>
    /// 请求结果Code枚举
    /// </summary>
    public enum SMSCode
    {
        #region "验证码短信接口调用方错误码"
        /// <summary>
        /// 参数不正确，需要重新核定参数
        /// </summary>
        [Description("参数不正确，需要重新核定参数")]
        ParameterError = -8,

        /// <summary>
        /// 内容中含有敏感词，需去掉敏感词
        /// </summary>
        [Description("内容中含有敏感词，需去掉敏感词")]
        ContentSensitive = -9,

        /// <summary>
        /// 接口调用成功，短信已经提交到运营商
        /// </summary>
        [Description("接口调用成功，短信已经提交到运营商")]
        Success = 0,

        /// <summary>
        /// 调用发送异常，可能是参数问题
        /// </summary>
        [Description("调用发送异常，可能是参数问题")]
        Exception = 1,

        #endregion

        #region "代理平台错误"
        /// <summary>
        /// 内部错误
        /// </summary>
        [Description("内部错误")]
        InternalError = 300301,

        /// <summary>
        /// 系统繁忙稍候再试
        /// </summary>
        [Description("系统繁忙稍候再试")]
        SystemBusy = 300302,

        #endregion

        #region "调用方错误"
        /// <summary>
        /// url无法解析
        /// </summary>
        [Description("url无法解析")]
        URLResolved = 300201,

        /// <summary>
        /// 请求缺少apikey，登录即可获取
        /// </summary>
        [Description("请求缺少apikey，登录即可获取")]
        MissingApikey = 300202,

        /// <summary>
        /// 服务没有取到apikey或secretkey
        /// </summary>
        [Description("服务没有取到apikey或secretkey")]
        ApikeyNULL = 300203,

        /// <summary>
        /// apikey不存在
        /// </summary>
        [Description("apikey不存在")]
        ApikeyNotExist = 300204,

        /// <summary>
        /// api不存在
        /// </summary
        [Description("api不存在")]
        ApiNotExist = 300205,

        /// <summary>
        /// api已关闭服务
        /// </summary>
        [Description("api已关闭服务")]
        ApiOutService = 300206,

        #endregion

        #region "限制类错误"

        /// <summary>
        /// 用户请求过期
        /// </summary>
        [Description("用户请求过期")]
        RequestExpired = 300101,

        /// <summary>
        /// 用户日调用量超限
        /// </summary>
        [Description("用户日调用量超限")]
        UserCallOverrunPerDay = 300102,

        /// <summary>
        /// 服务每秒调用量超限
        /// </summary>
        [Description("服务每秒调用量超限")]
        ServiceCallOverrunPerSecond = 300103,

        /// <summary>
        /// 服务日调用量超限
        /// </summary>
        [Description("服务日调用量超限")]
        ServiceCallOverrunPerDay = 300104

        #endregion
    }
}
