/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using Klocman.Forms.Tools;

namespace Klocman.Controls
{
    public sealed class TwitterButton : Button
    {
        public TwitterButton()
        {
            Text = string.Empty;
            BackgroundImage = Properties.Resources.twitterButton;
            BackgroundImageLayout = ImageLayout.Stretch;

            Size = new Size(23, 23);
            base.FlatStyle = FlatStyle.Standard;

            TabStop = false;

            Click += TwitterButton_Click;
        }

        public new FlatStyle FlatStyle => base.FlatStyle;

        /// <summary>
        ///     Overwrite default message text. If set ignores TargetSite.
        /// </summary>
        public string MessageText { get; set; }

        public string TargetSite { get; set; }

        private void TwitterButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(MessageText))
            {
                PremadeDialogs.StartProcessSafely(@"https://twitter.com/intent/tweet?text=" +
                                                MessageText.Replace(' ', '+'));
            }
            else if (!string.IsNullOrEmpty(TargetSite))
            {
                PremadeDialogs.StartProcessSafely(@"https://twitter.com/intent/tweet?text=" +
                                                "Check+out+this+cool+app+I+found!+" + TargetSite);
            }
            else
            {
                throw new InvalidOperationException("TargetSite and MessageText are both null or empty");
            }
        }
    }
}