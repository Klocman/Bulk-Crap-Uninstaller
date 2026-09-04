using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BrightIdeasSoftware
{
    public partial class ObjectListView
    {
        private int columnDpi = 96;
        private bool columnDpiInitialized;
        private bool initializingColumnDpi;
        private bool scalingColumnWidths;
        private readonly Dictionary<OLVColumn, ColumnDimensions> columnDimensions = new();

        private void EnsureColumnDpi()
        {
            if (!columnDpiInitialized) ScaleColumnWidthsForDpi(DeviceDpi);
        }

        private OLVColumn[] ColumnsIncludingHidden() =>
            AllColumns.Concat(Columns.Cast<OLVColumn>()).Distinct().ToArray();

        private void ScaleColumnWidthsForDpi(int newDpi)
        {
            if (DesignMode || initializingColumnDpi || scalingColumnWidths || newDpi <= 0) return;
            var columns = ColumnsIncludingHidden();
            // BeginInit/EndInit defer designer columns; runtime columns added to an
            // already initialized control use its current device-pixel units.

            // Capture every column before applying constraints or resizing fill columns.
            // Retain unrounded logical dimensions to prevent drift on repeated changes.
            foreach (var column in columns)
            {
                if (!columnDimensions.TryGetValue(column, out var dimensions))
                    columnDimensions.Add(column, dimensions = new ColumnDimensions());
                dimensions.Width.Observe(column.Width, columnDpi);
                dimensions.Minimum.Observe(column.MinimumWidth, columnDpi);
                dimensions.Maximum.Observe(column.MaximumWidth, columnDpi);
            }
            foreach (var removed in columnDimensions.Keys.Except(columns).ToArray())
                columnDimensions.Remove(removed);

            scalingColumnWidths = true;
            try
            {
                foreach (var column in columns)
                {
                    var dimensions = columnDimensions[column];
                    // Remove old limits before assigning the new ones, otherwise
                    // scaling down/up can clamp a user width against the old DPI.
                    column.MinimumWidth = -1;
                    column.MaximumWidth = -1;
                    column.MinimumWidth = dimensions.Minimum.AtDpi(newDpi);
                    column.MaximumWidth = dimensions.Maximum.AtDpi(newDpi);
                    if (!column.FillsFreeSpace)
                        column.Width = dimensions.Width.AtDpi(newDpi);
                    dimensions.Width.Applied(column.Width);
                    dimensions.Minimum.Applied(column.MinimumWidth);
                    dimensions.Maximum.Applied(column.MaximumWidth);
                }
                columnDpi = newDpi;
                columnDpiInitialized = true;
            }
            finally { scalingColumnWidths = false; }

            // Fill columns follow the current client width, not a scaled old width.
            ResizeFreeSpaceFillingColumns();
        }

        private void RememberRestoredColumnWidths()
        {
            foreach (var column in ColumnsIncludingHidden())
                if (columnDimensions.TryGetValue(column, out var dimensions))
                    dimensions.Width.Reset(column.Width, columnDpi);
        }

        private sealed class ColumnDimensions
        {
            internal readonly DpiDimension Width = new();
            internal readonly DpiDimension Minimum = new();
            internal readonly DpiDimension Maximum = new();
        }

        private sealed class DpiDimension
        {
            private int? lastPixels;
            private double logical;

            internal void Observe(int pixels, int dpi)
            {
                // A changed width is a user/programmatic resize at the current DPI.
                if (lastPixels != pixels) Reset(pixels, dpi);
            }

            internal void Reset(int pixels, int dpi)
            {
                lastPixels = pixels;
                logical = pixels < 0 ? pixels : pixels * 96d / dpi;
            }

            internal int AtDpi(int dpi) => logical < 0 ? (int)logical : (int)Math.Round(logical * dpi / 96d);
            internal void Applied(int pixels) => lastPixels = pixels;
        }
    }
}
