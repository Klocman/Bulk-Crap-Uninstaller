using System;
using System.Reflection;
using System.Windows.Forms;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.Forms.Tools;
using Klocman.Tools;

namespace BulkCrapUninstaller.Forms
{
    sealed partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            labelVersion.Text += AssemblyVersion;
            labelCopyright.Text = AssemblyCopyright;
            labelCompanyName.Text = AssemblyCompany;
            labelis64.Text += ProcessTools.Is64BitProcess.ToYesNo();
            labelArchitecture.Text += Assembly.GetExecutingAssembly().GetName().ProcessorArchitecture;
            labelPortable.Text += Program.IsInstalled.ToYesNo();
        }

        public static string AssemblyCompany
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof (AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute) attributes[0]).Company;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute) attributes[0]).Copyright;
            }
        }

        public static Version AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version;

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PremadeDialogs.ProcessStartSafe(@"http://objectlistview.sourceforge.net");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PremadeDialogs.ProcessStartSafe(
                @"http://www.codeproject.com/Articles/20917/Creating-a-Custom-Settings-Provider");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PremadeDialogs.ProcessStartSafe(@"https://github.com/Templarian/WindowsIcons/");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PremadeDialogs.ProcessStartSafe(Resources.HomepageUrl);
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PremadeDialogs.ProcessStartSafe(@"https://nbug.codeplex.com/");
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PremadeDialogs.ProcessStartSafe(@"http://dotnetzip.codeplex.com/");
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PremadeDialogs.ProcessStartSafe(@"http://taskscheduler.codeplex.com/");
        }
    }
}