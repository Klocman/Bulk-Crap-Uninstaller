// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Full.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Windows.Forms;
using NBug.Core.Reporting.Info;
using NBug.Core.Util.Serialization;
using NBug.Properties;

namespace NBug.Core.UI.WinForms
{
    internal partial class Full : Form
    {
        private UIDialogResult uiDialogResult;

        internal Full()
        {
            InitializeComponent();
            Icon = Resources.NBug_icon_16;
            warningLabel.Text = Settings.Resources.UI_Dialog_Full_Message;
            generalTabPage.Text = Settings.Resources.UI_Dialog_Full_General_Tab;
            exceptionTabPage.Text = Settings.Resources.UI_Dialog_Full_Exception_Tab;
            reportContentsTabPage.Text = Settings.Resources.UI_Dialog_Full_Report_Contents_Tab;
            errorDescriptionLabel.Text = Settings.Resources.UI_Dialog_Full_How_to_Reproduce_the_Error_Notification;
            quitButton.Text = Settings.Resources.UI_Dialog_Full_Quit_Button;
            sendAndQuitButton.Text = Settings.Resources.UI_Dialog_Full_Send_and_Quit_Button;
            
            mainTabs.TabPages.Remove(mainTabs.TabPages["reportContentsTabPage"]);
        }

        internal UIDialogResult ShowDialog(SerializableException exception, Report report)
        {
            Text = string.Format("{0} {1}", report.GeneralInfo.HostApplication, Settings.Resources.UI_Dialog_Full_Title);

            // Fill in the 'General' tab
            warningPictureBox.Image = SystemIcons.Warning.ToBitmap();
            exceptionTextBox.Text = exception.Type;
            exceptionMessageTextBox.Text = exception.Message;
            targetSiteTextBox.Text = exception.TargetSite;
            applicationTextBox.Text = report.GeneralInfo.HostApplication + " [" +
                                      report.GeneralInfo.HostApplicationVersion + "]";
            nbugTextBox.Text = report.GeneralInfo.NBugVersion;
            dateTimeTextBox.Text = report.GeneralInfo.DateTime;
            clrTextBox.Text = report.GeneralInfo.CLRVersion;

            // Fill in the 'Exception' tab
            exceptionDetails.Initialize(exception);
            
            ShowDialog();

            // Write back the user description (as we passed 'report' as a reference since it is a refence object anyway)
            report.GeneralInfo.UserDescription = descriptionTextBox.Text;
            return uiDialogResult;
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            uiDialogResult = new UIDialogResult(ExecutionFlow.BreakExecution, SendReport.DoNotSend);
            Close();
        }

        private void ReportContentsTabPage_Enter(object sender, EventArgs e)
        {
            /*using (Storer storer = new Storer())
			using (ZipStorer zipStorer = ZipStorer.Open(storer.GetFirstReportFile(), FileAccess.Read))
			using (Stream zipItemStream = new MemoryStream())
			{
				List<ZipStorer.ZipFileEntry> zipDirectory = zipStorer.ReadCentralDir();

				foreach (ZipStorer.ZipFileEntry entry in zipDirectory)
				{
					zipItemStream.SetLength(0);
					zipStorer.ExtractFile(entry, zipItemStream);
					zipItemStream.Position = 0;
					this.reportContentsListView.Items.Add(entry.FilenameInZip);
				}
			}*/
        }

        private void SendAndQuitButton_Click(object sender, EventArgs e)
        {
            uiDialogResult = new UIDialogResult(ExecutionFlow.BreakExecution, SendReport.Send);
            Close();
        }
    }
}