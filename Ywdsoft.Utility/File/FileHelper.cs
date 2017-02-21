using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Web;

namespace Ywdsoft.Utility
{
    /// <summary>
    /// 文件相关助手类
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 获取文件的绝对路径,针对window程序和web程序都可使用
        /// </summary>
        /// <param name="relativePath">相对路径地址</param>
        /// <returns>绝对路径地址</returns>
        public static string GetAbsolutePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                throw new ArgumentNullException("参数relativePath空异常！");
            }
            relativePath = relativePath.Replace("/", "\\");
            if (relativePath[0] == '\\')
            {
                relativePath = relativePath.Remove(0, 1);
            }
            //判断是Web程序还是window程序
            if (HttpContext.Current != null)
            {
                return Path.Combine(HttpRuntime.AppDomainAppPath, relativePath);
            }
            else
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            }
        }

        /// <summary>
        /// 获取文件的绝对路径,针对window程序和web程序都可使用
        /// </summary>
        /// <param name="relativePath">相对路径地址</param>
        /// <returns>绝对路径地址</returns>
        public static string GetRootPath()
        {
            //判断是Web程序还是window程序
            if (HttpContext.Current != null)
            {
                return HttpRuntime.AppDomainAppPath;
            }
            else
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        /// <summary>
        /// 通过文件Hash 比较两个文件内容是否相同
        /// </summary>
        /// <param name="filePath1">文件1地址</param>
        /// <param name="filePath2">文件2地址</param>
        /// <returns></returns>
        public static bool isValidFileContent(string filePath1, string filePath2)
        {
            //创建一个哈希算法对象 
            using (HashAlgorithm hash = HashAlgorithm.Create())
            {
                using (FileStream file1 = new FileStream(filePath1, FileMode.Open), file2 = new FileStream(filePath2, FileMode.Open))
                {
                    byte[] hashByte1 = hash.ComputeHash(file1);//哈希算法根据文本得到哈希码的字节数组 
                    byte[] hashByte2 = hash.ComputeHash(file2);
                    string str1 = BitConverter.ToString(hashByte1);//将字节数组装换为字符串 
                    string str2 = BitConverter.ToString(hashByte2);
                    return (str1 == str2);//比较哈希码 
                }
            }
        }

        /// <summary>
        /// 计算文件的hash值 用于比较两个文件是否相同
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件hash值</returns>
        public static string GetFileHash(string filePath)
        {
            //创建一个哈希算法对象 
            using (HashAlgorithm hash = HashAlgorithm.Create())
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open))
                {
                    //哈希算法根据文本得到哈希码的字节数组 
                    byte[] hashByte = hash.ComputeHash(file);
                    //将字节数组装换为字符串  
                    return BitConverter.ToString(hashByte);
                }
            }
        }

        /// <summary>
        /// 复制一个文件夹下的所有文件到另外一个文件夹
        /// </summary>
        /// <param name="sourceDirName">源文件夹目录</param>
        /// <param name="destDirName">目标文件夹路径</param>
        public static void CopyDirectory(string sourceDirName, string destDirName)
        {
            try
            {
                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                    File.SetAttributes(destDirName, File.GetAttributes(sourceDirName));

                }

                if (destDirName[destDirName.Length - 1] != Path.DirectorySeparatorChar)
                    destDirName = destDirName + Path.DirectorySeparatorChar;

                string[] files = Directory.GetFiles(sourceDirName);
                foreach (string file in files)
                {
                    if (File.Exists(destDirName + Path.GetFileName(file)))
                        continue;
                    File.Copy(file, destDirName + Path.GetFileName(file), true);
                    File.SetAttributes(destDirName + Path.GetFileName(file), FileAttributes.Normal);
                }

                string[] dirs = Directory.GetDirectories(sourceDirName);
                foreach (string dir in dirs)
                {
                    CopyDirectory(dir, destDirName + Path.GetFileName(dir));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 复制一个文件夹下指定的文件到另外一个文件夹
        /// </summary>
        /// <param name="list">指定文件</param>
        /// <param name="destDirName">目标文件夹路径</param>
        public static void CopyFiles(List<string> list, string destDirName)
        {
            try
            {
                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                }

                if (destDirName[destDirName.Length - 1] != Path.DirectorySeparatorChar)
                {
                    destDirName = destDirName + Path.DirectorySeparatorChar;
                }


                string[] files = list.ToArray();
                foreach (string file in files)
                {
                    if (File.Exists(destDirName + Path.GetFileName(file)))
                        continue;
                    File.Copy(file, destDirName + Path.GetFileName(file), true);
                    File.SetAttributes(destDirName + Path.GetFileName(file), FileAttributes.Normal);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
