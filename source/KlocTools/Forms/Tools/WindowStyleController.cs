/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Klocman.Extensions;

namespace Klocman.Forms.Tools
{
    /// <summary>
    ///     Allows easy acces to FlatStyle and ToolStripRenderMode properties of all child controls.
    ///     It is possible to switch between System and Standard/ManagerRenderMode with one method call.
    /// </summary>
    public class WindowStyleController
    {
        private readonly Form _reference;
        private readonly List<Action<bool>> _targets = new();

        public WindowStyleController(Form parentForm)
        {
            _reference = parentForm;

            var children = parentForm.GetAllChildren(CanBeChanged).Concat(parentForm.GetComponents().Where(CanBeChanged));

            foreach (var item in children)
            {
                var type = item.GetType();
                var property = type.GetProperty("FlatStyle");
                if (property != null)
                {
                    _targets.Add(x => property.SetValue(item, x ? FlatStyle.System : FlatStyle.Standard, null));
                }
                else
                {
                    property = type.GetProperty("RenderMode");
                    if (property != null)
                    {
                        _targets.Add(
                            x =>
                                property.SetValue(item,
                                    x ? ToolStripRenderMode.System : ToolStripRenderMode.ManagerRenderMode, null));
                    }
                }
            }
        }

        /// <summary>
        ///     Switch between System (if true) and Standard/ManagerRenderMode (if false);
        /// </summary>
        /// <param name="useSystemStyle">Use system style for all child controls.</param>
        public void SetStyles(bool useSystemStyle)
        {
            _reference.SuspendLayout();
            foreach (var child in _targets)
            {
                child(useSystemStyle);
            }
            _reference.ResumeLayout();
        }

        private static bool CanBeChanged(Component x)
        {
            var attrib =
                x.GetType()
                    .GetCustomAttributes(typeof (ControlStyleAttribute), true)
                    .Cast<ControlStyleAttribute>()
                    .FirstOrDefault();
            return attrib == null || attrib.AllowStyleChange;
        }

        public class ControlStyleAttribute : Attribute
        {
            public ControlStyleAttribute(bool allowStyleChange)
            {
                AllowStyleChange = allowStyleChange;
            }

            public bool AllowStyleChange { get; }
        }
    }
}