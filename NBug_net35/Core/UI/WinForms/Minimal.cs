// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Minimal.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Forms;
using NBug.Core.Reporting.Info;
using NBug.Properties;

namespace NBug.Core.UI.WinForms
{
    internal class Minimal
    {
        internal UIDialogResult ShowDialog(Report report)
        {
            MessageBox.Show(
                new Form {TopMost = true},
                Settings.Resources.UI_Dialog_Minimal_Message,
                report.GeneralInfo.HostApplication + " " + Localization.UI_Dialog_Minimal_Title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return new UIDialogResult(ExecutionFlow.BreakExecution, SendReport.Send);
        }
    }
}