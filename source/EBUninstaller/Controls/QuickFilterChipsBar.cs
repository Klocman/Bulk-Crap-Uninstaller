/*
    EBUninstaller Pro - Modern Quick Filter Chips & Pills Bar
    Fast category filtering (All, Win32, Store, Games, Portable, Updates, System, Large Apps)
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;

namespace BulkCrapUninstaller.Controls
{
    public enum AppFilterCategory
    {
        All,
        Win32,
        StoreApps,
        Games,
        Portable,
        Updates,
        SystemComponents,
        LargeApps
    }

    public sealed class QuickFilterChipsBar : UserControl
    {
        private FlowLayoutPanel _flow;
        private Button _activeButton;

        public event EventHandler<AppFilterCategory> FilterCategoryChanged;

        public QuickFilterChipsBar()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Top;
            Height = 38;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Padding = new Padding(6, 2, 6, 2);

            _flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = true
            };

            AddChip("All Applications", AppFilterCategory.All, true);
            AddChip("Win32 Desktop", AppFilterCategory.Win32);
            AddChip("Windows Store (UWP)", AppFilterCategory.StoreApps);
            AddChip("Games & Launchers", AppFilterCategory.Games);
            AddChip("Portable Apps", AppFilterCategory.Portable);
            AddChip("Large Apps (> 1 GB)", AppFilterCategory.LargeApps);
            AddChip("System Components", AppFilterCategory.SystemComponents);
            AddChip("Windows Updates", AppFilterCategory.Updates);

            Controls.Add(_flow);
        }

        private void AddChip(string title, AppFilterCategory category, bool isDefault = false)
        {
            var btn = new Button
            {
                Text = title,
                Tag = category,
                AutoSize = true,
                Height = 28,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(3, 2, 3, 2),
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 8.5F, isDefault ? FontStyle.Bold : FontStyle.Regular)
            };

            var palette = ThemeEngine.CurrentPalette;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = isDefault ? palette.Accent : palette.Border;
            btn.BackColor = isDefault ? palette.SurfaceHighlight : palette.Surface;
            btn.ForeColor = isDefault ? palette.Accent : palette.TextPrimary;

            if (isDefault) _activeButton = btn;

            btn.Click += (s, e) =>
            {
                SelectChip(btn);
                FilterCategoryChanged?.Invoke(this, category);
            };

            _flow.Controls.Add(btn);
        }

        private void SelectChip(Button btn)
        {
            _activeButton = btn;
            var palette = ThemeEngine.CurrentPalette;

            foreach (Control c in _flow.Controls)
            {
                if (c is Button b)
                {
                    bool isSel = b == btn;
                    b.FlatAppearance.BorderColor = isSel ? palette.Accent : palette.Border;
                    b.BackColor = isSel ? palette.SurfaceHighlight : palette.Surface;
                    b.ForeColor = isSel ? palette.Accent : palette.TextPrimary;
                    b.Font = new Font("Segoe UI", 8.5F, isSel ? FontStyle.Bold : FontStyle.Regular);
                }
            }
        }

        public void RefreshColors()
        {
            var palette = ThemeEngine.CurrentPalette;
            BackColor = palette.Background;
            if (_activeButton != null)
                SelectChip(_activeButton);
        }
    }
}
