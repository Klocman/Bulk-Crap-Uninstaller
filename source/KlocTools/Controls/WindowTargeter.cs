/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Windows.Forms;
using Klocman.Subsystems;

namespace Klocman.Controls
{
    public partial class WindowTargeter : UserControl
    {
        private readonly string _helpText;
        private readonly WindowHoverSearcher _searcher;

        public WindowTargeter()
        {
            InitializeComponent();

            _helpText = label2.Text;

            _searcher = new WindowHoverSearcher(pictureBox1);
            _searcher.HoveredWindowChanged += SearcherOnHoveredWindowChanged;
            _searcher.WindowSelected += SearcherOnWindowSelected;
        }

        public event EventHandler<WindowHoverEventArgs> HoveredWindowChanged
        {
            add { _searcher.HoveredWindowChanged += value; }
            remove { _searcher.HoveredWindowChanged -= value; }
        }

        public event EventHandler<WindowHoverEventArgs> WindowSelected
        {
            add { _searcher.WindowSelected += value; }
            remove { _searcher.WindowSelected -= value; }
        }

        private void SearcherOnWindowSelected(object sender, WindowHoverEventArgs windowHoverEventArgs)
        {
            label1.Text = string.Empty;
            label2.Text = _helpText;
            label3.Text = string.Empty;
        }

        private void SearcherOnHoveredWindowChanged(object sender, WindowHoverEventArgs windowHoverEventArgs)
        {
            label1.Text = windowHoverEventArgs.TargetWindow.WindowText;
            label2.Text = windowHoverEventArgs.TargetWindow.WindowRect.ToString();
            label3.Text = windowHoverEventArgs.TargetWindow.GetRunningProcess().MainModule.FileName;
        }
    }
}