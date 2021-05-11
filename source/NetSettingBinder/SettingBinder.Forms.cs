using System;
using System.Configuration;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace Klocman.Binding.Settings
{
    public partial class SettingBinder<TSettingClass> where TSettingClass : ApplicationSettingsBase
    {
        /// <summary>
        ///     Control will update any changes to the settings store and receive updates to change accordingly.
        ///     Best to tag using the parent form.
        /// </summary>
        /// <param name="sourceControl">Control to bind the setting to</param>
        /// <param name="targetSetting">Lambda of style 'x => x.Property' or 'x => class.Property'</param>
        /// <param name="tag">Tag used for grouping</param>
        /// <param name="clickingChangesChecked">Adds an event handler to ToolStripMenuItem.Click that will flip its Checked property automatically.</param>
        /// <exception cref="ArgumentException">Invalid lambda format</exception>
        public void BindControl(ToolStripMenuItem sourceControl, Expression<Func<TSettingClass, bool>> targetSetting,
            object tag, bool clickingChangesChecked = true)
        {
            Bind(x => sourceControl.Checked = x, () => sourceControl.Checked,
                eh => sourceControl.CheckedChanged += eh, eh => sourceControl.CheckedChanged -= eh,
                targetSetting, tag);

            sourceControl.Click += (x, y) => sourceControl.Checked = !sourceControl.Checked;
        }

        /// <summary>
        ///     Control will update any changes to the settings store and receive updates to change accordingly.
        ///     Best to tag using the parent form.
        /// </summary>
        /// <param name="sourceControl">Control to bind the setting to</param>
        /// <param name="targetSetting">Lambda of style 'x => x.Property' or 'x => class.Property'</param>
        /// <param name="tag">Tag used for grouping</param>
        /// <exception cref="ArgumentException">Invalid lambda format</exception>
        public void BindControl(ToolStripButton sourceControl, Expression<Func<TSettingClass, bool>> targetSetting,
            object tag)
        {
            Bind(x => sourceControl.Checked = x, () => sourceControl.Checked,
                eh => sourceControl.CheckedChanged += eh, eh => sourceControl.CheckedChanged -= eh,
                targetSetting, tag);
        }

        /// <summary>
        ///     Control will update any changes to the settings store and receive updates to change accordingly.
        ///     Best to tag using the parent form.
        /// </summary>
        /// <param name="sourceControl">Control to bind the setting to</param>
        /// <param name="targetSetting">Lambda of style 'x => x.Property' or 'x => class.Property'</param>
        /// <param name="tag">Tag used for grouping</param>
        /// <exception cref="ArgumentException">Invalid lambda format</exception>
        public void BindControl(TextBox sourceControl, Expression<Func<TSettingClass, string>> targetSetting, object tag)
        {
            Bind(x => sourceControl.Text = x, () => sourceControl.Text,
                eh => sourceControl.TextChanged += eh, eh => sourceControl.TextChanged -= eh,
                targetSetting, tag);
        }

        /// <summary>
        ///     Control will update any changes to the settings store and receive updates to change accordingly.
        ///     Best to tag using the parent form.
        /// </summary>
        /// <param name="sourceControl">Control to bind the setting to</param>
        /// <param name="targetSetting">Lambda of style 'x => x.Property' or 'x => class.Property'</param>
        /// <param name="tag">Tag used for grouping</param>
        /// <exception cref="ArgumentException">Invalid lambda format</exception>
        public void BindControl(CheckBox sourceControl, Expression<Func<TSettingClass, bool>> targetSetting, object tag)
        {
            Bind(x => sourceControl.Checked = x, () => sourceControl.Checked,
                eh => sourceControl.CheckedChanged += eh, eh => sourceControl.CheckedChanged -= eh,
                targetSetting, tag);
        }

        /// <summary>
        ///     Control will update any changes to the settings store and receive updates to change accordingly.
        ///     Best to tag using the parent form.
        ///     Clicking the ToolStripMenuItem will automatically change it's Checked property.
        /// </summary>
        /// <param name="sourceControl">Control to bind the setting to</param>
        /// <param name="targetSetting">Lambda of style 'x => x.Property' or 'x => class.Property'</param>
        /// <param name="tag">Tag used for grouping</param>
        /// <exception cref="ArgumentException">Invalid lambda format</exception>
        public void BindControl(NumericUpDown sourceControl, Expression<Func<TSettingClass, decimal>> targetSetting,
            object tag)
        {
            Bind(x => sourceControl.Value = x, () => sourceControl.Value,
                eh => sourceControl.ValueChanged += eh, eh => sourceControl.ValueChanged -= eh,
                targetSetting, tag);
        }
    }
}
