/*
    OpenUninstall Pro - Modern Windows 11 Navigation Command Bar
    Unified 13-Section Navigation Bar
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;

namespace BulkCrapUninstaller.Controls
{
    public sealed class ModernNavCommandBar : UserControl
    {
        private FlowLayoutPanel _navFlow;

        public event EventHandler<string> SectionNavigated;

        public ModernNavCommandBar()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Top;
            Height = 46;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Padding = new Padding(4, 3, 4, 3);

            _navFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = true
            };

            // Add all 13 core navigation items
            AddNavButton("Apps", "Applications", true);
            AddNavButton("Uninstall", "Uninstall Pipeline");
            AddNavButton("Leftovers", "Leftovers Scanner");
            AddNavButton("Monitor", "Installation Monitor");
            AddNavButton("Backups", "Backup Center");
            AddNavButton("Startup", "Startup Manager");
            AddNavButton("Junk", "Junk Cleaner");
            AddNavButton("Extensions", "Browser Extensions");
            AddNavButton("Privacy", "Privacy Cleaner");
            AddNavButton("Shredder", "Secure Shredder");
            AddNavButton("WinTools", "Windows Tools");
            AddNavButton("Settings", "Settings");
            AddNavButton("History", "Audit History");

            Controls.Add(_navFlow);
        }

        private void AddNavButton(string sectionKey, string label, bool isDefault = false)
        {
            var btn = new Button
            {
                Text = label,
                Tag = sectionKey,
                AutoSize = true,
                Height = 34,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(2, 2, 2, 2),
                Cursor = Cursors.Hand
            };

            var palette = ThemeEngine.CurrentPalette;
            btn.FlatAppearance.BorderColor = isDefault ? palette.Accent : palette.Border;
            btn.FlatAppearance.BorderSize = isDefault ? 2 : 1;
            btn.BackColor = isDefault ? palette.SurfaceHighlight : palette.Surface;
            btn.ForeColor = isDefault ? palette.Accent : palette.TextPrimary;

            btn.Click += (s, e) =>
            {
                SelectButton(btn);
                SectionNavigated?.Invoke(this, sectionKey);
            };

            _navFlow.Controls.Add(btn);
        }

        private void SelectButton(Button selectedBtn)
        {
            var palette = ThemeEngine.CurrentPalette;
            foreach (Control c in _navFlow.Controls)
            {
                if (c is Button b)
                {
                    var isSelected = b == selectedBtn;
                    b.FlatAppearance.BorderColor = isSelected ? palette.Accent : palette.Border;
                    b.FlatAppearance.BorderSize = isSelected ? 2 : 1;
                    b.BackColor = isSelected ? palette.SurfaceHighlight : palette.Surface;
                    b.ForeColor = isSelected ? palette.Accent : palette.TextPrimary;
                    b.Font = new Font(b.Font, isSelected ? FontStyle.Bold : FontStyle.Regular);
                }
            }
        }
    }
}
