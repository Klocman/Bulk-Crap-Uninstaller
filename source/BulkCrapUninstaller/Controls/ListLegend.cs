/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions.ApplicationList;
using BulkCrapUninstaller.Themes;
using Klocman.Forms.Tools;

namespace BulkCrapUninstaller.Controls
{
    [WindowStyleController.ControlStyle(false)]
    public partial class ListLegend : UserControl
    {
        public ListLegend()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Properties.Settings.Default.SettingBinder.Subscribe((x, y) => UpdateColors(), settings => settings.MiscColorblind, this);
            Properties.Settings.Default.SettingBinder.Subscribe((x, y) => UpdateColors(), settings => settings.MiscTheme, this);
            UpdateColors();
        }

        public void UpdateColors()
        {
            var palette = ThemeManager.CurrentPalette;
            if (palette != null)
            {
                BackColor = palette.ControlBackground;
                ForeColor = palette.TextPrimary;
                labelLegend.BackColor = palette.ControlBackground;
                labelLegend.ForeColor = palette.TextPrimary;

                var textForeColor = palette.TextPrimary;
                labelVerified.ForeColor = textForeColor;
                labelUnverified.ForeColor = textForeColor;
                labelInvalid.ForeColor = textForeColor;
                labelOrphaned.ForeColor = textForeColor;
                labelWinFeature.ForeColor = textForeColor;
                labelStoreApp.ForeColor = textForeColor;
            }
            else
            {
                BackColor = Enabled ? SystemColors.ControlLightLight : SystemColors.Control;
                labelLegend.BackColor = SystemColors.Control;
                labelLegend.ForeColor = SystemColors.ControlText;
            }

            flowLayoutPanellabelInvalid.BackColor = ApplicationListConstants.Colors.InvalidColor;
            flowLayoutPanellabelOrphaned.BackColor = ApplicationListConstants.Colors.UnregisteredColor;
            flowLayoutPanellabelUnverified.BackColor = ApplicationListConstants.Colors.UnverifiedColor;
            flowLayoutPanellabelVerified.BackColor = ApplicationListConstants.Colors.VerifiedColor;
            flowLayoutPanellabelWinFeature.BackColor = ApplicationListConstants.Colors.WindowsFeatureColor;
            flowLayoutPanellabelStoreApp.BackColor = ApplicationListConstants.Colors.WindowsStoreAppColor;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InvalidEnabled
        {
            get { return flowLayoutPanellabelInvalid.Visible; }
            set { flowLayoutPanellabelInvalid.Visible = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool WinFeatureEnabled
        {
            get { return flowLayoutPanellabelWinFeature.Visible; }
            set { flowLayoutPanellabelWinFeature.Visible = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CertificatesEnabled
        {
            get { return flowLayoutPanellabelVerified.Visible; }
            set { flowLayoutPanellabelVerified.Visible = value; flowLayoutPanellabelUnverified.Visible = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool OrphanedEnabled
        {
            get { return flowLayoutPanellabelOrphaned.Visible; }
            set { flowLayoutPanellabelOrphaned.Visible = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool StoreAppEnabled
        {
            get { return flowLayoutPanellabelStoreApp.Visible; }
            set { flowLayoutPanellabelStoreApp.Visible = value; }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            CloseRequested?.Invoke(sender, e);
        }

        private void ThisEnabledChanged(object sender, EventArgs e)
        {
            UpdateColors();
        }

        public event EventHandler CloseRequested;
    }
}