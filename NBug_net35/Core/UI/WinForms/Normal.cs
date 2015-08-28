// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Normal.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Windows.Forms;
using NBug.Core.Reporting.Info;
using NBug.Properties;

namespace NBug.Core.UI.WinForms
{
    internal partial class Normal : Form
    {
        private UIDialogResult uiDialogResult;

        internal Normal()
        {
            InitializeComponent();
            Icon = Resources.NBug_icon_16;
            warningPictureBox.Image = SystemIcons.Warning.ToBitmap();
            warningLabel.Text = Settings.Resources.UI_Dialog_Normal_Message;
            continueButton.Text = Settings.Resources.UI_Dialog_Normal_Continue_Button;
            quitButton.Text = Settings.Resources.UI_Dialog_Normal_Quit_Button;
        }

        internal UIDialogResult ShowDialog(Report report)
        {
            Text = string.Format("{0} {1}", report.GeneralInfo.HostApplication,
                Settings.Resources.UI_Dialog_Normal_Title);
            exceptionMessageLabel.Text = report.GeneralInfo.ExceptionMessage;

            ShowDialog();

            return uiDialogResult;
        }

        private void ContinueButton_Click(object sender, EventArgs e)
        {
            uiDialogResult = new UIDialogResult(ExecutionFlow.ContinueExecution, SendReport.Send);
            Close();
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            uiDialogResult = new UIDialogResult(ExecutionFlow.BreakExecution, SendReport.Send);
            Close();
        }
    }
}