using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Reflection;

namespace WSWinForm
{
    public partial class FrmMain : Form
    {

        public FrmMain(string[] args)
        {
            InitializeComponent();
            if (args != null && args.Length>0)
            {
                txtPath.Text = args[0];
            }
        }

        /// <summary>
        /// 操作前校验
        /// </summary>
        /// <returns></returns>
        private bool Vaild()
        {
            if (String.IsNullOrEmpty(txtPath.Text.Trim()))
            {
                txtTip.Text = "请先选择Windows服务路径！";
                return false;
            }
            if (!File.Exists(txtPath.Text.Trim()))
            {
                txtTip.Text = "路径不存在！";
                return false;
            }
            if (!txtPath.Text.Trim().EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                txtTip.Text = "所选文件不是Windows服务！";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取Windows服务的名称
        /// </summary>
        /// <param name="serviceFileName">文件路径</param>
        /// <returns>服务名称</returns>
        private string GetServiceName(string serviceFileName)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(serviceFileName);
                Type[] types = assembly.GetTypes();
                foreach (Type myType in types)
                {
                    if (myType.IsClass && myType.BaseType == typeof(System.Configuration.Install.Installer))
                    {
                        FieldInfo[] fieldInfos = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Default | BindingFlags.Instance | BindingFlags.Static);
                        foreach (FieldInfo myFieldInfo in fieldInfos)
                        {
                            if (myFieldInfo.FieldType == typeof(System.ServiceProcess.ServiceInstaller))
                            {
                                ServiceInstaller serviceInstaller = (ServiceInstaller)myFieldInfo.GetValue(Activator.CreateInstance(myType));
                                return serviceInstaller.ServiceName;
                            }
                        }
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 判断服务是否已经存在
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>bool</returns>
        private bool ServiceIsExisted(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController s in services)
            {
                if (s.ServiceName == serviceName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 安装服务
        /// </summary>
        private void btnInstall_Click(object sender, EventArgs e)
        {
            if (!Vaild())
            {
                return;
            }
            try
            {
                string[] cmdline = { };
                string serviceFileName = txtPath.Text.Trim();
                string serviceName = GetServiceName(serviceFileName);
                if (string.IsNullOrEmpty(serviceName))
                {
                    txtTip.Text = "指定文件不是Windows服务！";
                    return;
                }
                if (ServiceIsExisted(serviceName))
                {
                    txtTip.Text = "要安装的服务已经存在！";
                    return;
                }
                TransactedInstaller transactedInstaller = new TransactedInstaller();
                AssemblyInstaller assemblyInstaller = new AssemblyInstaller(serviceFileName, cmdline);
                assemblyInstaller.UseNewContext = true;
                transactedInstaller.Installers.Add(assemblyInstaller);
                transactedInstaller.Install(new System.Collections.Hashtable());
                txtTip.Text = "服务安装成功！";
            }
            catch (Exception ex)
            {
                txtTip.Text = ex.Message;
            }
        }

        /// <summary>
        ///运行服务
        /// </summary>
        private void btnRun_Click(object sender, EventArgs e)
        {
            if (!Vaild())
            {
                return;
            }
            try
            {
                string serviceName = GetServiceName(txtPath.Text.Trim());
                if (string.IsNullOrEmpty(serviceName))
                {
                    txtTip.Text = "指定文件不是Windows服务！";
                    return;
                }
                if (!ServiceIsExisted(serviceName))
                {
                    txtTip.Text = "要运行的服务不存在！";
                    return;
                }
                ServiceController service = new ServiceController(serviceName);
                if (service.Status != ServiceControllerStatus.Running && service.Status != ServiceControllerStatus.StartPending)
                {
                    service.Start();
                    txtTip.Text = "服务运行成功！";
                }
                else
                {
                    txtTip.Text = "服务正在运行！";
                }
            }
            catch (Exception ex)
            {
                txtTip.Text = ex.Message;
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (!Vaild())
            {
                return;
            }
            try
            {
                string[] cmdline = { };
                string serviceFileName = txtPath.Text.Trim();
                string serviceName = GetServiceName(serviceFileName);
                if (string.IsNullOrEmpty(serviceName))
                {
                    txtTip.Text = "指定文件不是Windows服务！";
                    return;
                }
                if (!ServiceIsExisted(serviceName))
                {
                    txtTip.Text = "要停止的服务不存在！";
                    return;
                }
                ServiceController service = new ServiceController(serviceName);
                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    txtTip.Text = "服务停止成功！";
                }
                else
                {
                    txtTip.Text = "服务已经停止！";
                }

            }
            catch (Exception ex)
            {
                txtTip.Text = ex.Message;
            }
        }

        /// <summary>
        /// 卸载服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUninstall_Click(object sender, EventArgs e)
        {
            if (!Vaild())
            {
                return;
            }
            try
            {
                string serviceFileName = txtPath.Text.Trim();
                string serviceName = GetServiceName(serviceFileName);
                if (string.IsNullOrEmpty(serviceName))
                {
                    txtTip.Text = "指定文件不是Windows服务！";
                    return;
                }
                if (!ServiceIsExisted(serviceName))
                {
                    txtTip.Text = "要卸载的服务不存在！";
                    return;
                }
                //让主线程去访问设置提示信息
                if (mBackgroundWorker.IsBusy)
                {
                    MessageBox.Show("当前进程正在卸载服务，请等待本次操作完成！");
                    return;
                }
                //后台运行卸载服务
                mBackgroundWorker.RunWorkerAsync(serviceFileName);               
            }
            catch (Exception ex)
            {
                txtTip.Text = ex.Message;
            }
        }



        /// <summary>
        /// 卸载服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string serviceFileName = (string)e.Argument;
            string[] cmdline = { };
            BackgroundWorker bw = (BackgroundWorker)sender;
            bw.ReportProgress(0, "正在卸载服务，请稍等....");


            TransactedInstaller transactedInstaller = new TransactedInstaller();
            AssemblyInstaller assemblyInstaller = new AssemblyInstaller(serviceFileName, cmdline);
            transactedInstaller.Installers.Add(assemblyInstaller);
            transactedInstaller.Uninstall(null);
            bw.ReportProgress(0, "服务卸载成功！");
        }

        private void mBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            txtTip.Text = (string)e.UserState;
            if (txtTip.Text == "服务卸载成功！")
            {
                System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location, txtPath.Text.Trim());
                this.Close();
            }
        }


        /// <summary>
        /// 选择文件
        /// </summary>
        private void btnBrowser_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "(*.exe)|*.exe";
            openFileDialog1.FileName = "Mysoft.CRE.WindowService.exe";
            openFileDialog1.Title = "选择Windows服务";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = openFileDialog1.FileName;
            }
        }
    }
}
