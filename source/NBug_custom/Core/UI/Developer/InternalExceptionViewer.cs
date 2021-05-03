// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternalExceptionViewer.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using NBug.Core.Util.Exceptions;
using NBug.Core.Util.Serialization;
using NBug.Properties;

namespace NBug.Core.UI.Developer
{
    internal partial class InternalExceptionViewer : Form
    {
        internal InternalExceptionViewer()
        {
            InitializeComponent();
            Icon = Resources.NBug_icon_16;
            warningPictureBox.Image = SystemIcons.Warning.ToBitmap();
        }

        internal void ShowDialog(Exception exception)
        {
            if (exception is NBugConfigurationException)
            {
                ShowDialog(exception as NBugConfigurationException);
            }
            else if (exception is NBugRuntimeException)
            {
                ShowDialog(exception as NBugRuntimeException);
            }
            else
            {
                messageLabel.Text =
                    "An internal runtime exception has occurred. This maybe due to a configuration failure or an internal bug. You may choose to debug the exception or send a bug report to NBug developers. You may also use discussion forum to get help.";
                bugReportButton.Enabled = true;
                DisplayExceptionDetails(exception);
            }
        }

        internal void ShowDialog(NBugConfigurationException configurationException)
        {
            messageLabel.Text =
                "An internal configuration exception has occurred. Please correct the invalid configuration regarding the information below. You may also use discussion forum to get help or read the online documentation's configuration section.";
            invalidSettingLabel.Enabled = true;
            invalidSettingTextBox.Enabled = true;
            invalidSettingTextBox.Text = configurationException.MisconfiguredProperty;
            DisplayExceptionDetails(configurationException);
        }

        internal void ShowDialog(NBugRuntimeException runtimeException)
        {
            messageLabel.Text =
                "An internal runtime exception has occurred. This maybe due to a configuration failure or an internal bug. You may choose to debug the exception or send a bug report to NBug developers. You may also use discussion forum to get help.";
            bugReportButton.Enabled = true;
            DisplayExceptionDetails(runtimeException);
        }

        private void BugReportButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Internal bug reporting feature is not implemented yet but you can still manually submit a bug report using the bug tracker.",
                "Information",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            bugReportButton.Enabled = false;
        }

        private void DebugButton_Click(object sender, EventArgs e)
        {
            // Let the exception propagate down to SEH
            Close();
        }

        private void DisplayExceptionDetails(Exception exception)
        {
            exceptionTextBox.Text = exception.GetType().ToString();
            exceptionMessageTextBox.Text = exception.Message;

            if (exception.TargetSite != null)
            {
                targetSiteTextBox.Text = exception.TargetSite.ToString();
            }
            else if (exception.InnerException != null && exception.InnerException.TargetSite != null)
            {
                targetSiteTextBox.Text = exception.InnerException.TargetSite.ToString();
            }

            exceptionDetails.Initialize(new SerializableException(exception));
            ShowDialog();
        }

        private void DocumentationToolStripButton_Click(object sender, EventArgs e)
        {
            Process.Start(documentationToolStripButton.Tag.ToString());
        }

        private void ForumToolStripLabel_Click(object sender, EventArgs e)
        {
            Process.Start(forumToolStripLabel.Tag.ToString());
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void TrackerToolStripLabel_Click(object sender, EventArgs e)
        {
            Process.Start(trackerToolStripLabel.Tag.ToString());
        }
    }
}