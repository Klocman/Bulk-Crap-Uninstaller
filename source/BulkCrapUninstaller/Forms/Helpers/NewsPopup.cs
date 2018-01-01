/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BulkCrapUninstaller.Forms
{
    public partial class NewsPopup : Form
    {
        private NewsPopup()
        {
            InitializeComponent();
        }

        public static void ShowPopup(Form owner)
        {
            // TODO change when adding a new message
            if (Program.PreviousVersion != null && Program.PreviousVersion.Major >= 4)
                return;

            using (var news = new NewsPopup())
            {
                news.StartPosition = FormStartPosition.CenterParent;
                news.ShowDialog(owner);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnClick(EventArgs e)
        {
            Close();
        }
    }
}
