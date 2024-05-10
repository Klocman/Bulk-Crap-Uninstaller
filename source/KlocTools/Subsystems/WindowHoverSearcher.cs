/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Klocman.Subsystems
{
    /// <summary>
    /// Used to detect what window is under the cursor and return information about it. 
    /// Can detect any window of any application and return the owning process.
    /// </summary>
    public class WindowHoverSearcher
    {
        private const int WmGettext = 0xD;
        private const int WmGettextlength = 0x000E;
        private readonly Control _mouseTarget;
        private WindowInfo _curWindow;
        private WindowInfo _lastWindow;

        public WindowHoverSearcher(Control mouseTarget)
        {
            _mouseTarget = mouseTarget;
            _mouseTarget.MouseDown += OnMouseDown;
            _mouseTarget.MouseMove += OnMouseMove;
            _mouseTarget.MouseUp += OnMouseUp;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point point);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr handle, StringBuilder className, int maxCount);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr handle, int msg, int param1, int param2);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr handle, int msg, int param, StringBuilder text);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr handle, out Rect rect);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        public event EventHandler PickingStarted;
        public event EventHandler<WindowHoverEventArgs> HoveredWindowChanged;
        public event EventHandler<WindowHoverEventArgs> WindowSelected;

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            
            _mouseTarget.Cursor = Cursors.Cross;

            PickingStarted?.Invoke(this, EventArgs.Empty);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            var pt = Cursor.Position;
            _curWindow = new WindowInfo(WindowFromPoint(pt));

            if (_lastWindow == null)
            {
                ControlPaint.DrawReversibleFrame(_curWindow.WindowRect, Color.Black, FrameStyle.Thick);
                HoveredWindowChanged?.Invoke(this, new WindowHoverEventArgs(_curWindow));
            }
            else if (!_curWindow.Handle.Equals(_lastWindow.Handle))
            {
                ControlPaint.DrawReversibleFrame(_lastWindow.WindowRect, Color.Black, FrameStyle.Thick);
                ControlPaint.DrawReversibleFrame(_curWindow.WindowRect, Color.Black, FrameStyle.Thick);
                HoveredWindowChanged?.Invoke(this, new WindowHoverEventArgs(_curWindow));
            }

            _lastWindow = _curWindow;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            _mouseTarget.Cursor = Cursors.Default;

            if (_lastWindow == null) return;

            ControlPaint.DrawReversibleFrame(_lastWindow.WindowRect, Color.Black, FrameStyle.Thick);

            WindowSelected?.Invoke(this, new WindowHoverEventArgs(_lastWindow));
        }

        private static string GetWindowClassName(IntPtr handle)
        {
            var buffer = new StringBuilder(128);
            GetClassName(handle, buffer, buffer.Capacity);
            return buffer.ToString();
        }

        private static string GetWindowText(IntPtr handle)
        {
            var buffer = new StringBuilder(SendMessage(handle, WmGettextlength, 0, 0) + 1);
            SendMessage(handle, WmGettext, buffer.Capacity, buffer);
            return buffer.ToString();
        }

        private static Rectangle GetWindowRectangle(IntPtr handle)
        {
            Rect rect;
            GetWindowRect(handle, out rect);
            return new Rectangle(rect.Left, rect.Top, (rect.Right - rect.Left) + 1, (rect.Bottom - rect.Top) + 1);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public readonly int Left;
            public readonly int Top;
            public readonly int Right;
            public readonly int Bottom;
        }

        public class WindowInfo
        {
            private string _className;
            private int _processId;
            private Rectangle _windowRect;
            private string _windowText;

            public WindowInfo(IntPtr handle)
            {
                Handle = handle;
            }

            public IntPtr Handle { get; }
            public string ClassName => _className ?? (_className = GetWindowClassName(Handle));
            public string WindowText => _windowText ?? (_windowText = GetWindowText(Handle));
            public Rectangle WindowRect
                => !_windowRect.IsEmpty ? _windowRect : (_windowRect = GetWindowRectangle(Handle));

            public int ProcessId
            {
                get
                {
                    if (_processId == 0)
                    {
                        uint processId;
                        GetWindowThreadProcessId(Handle, out processId);
                        _processId = (int) processId;
                    }
                    return _processId;
                }
            }

            public Process GetRunningProcess() => ProcessId == 0 ? null : Process.GetProcessById(ProcessId);
        }
    }
}