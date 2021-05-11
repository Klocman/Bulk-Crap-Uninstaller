using System;
using System.Drawing;
using System.Windows.Forms;

namespace Klocman.Forms
{
    internal partial class OverlaySplashScreen : Form
    {
        private readonly Form _owner;
        private bool _faded;

        /// <summary>
        ///     Create a new splash screen.
        /// </summary>
        /// <param name="owner">Form over which the splas screen will be drawn</param>
        /// <param name="shownImage">Image that the splash is showing</param>
        public OverlaySplashScreen(Form owner, Image shownImage) : this()
        {
            _owner = owner;
            startupSplashPictureBox.Image = shownImage;
            Icon = _owner.Icon;

            Location = _owner.PointToScreen(Point.Empty);
            Size = _owner.ClientSize;

            _owner.Move += OnOwnerMove;
            _owner.Resize += OnOwnerResize;
            _owner.FormClosed += OnOwnerClosed;
        }

        private OverlaySplashScreen()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Fade out the splash screen
        /// </summary>
        public void FadeOut()
        {
            if (_faded || IsDisposed) return;
            _faded = true;
            timerFadeout.Start();
        }

        protected override void OnClosed(EventArgs e)
        {
            _owner.Move -= OnOwnerMove;
            _owner.Resize -= OnOwnerResize;
            _owner.FormClosed -= OnOwnerClosed;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            _owner.Focus();
        }

        private void OnOwnerClosed(object sender, FormClosedEventArgs args)
        {
            Close();
        }

        private void OnOwnerMove(object sender, EventArgs args)
        {
            Location = _owner.PointToScreen(Point.Empty);
        }

        private void OnOwnerResize(object sender, EventArgs e)
        {
            Size = _owner.ClientSize;
        }

        private void timerFadeout_Tick(object sender, EventArgs e)
        {
            var op = Math.Max(0, Opacity - 0.14);

            if (Math.Abs(op) < 0.01)
            {
                timerFadeout.Stop();
                Close();
                Dispose();
            }
            else
            {
                Opacity = op;
            }
        }
    }
}