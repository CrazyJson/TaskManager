using System;
using System.Collections.Concurrent;
using System.IO;

namespace Ywdsoft.Utility.Http
{
    /// <summary>
    /// Mine常见文件类型
    /// </summary>
    public class MimeHelper
    {
        /// <summary>
        /// 拓展类型和mime映射缓存
        /// </summary>
        private static ConcurrentDictionary<string, string> _mimeMappingDict = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 获取常见文件对应MimeType
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <returns>MimeType</returns>
        public static string GetMineType(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("参数filename空异常");
            }
            string ext = Path.GetExtension(fileName).ToLower();
            string result = string.Empty;
            if (_mimeMappingDict.Count == 0)
            {
                InitMapper();
            }
            if (_mimeMappingDict.TryGetValue(ext, out result))
            {
                return result;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 初始化mimeType缓存
        /// </summary>
        private static void InitMapper()
        {
            _mimeMappingDict.TryAdd(".323", "text/h323");
            _mimeMappingDict.TryAdd(".asx", "video/x-ms-asf");
            _mimeMappingDict.TryAdd(".acx", "application/internet-property-stream");
            _mimeMappingDict.TryAdd(".ai", "application/postscript");
            _mimeMappingDict.TryAdd(".aif", "audio/x-aiff");
            _mimeMappingDict.TryAdd(".aiff", "audio/aiff");
            _mimeMappingDict.TryAdd(".axs", "application/olescript");
            _mimeMappingDict.TryAdd(".aifc", "audio/aiff");
            _mimeMappingDict.TryAdd(".asr", "video/x-ms-asf");
            _mimeMappingDict.TryAdd(".avi", "video/x-msvideo");
            _mimeMappingDict.TryAdd(".asf", "video/x-ms-asf");
            _mimeMappingDict.TryAdd(".au", "audio/basic");
            _mimeMappingDict.TryAdd(".application", "application/x-ms-application");
            _mimeMappingDict.TryAdd(".bin", "application/octet-stream");
            _mimeMappingDict.TryAdd(".bas", "text/plain");
            _mimeMappingDict.TryAdd(".bcpio", "application/x-bcpio");
            _mimeMappingDict.TryAdd(".bmp", "image/bmp");
            _mimeMappingDict.TryAdd(".cdf", "application/x-cdf");
            _mimeMappingDict.TryAdd(".cat", "application/vndms-pkiseccat");
            _mimeMappingDict.TryAdd(".crt", "application/x-x509-ca-cert");
            _mimeMappingDict.TryAdd(".c", "text/plain");
            _mimeMappingDict.TryAdd(".css", "text/css");
            _mimeMappingDict.TryAdd(".cer", "application/x-x509-ca-cert");
            _mimeMappingDict.TryAdd(".crl", "application/pkix-crl");
            _mimeMappingDict.TryAdd(".cmx", "image/x-cmx");
            _mimeMappingDict.TryAdd(".csh", "application/x-csh");
            _mimeMappingDict.TryAdd(".cod", "image/cis-cod");
            _mimeMappingDict.TryAdd(".cpio", "application/x-cpio");
            _mimeMappingDict.TryAdd(".clp", "application/x-msclip");
            _mimeMappingDict.TryAdd(".crd", "application/x-mscardfile");
            _mimeMappingDict.TryAdd(".deploy", "application/octet-stream");
            _mimeMappingDict.TryAdd(".dll", "application/x-msdownload");
            _mimeMappingDict.TryAdd(".dot", "application/msword");
            _mimeMappingDict.TryAdd(".doc", "application/msword");
            _mimeMappingDict.TryAdd(".docx", "application/msword");
            _mimeMappingDict.TryAdd(".dvi", "application/x-dvi");
            _mimeMappingDict.TryAdd(".dir", "application/x-director");
            _mimeMappingDict.TryAdd(".dxr", "application/x-director");
            _mimeMappingDict.TryAdd(".der", "application/x-x509-ca-cert");
            _mimeMappingDict.TryAdd(".dib", "image/bmp");
            _mimeMappingDict.TryAdd(".dcr", "application/x-director");
            _mimeMappingDict.TryAdd(".disco", "text/xml");
            _mimeMappingDict.TryAdd(".exe", "application/octet-stream");
            _mimeMappingDict.TryAdd(".etx", "text/x-setext");
            _mimeMappingDict.TryAdd(".evy", "application/envoy");
            _mimeMappingDict.TryAdd(".eml", "message/rfc822");
            _mimeMappingDict.TryAdd(".eps", "application/postscript");
            _mimeMappingDict.TryAdd(".flr", "x-world/x-vrml");
            _mimeMappingDict.TryAdd(".fif", "application/fractals");
            _mimeMappingDict.TryAdd(".gtar", "application/x-gtar");
            _mimeMappingDict.TryAdd(".gif", "image/gif");
            _mimeMappingDict.TryAdd(".gz", "application/x-gzip");
            _mimeMappingDict.TryAdd(".hta", "application/hta");
            _mimeMappingDict.TryAdd(".htc", "text/x-component");
            _mimeMappingDict.TryAdd(".htt", "text/webviewhtml");
            _mimeMappingDict.TryAdd(".h", "text/plain");
            _mimeMappingDict.TryAdd(".hdf", "application/x-hdf");
            _mimeMappingDict.TryAdd(".hlp", "application/winhlp");
            _mimeMappingDict.TryAdd(".html", "text/html");
            _mimeMappingDict.TryAdd(".htm", "text/html");
            _mimeMappingDict.TryAdd(".hqx", "application/mac-binhex40");
            _mimeMappingDict.TryAdd(".isp", "application/x-internet-signup");
            _mimeMappingDict.TryAdd(".iii", "application/x-iphone");
            _mimeMappingDict.TryAdd(".ief", "image/ief");
            _mimeMappingDict.TryAdd(".ivf", "video/x-ivf");
            _mimeMappingDict.TryAdd(".ins", "application/x-internet-signup");
            _mimeMappingDict.TryAdd(".ico", "image/x-icon");
            _mimeMappingDict.TryAdd(".jpg", "image/jpeg");
            _mimeMappingDict.TryAdd(".jfif", "image/pjpeg");
            _mimeMappingDict.TryAdd(".jpe", "image/jpeg");
            _mimeMappingDict.TryAdd(".jpeg", "image/jpeg");
            _mimeMappingDict.TryAdd(".js", "application/x-javascript");
            _mimeMappingDict.TryAdd(".lsx", "video/x-la-asf");
            _mimeMappingDict.TryAdd(".latex", "application/x-latex");
            _mimeMappingDict.TryAdd(".lsf", "video/x-la-asf");
            _mimeMappingDict.TryAdd(".manifest", "application/x-ms-manifest");
            _mimeMappingDict.TryAdd(".mhtml", "message/rfc822");
            _mimeMappingDict.TryAdd(".mny", "application/x-msmoney");
            _mimeMappingDict.TryAdd(".mht", "message/rfc822");
            _mimeMappingDict.TryAdd(".mid", "audio/mid");
            _mimeMappingDict.TryAdd(".mpv2", "video/mpeg");
            _mimeMappingDict.TryAdd(".man", "application/x-troff-man");
            _mimeMappingDict.TryAdd(".mvb", "application/x-msmediaview");
            _mimeMappingDict.TryAdd(".mpeg", "video/mpeg");
            _mimeMappingDict.TryAdd(".m3u", "audio/x-mpegurl");
            _mimeMappingDict.TryAdd(".mdb", "application/x-msaccess");
            _mimeMappingDict.TryAdd(".mpp", "application/vnd.ms-project");
            _mimeMappingDict.TryAdd(".m1v", "video/mpeg");
            _mimeMappingDict.TryAdd(".mpa", "video/mpeg");
            _mimeMappingDict.TryAdd(".me", "application/x-troff-me");
            _mimeMappingDict.TryAdd(".m13", "application/x-msmediaview");
            _mimeMappingDict.TryAdd(".movie", "video/x-sgi-movie");
            _mimeMappingDict.TryAdd(".m14", "application/x-msmediaview");
            _mimeMappingDict.TryAdd(".mpe", "video/mpeg");
            _mimeMappingDict.TryAdd(".mp2", "video/mpeg");
            _mimeMappingDict.TryAdd(".mov", "video/quicktime");
            _mimeMappingDict.TryAdd(".mp3", "audio/mpeg");
            _mimeMappingDict.TryAdd(".mpg", "video/mpeg");
            _mimeMappingDict.TryAdd(".ms", "application/x-troff-ms");
            _mimeMappingDict.TryAdd(".nc", "application/x-netcdf");
            _mimeMappingDict.TryAdd(".nws", "message/rfc822");
            _mimeMappingDict.TryAdd(".oda", "application/oda");
            _mimeMappingDict.TryAdd(".ods", "application/oleobject");
            _mimeMappingDict.TryAdd(".pmc", "application/x-perfmon");
            _mimeMappingDict.TryAdd(".p7r", "application/x-pkcs7-certreqresp");
            _mimeMappingDict.TryAdd(".p7b", "application/x-pkcs7-certificates");
            _mimeMappingDict.TryAdd(".p7s", "application/pkcs7-signature");
            _mimeMappingDict.TryAdd(".pmw", "application/x-perfmon");
            _mimeMappingDict.TryAdd(".ps", "application/postscript");
            _mimeMappingDict.TryAdd(".p7c", "application/pkcs7-mime");
            _mimeMappingDict.TryAdd(".pbm", "image/x-portable-bitmap");
            _mimeMappingDict.TryAdd(".ppm", "image/x-portable-pixmap");
            _mimeMappingDict.TryAdd(".pub", "application/x-mspublisher");
            _mimeMappingDict.TryAdd(".pnm", "image/x-portable-anymap");
            _mimeMappingDict.TryAdd(".png", "image/png");
            _mimeMappingDict.TryAdd(".pml", "application/x-perfmon");
            _mimeMappingDict.TryAdd(".p10", "application/pkcs10");
            _mimeMappingDict.TryAdd(".pfx", "application/x-pkcs12");
            _mimeMappingDict.TryAdd(".p12", "application/x-pkcs12");
            _mimeMappingDict.TryAdd(".pdf", "application/pdf");
            _mimeMappingDict.TryAdd(".pps", "application/mspowerpoint");
            _mimeMappingDict.TryAdd(".p7m", "application/pkcs7-mime");
            _mimeMappingDict.TryAdd(".pko", "application/vndms-pkipko");
            _mimeMappingDict.TryAdd(".ppt", "application/mspowerpoint");
            _mimeMappingDict.TryAdd(".pptx", "application/mspowerpoint");
            _mimeMappingDict.TryAdd(".pmr", "application/x-perfmon");
            _mimeMappingDict.TryAdd(".pma", "application/x-perfmon");
            _mimeMappingDict.TryAdd(".pot", "application/mspowerpoint");
            _mimeMappingDict.TryAdd(".prf", "application/pics-rules");
            _mimeMappingDict.TryAdd(".pgm", "image/x-portable-graymap");
            _mimeMappingDict.TryAdd(".qt", "video/quicktime");
            _mimeMappingDict.TryAdd(".ra", "audio/x-pn-realaudio");
            _mimeMappingDict.TryAdd(".rgb", "image/x-rgb");
            _mimeMappingDict.TryAdd(".ram", "audio/x-pn-realaudio");
            _mimeMappingDict.TryAdd(".rmi", "audio/mid");
            _mimeMappingDict.TryAdd(".ras", "image/x-cmu-raster");
            _mimeMappingDict.TryAdd(".roff", "application/x-troff");
            _mimeMappingDict.TryAdd(".rtf", "application/rtf");
            _mimeMappingDict.TryAdd(".rtx", "text/richtext");
            _mimeMappingDict.TryAdd(".sv4crc", "application/x-sv4crc");
            _mimeMappingDict.TryAdd(".spc", "application/x-pkcs7-certificates");
            _mimeMappingDict.TryAdd(".setreg", "application/set-registration-initiation");
            _mimeMappingDict.TryAdd(".snd", "audio/basic");
            _mimeMappingDict.TryAdd(".stl", "application/vndms-pkistl");
            _mimeMappingDict.TryAdd(".setpay", "application/set-payment-initiation");
            _mimeMappingDict.TryAdd(".stm", "text/html");
            _mimeMappingDict.TryAdd(".shar", "application/x-shar");
            _mimeMappingDict.TryAdd(".sh", "application/x-sh");
            _mimeMappingDict.TryAdd(".sit", "application/x-stuffit");
            _mimeMappingDict.TryAdd(".spl", "application/futuresplash");
            _mimeMappingDict.TryAdd(".sct", "text/scriptlet");
            _mimeMappingDict.TryAdd(".scd", "application/x-msschedule");
            _mimeMappingDict.TryAdd(".sst", "application/vndms-pkicertstore");
            _mimeMappingDict.TryAdd(".src", "application/x-wais-source");
            _mimeMappingDict.TryAdd(".sv4cpio", "application/x-sv4cpio");
            _mimeMappingDict.TryAdd(".tex", "application/x-tex");
            _mimeMappingDict.TryAdd(".tgz", "application/x-compressed");
            _mimeMappingDict.TryAdd(".t", "application/x-troff");
            _mimeMappingDict.TryAdd(".tar", "application/x-tar");
            _mimeMappingDict.TryAdd(".tr", "application/x-troff");
            _mimeMappingDict.TryAdd(".tif", "image/tiff");
            _mimeMappingDict.TryAdd(".txt", "text/plain");
            _mimeMappingDict.TryAdd(".texinfo", "application/x-texinfo");
            _mimeMappingDict.TryAdd(".trm", "application/x-msterminal");
            _mimeMappingDict.TryAdd(".tiff", "image/tiff");
            _mimeMappingDict.TryAdd(".tcl", "application/x-tcl");
            _mimeMappingDict.TryAdd(".texi", "application/x-texinfo");
            _mimeMappingDict.TryAdd(".tsv", "text/tab-separated-values");
            _mimeMappingDict.TryAdd(".ustar", "application/x-ustar");
            _mimeMappingDict.TryAdd(".uls", "text/iuls");
            _mimeMappingDict.TryAdd(".vcf", "text/x-vcard");
            _mimeMappingDict.TryAdd(".wps", "application/vnd.ms-works");
            _mimeMappingDict.TryAdd(".wav", "audio/wav");
            _mimeMappingDict.TryAdd(".wrz", "x-world/x-vrml");
            _mimeMappingDict.TryAdd(".wri", "application/x-mswrite");
            _mimeMappingDict.TryAdd(".wks", "application/vnd.ms-works");
            _mimeMappingDict.TryAdd(".wmf", "application/x-msmetafile");
            _mimeMappingDict.TryAdd(".wcm", "application/vnd.ms-works");
            _mimeMappingDict.TryAdd(".wrl", "x-world/x-vrml");
            _mimeMappingDict.TryAdd(".wdb", "application/vnd.ms-works");
            _mimeMappingDict.TryAdd(".wsdl", "text/xml");
            _mimeMappingDict.TryAdd(".xap", "application/x-silverlight-app");
            _mimeMappingDict.TryAdd(".xml", "text/xml");
            _mimeMappingDict.TryAdd(".xlm", "application/msexcel");
            _mimeMappingDict.TryAdd(".xaf", "x-world/x-vrml");
            _mimeMappingDict.TryAdd(".xla", "application/msexcel");
            _mimeMappingDict.TryAdd(".xls", "application/msexcel");
            _mimeMappingDict.TryAdd(".xlsx", "application/msexcel");
            _mimeMappingDict.TryAdd(".xof", "x-world/x-vrml");
            _mimeMappingDict.TryAdd(".xlt", "application/msexcel");
            _mimeMappingDict.TryAdd(".xlc", "application/msexcel");
            _mimeMappingDict.TryAdd(".xsl", "text/xml");
            _mimeMappingDict.TryAdd(".xbm", "image/x-xbitmap");
            _mimeMappingDict.TryAdd(".xlw", "application/msexcel");
            _mimeMappingDict.TryAdd(".xpm", "image/x-xpixmap");
            _mimeMappingDict.TryAdd(".xwd", "image/x-xwindowdump");
            _mimeMappingDict.TryAdd(".xsd", "text/xml");
            _mimeMappingDict.TryAdd(".z", "application/x-compress");
            _mimeMappingDict.TryAdd(".zip", "application/x-zip-compressed");
            _mimeMappingDict.TryAdd(".*", "application/octet-stream");
        }
    }
}
