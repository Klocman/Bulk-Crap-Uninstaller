// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternalLogViewer.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Windows.Forms;
using NBug.Enums;
using NBug.Properties;

namespace NBug.Core.UI.Developer
{
    internal partial class InternalLogViewer : Form
    {
        private static bool closed;
        private static ManualResetEvent handleCreated;
        private static bool initialized;
        private static InternalLogViewer viewer;

        internal InternalLogViewer()
        {
            InitializeComponent();
            Icon = Resources.NBug_icon_16;
            notifyIcon.Icon = Resources.NBug_icon_16;
        }

        public static void InitializeInternalLogViewer()
        {
            if (!initialized)
            {
                initialized = true;
                viewer = new InternalLogViewer();
                handleCreated = new ManualResetEvent(false);
                viewer.HandleCreated += (sender, e) => handleCreated.Set();
                //Task.Factory.StartNew(() => Application.Run(viewer));
                new Thread(() => Application.Run(viewer)).Start();
                handleCreated.WaitOne();
            }
        }

        public static void LogEntry(string message, LoggerCategory category)
        {
            InitializeInternalLogViewer();

            if (!closed)
            {
                viewer.Invoke((MethodInvoker) (() => viewer.InternalLogEntry(message, category)));
            }
        }

        internal void InternalLogEntry(string message, LoggerCategory category)
        {
            loggerListView.Items.Add(
                new ListViewItem(new[] {category.ToString().Remove(0, 4), DateTime.Now.ToString("HH:mm:ss"), message}));
        }

        private void HideButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void InternalLogViewer_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void LoggerListView_Click(object sender, EventArgs e)
        {
            detailsTextBox.Text = loggerListView.SelectedItems[0].SubItems[2].Text;
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }

            Show();
            Activate();
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            notifyIcon = null;
            closed = true;
            Close();
        }
    }
}