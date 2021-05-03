/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;
using System.Windows.Forms;

namespace Klocman.Extensions
{
    public static class RichTextBoxExtensions
    {
        /// <summary>
        ///     Append text in specified color to the end of the document without interrupting the user.
        /// </summary>
        /// <param name="box">Box to append text to</param>
        /// <param name="text">Text to append.</param>
        /// <param name="color">Color of the appended text.</param>
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SuspendLayout();

            // Backup current state
            var start = box.SelectionStart;
            var length = box.SelectionLength;
            var prevColor = box.SelectionColor;

            // Append the text
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionColor = color;
            box.AppendText(text);

            // Restore state
            box.SelectionStart = start;
            box.SelectionLength = length;
            box.SelectionColor = prevColor;

            box.ResumeLayout();
        }
    }
}