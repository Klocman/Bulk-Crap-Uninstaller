/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Klocman.Extensions
{
    public static class FormsExtensions
    {
        /// <summary>
        ///     An application sends the WM_SETREDRAW message to a window to allow changes in that
        ///     window to be redrawn or to prevent changes in that window from being redrawn.
        /// </summary>
        private const int WM_SETREDRAW = 11;

        /// <summary>
        ///     Suspends painting for the target control. Do NOT forget to call EndControlUpdate!!!
        /// </summary>
        /// <param name="control">visual control</param>
        public static void BeginControlUpdate(this Control control)
        {
            var msgSuspendUpdate = Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero,
                IntPtr.Zero);

            var window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgSuspendUpdate);
        }

        /// <summary>
        /// Show the form and make sure that it is at the top of the screen
        /// </summary>
        public static void ShowAndMoveToTop(this Form targetForm)
        {
            targetForm.WindowState = FormWindowState.Minimized;
            targetForm.Show();
            targetForm.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        ///     Move this form to be centered to the other. Same effect as setting FormStartPosition.CenterParent and using
        ///     ShowDialog.
        /// </summary>
        /// <param name="thisForm">Form that will be moved</param>
        /// <param name="targetForm">Form to be centered to.</param>
        public static void CenterToForm(this Form thisForm, Form targetForm)
        {
            thisForm.StartPosition = FormStartPosition.Manual;
            thisForm.Location = new Point(targetForm.Location.X + (targetForm.Width - thisForm.Width) / 2,
                targetForm.Location.Y + (targetForm.Height - thisForm.Height) / 2);
        }

        /// <summary>
        ///     Resumes painting for the target control. Intended to be called following a call to BeginControlUpdate()
        /// </summary>
        /// <param name="control">visual control</param>
        public static void EndControlUpdate(this Control control)
        {
            // Create a C "true" boolean as an IntPtr
            var wparam = new IntPtr(1);
            var msgResumeUpdate = Message.Create(control.Handle, WM_SETREDRAW, wparam,
                IntPtr.Zero);

            var window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgResumeUpdate);
            control.Invalidate();
            control.Refresh();
        }

        /// <summary>
        ///     Get an enumerator that will get all child controls recursively. It will return contents of custom controls as well.
        /// </summary>
        /// <param name="control">Control to get children of</param>
        public static IEnumerable<Control> GetAllChildren(this Control control)
        {
            var controls = control.Controls.Cast<Control>().ToList();
            return controls.SelectMany(GetAllChildren).Concat(controls);
        }

        /// <summary>
        ///     Get an enumerator that will get all child controls recursively. It will return contents of custom controls as well.
        /// </summary>
        /// <param name="control">Control to get children of</param>
        /// <param name="predicate">
        ///     Predicate to filter the controls by. If a control is filtered its children will be filtered as
        ///     well.
        /// </param>
        public static IEnumerable<Control> GetAllChildren(this Control control, Func<Control, bool> predicate)
        {
            var controls = control.Controls.Cast<Control>().Where(predicate).ToList();
            return controls.SelectMany(ctrl => GetAllChildren(ctrl, predicate)).Concat(controls);
        }

        public static IEnumerable<Component> GetComponents(this Form form)
        {
            return
                from field in
                    form.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                where typeof(Component).IsAssignableFrom(field.FieldType)
                let component = (Component)field.GetValue(form)
                where component != null
                select component;
        }

        /// <summary>
        ///     Recursively check if this form is a child of specified control.
        ///     Will deadlock if you have a ring of parented controls (don't do that).
        /// </summary>
        /// <param name="c">Child control</param>
        /// <param name="parent">Control to check against</param>
        public static bool IsChildOf(this Control c, Control parent)
        {
            if (c == null) throw new NullReferenceException();
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            while (true)
            {
                if (c.Parent == null) return false;
                if (c.Parent == parent) return true;
                c = c.Parent;
            }
        }

        /// <summary>
        ///     Safely invoke supplied action on this object. If this object is disposed, do nothing.
        ///     If invoke is not required, supplied action is launched directly.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="action">Action to invoke</param>
        /// <exception cref="ArgumentNullException">The value of 'action' and 'obj' cannot be null. </exception>
        /// <exception cref="Exception">A delegate callback throws an exception. </exception>
        public static void SafeInvoke(this Control obj, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (!obj.IsDisposed && !obj.Disposing)
            {
                if (obj.InvokeRequired)
                {
                    try
                    {
                        obj.Invoke(action);
                    }
                    catch (ObjectDisposedException) { }
                    catch (InvalidOperationException) { }
                    catch (InvalidAsynchronousStateException) { }
                }
                else
                {
                    action();
                }
            }
        }

        /// <summary>
        ///     Extend aero frame into the client area of the form by specified amounts.
        /// </summary>
        /// <param name="f">Form to extend into</param>
        /// <param name="insetMargins">Amount of pixels to extend inwards</param>
        /// <returns>True if operation succeeded, otherwise false.</returns>
        public static bool ExtendAeroFrameInwards(this Form f, Padding insetMargins)
        {
            var margins = new NativeMethods.MARGINS
            {
                cxLeftWidth = insetMargins.Left,
                cxRightWidth = insetMargins.Right,
                cyTopHeight = insetMargins.Top,
                cyBottomHeight = insetMargins.Bottom
            };

            var result = NativeMethods.DwmExtendFrameIntoClientArea(f.Handle, ref margins);
            return result == 0;
        }

        /// <summary>
        ///     Enable/disable hidden double buffered attribute using reflection.
        ///     http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
        /// </summary>
        public static void SetDoubleBuffered(this Control c, bool enabled)
        {
            if (SystemInformation.TerminalServerSession)
                return;

            var aProp = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);

            if (aProp != null)
                aProp.SetValue(c, enabled, null);
        }

        /// <summary>
        ///     Convert keycode to a string with a single letter/digit. Returns null if key is not between A and Z or is not a
        ///     number.
        /// </summary>
        /// <param name="keyVal"></param>
        /// <returns></returns>
        public static string ToLetterOrNumberString(this Keys keyVal)
        {
            string keyName = null;

            if (keyVal >= Keys.A && keyVal <= Keys.Z)
            {
                keyName = keyVal.ToString();
            }
            else
            {
                var temp = keyVal.ToNumber();
                if (temp >= 0)
                    keyName = temp.ToString();
            }
            return keyName;
        }

        /// <summary>
        ///     Convert keycode of a pressed number key to the actual number. Returns -1 if key is not a number.
        /// </summary>
        public static int ToNumber(this Keys keyVal)
        {
            var value = -1;
            if (keyVal >= Keys.D0 && keyVal <= Keys.D9)
            {
                value = keyVal - Keys.D0;
            }
            else if (keyVal >= Keys.NumPad0 && keyVal <= Keys.NumPad9)
            {
                value = keyVal - Keys.NumPad0;
            }
            return value;
        }

        /// <summary>
        ///     Get height of the form's titlebar.
        /// </summary>
        public static int GetBorderHeight(this Form f)
        {
            var screenRectangle = f.RectangleToScreen(f.ClientRectangle);
            return screenRectangle.Top - f.Top;
        }

        /// <summary>
        ///     Get width of the form's left border.
        /// </summary>
        public static int GetBorderWidth(this Form f)
        {
            var screenRectangle = f.RectangleToScreen(f.ClientRectangle);
            return screenRectangle.Left - f.Left;
        }

        /// <summary>
        ///     Move the form as close to the cursor as possible without going past the edges of current screen's working area.
        /// </summary>
        public static void MoveCloseToCursor(this Form form)
        {
            var screen = Screen.FromPoint(Control.MousePosition).WorkingArea;
            var x = Math.Max(Math.Min(Control.MousePosition.X - form.Width / 2, screen.X + screen.Width - form.Width),
                screen.X);
            var y = Math.Max(Math.Min(Control.MousePosition.Y - form.Height / 2, screen.Y + screen.Height - form.Height),
                screen.Y);
            form.Location = new Point(x, y);
        }

        private static class NativeMethods
        {
            [DllImport("dwmapi.dll")]
            public static extern int DwmExtendFrameIntoClientArea(
                IntPtr hWnd,
                ref MARGINS pMarInset
                );

            [StructLayout(LayoutKind.Sequential)]
            public struct MARGINS
            {
                public int cxLeftWidth;
                public int cxRightWidth;
                public int cyTopHeight;
                public int cyBottomHeight;
            }
        }
    }
}