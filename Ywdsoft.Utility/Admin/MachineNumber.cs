using System;
using System.Collections.Generic;
using System.Management;
using System.IO;
using System.Text;
using System.Diagnostics;
using Ywdsoft.Utility.ConfigHandler;

namespace Ywdsoft.Utility
{
    internal enum WmiType
    {
        Win32_Processor,
        Win32_PerfFormattedData_PerfOS_Memory,
        Win32_PhysicalMemory,
        Win32_NetworkAdapterConfiguration,
        Win32_LogicalDisk
    }

    /// <summary>
    /// 获取硬盘号和CPU号
    /// </summary>
    public class MachineNumber
    {
        static Dictionary<string, ManagementObjectCollection> WmiDict = new Dictionary<string, ManagementObjectCollection>();

        static MachineNumber()
        {
            var names = Enum.GetNames(typeof(WmiType));
            foreach (string name in names)
            {
                WmiDict.Add(name, new ManagementObjectSearcher("SELECT * FROM " + name).Get());
            }
        }

        private static double MBSize = 1024.0 * 1024;
        /// <summary>
        /// 获取当前程序相关信息
        /// </summary>
        /// <returns></returns>
        public static dynamic GetMachineInfo()
        {
            Process curProcess = Process.GetCurrentProcess();
            var model = new
            {

                ProgramName = SystemConfig.ProgramName,
                ProcessName = curProcess.ProcessName,
                ProcessId = curProcess.Id,
                CPUUseRate = GetCPUInfo() + "%",//CPU使用率
                HandleCount = curProcess.HandleCount,//句柄数
                ThreadsCount = curProcess.Threads.Count,//线程数
                ProgramRunTime = FormatLongToTimeStr((DateTime.Now - curProcess.StartTime).TotalMilliseconds),//系统运行时间
                WorkSet = Math.Round(curProcess.WorkingSet64 / MBSize, 2),//工作集
                PeakWorkingSet = Math.Round(curProcess.PeakWorkingSet64 / MBSize, 2),//最大物理内存量
                VirtuaMemorySize = Math.Round(curProcess.VirtualMemorySize64 / MBSize, 2),//虚拟内存量
                PeakVirtualMEmorySize = Math.Round(curProcess.PeakVirtualMemorySize64 / MBSize, 2),//最大虚拟内存量
                MemoryInfo = MemoryInfo(),//内存占用
                HardDiskInfo = HardDiskInfo(),//硬盘信息
                CPUCount = Environment.ProcessorCount,//CPU个数
                MachineName = Environment.MachineName,//主机名称
                OSVersion = Environment.OSVersion,//操作系统版本
                SystemDirectory = Environment.SystemDirectory,//操作系统文件夹
                CLRVersion = Environment.Version,//.Net版本
                CurrentDirectory = Environment.CurrentDirectory,//当前程序工作目录
                UserName = Environment.UserName,//当前用户
                UserDomainName = Environment.UserDomainName//当前域
            };

            return model;
        }

        /// <summary>
        /// 获取CPU占用率
        /// </summary>
        /// <returns></returns>
        public static double GetCPUInfo()
        {
            PerformanceCounter pcCpuLoad = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            pcCpuLoad.NextValue();
            System.Threading.Thread.Sleep(100);
            return Math.Round(pcCpuLoad.NextValue(), 2);
        }

        /// <summary>
        /// 获取硬盘信息
        /// </summary>
        /// <returns></returns>
        public static string HardDiskInfo()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            StringBuilder sr = new StringBuilder();
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                {
                    var val1 = (double)drive.TotalSize / (MBSize * 1024.0);
                    var val2 = (double)drive.TotalFreeSpace / (MBSize * 1024.0);
                    if (val2 / val1 < 0.1)
                    {
                        sr.AppendFormat("<strong style='color: red'>{0}({1}G/{2}G);</strong>", drive.Name.Replace("\\", ""), Math.Round(val2, 2), Math.Round(val1, 2));
                    }
                    else
                    {
                        sr.AppendFormat("{0}({1}G/{2}G);", drive.Name.Replace("\\", ""), Math.Round(val2, 2), Math.Round(val1, 2));
                    }
                }
            }
            return sr.ToString();
        }

        /// <summary>
        /// 获取内存信息
        /// </summary>
        /// <returns></returns>
        public static string MemoryInfo()
        {
            double capacity = 0;
            var query = WmiDict[WmiType.Win32_PhysicalMemory.ToString()];
            int index = 1;
            foreach (var obj in query)
            {
                capacity += Convert.ToDouble(obj["Capacity"]);
                index++;
            }
            double Total = capacity / (MBSize * 1024.0);

            query = WmiDict[WmiType.Win32_PerfFormattedData_PerfOS_Memory.ToString()];
            double available = 0;
            foreach (var obj in query)
            {
                available += Convert.ToDouble(obj.Properties["AvailableMBytes"].Value);
            }
            double availableMem = available / 1024.0;

            return Math.Round(Total - availableMem, 2) + "G/" + Math.Round(Total, 2) + "G";
        }

        /// <summary>
        /// 将毫秒转成时分秒格式
        /// </summary>
        /// <param name="l">毫秒</param>
        /// <returns>时分秒格式</returns>
        private static string FormatLongToTimeStr(double l)
        {
            int hour = 0;
            int minute = 0;
            int second = 0;
            second = (int)l / 1000;

            if (second >= 60)
            {
                minute = second / 60;
                second = second % 60;
            }
            if (minute >= 60)
            {
                hour = minute / 60;
                minute = minute % 60;
            }
            return (hour < 10 ? ("0" + hour) : hour.ToString()) + ":" + (minute < 10 ? ("0" + minute) : minute.ToString()) + ":"
                + (second < 10 ? ("0" + second) : second.ToString());
        }
    }
}