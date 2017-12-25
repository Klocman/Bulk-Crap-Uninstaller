using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleTreeMap
{
    public partial class TreeMap : UserControl
    {
        private const double MinSliceRatio = 0.35;
        private Slice<object> _currentSlice;
        private List<SliceRectangle<object>> _rectangles;
        private HashSet<object> _selectedObjects;

        public TreeMap()
        {
            InitializeComponent();

            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ReadOnly(true)]
        public Func<object, Color> ObjectColorGetter { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ReadOnly(true)]
        public Func<object, string> ObjectNameGetter { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ReadOnly(true)]
        public Func<object, double> ObjectValueGetter { get; set; }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            var r = GetRectangleUnderMouse();

            if (r != null)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        OnSliceClicked(new SliceClickedEventArgs(r, r.Slice.Elements.Select(x => x.Object).ToArray(), (ModifierKeys & Keys.Control) != 0));
                        break;
                    case MouseButtons.Right:
                        OnSliceRightClicked(new SliceClickedEventArgs(r, r.Slice.Elements.Select(x => x.Object).ToArray(), (ModifierKeys & Keys.Control) != 0));
                        break;
                }
            }
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            toolTip1.Hide(this);
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            toolTip1.Hide(this);
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            if (_rectangles == null) return;
            UpdateTooltip(GetRectangleUnderMouse());
        }

        private SliceRectangle<object> GetRectangleUnderMouse()
        {
            var mousePos = PointToClient(MousePosition);
            var hoveredItem = _rectangles.FirstOrDefault(x => x.Contains(mousePos));
            return hoveredItem;
        }

        private SliceRectangle<object> _currentTooltipRectangle;
        private static readonly SolidBrush SelectedRectBrush = new SolidBrush(Color.DodgerBlue);

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_rectangles == null) return;
            UpdateTooltip(GetRectangleUnderMouse());
        }

        private void UpdateTooltip(SliceRectangle<object> hoveredItem)
        {
            if (hoveredItem == null)
                toolTip1.Hide(this);
            else if (!toolTip1.Active || _currentTooltipRectangle != hoveredItem)
                toolTip1.Show(string.Join("\n", hoveredItem.Slice.Elements.Select(x => x.Text).ToArray()), this, new Point(0, Height + 2));

            _currentTooltipRectangle = hoveredItem;
        }

        readonly Dictionary<Color, Brush> _brushCache = new Dictionary<Color, Brush>();
        public event EventHandler<SliceClickedEventArgs> SliceRightClicked;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // todo draw whole thing based on input, input is set by populate

            //var font = new Font("Arial", 8);
            var gfx = e.Graphics;

            gfx.FillRectangle(new SolidBrush(Color.Black), ClientRectangle);

            if (_rectangles == null)
            {
                if (DesignMode)
                    PopulateWithDemoData();
                return;
            }

            foreach (var r in _rectangles)
            {
                if (r.PaintRect.IsEmpty)
                    continue;

                if (_selectedObjects != null && r.Slice.Elements.Any(x => _selectedObjects.Contains(x.Object)))
                    gfx.FillRectangle(SelectedRectBrush, r.PaintRect);
                else
                    gfx.FillRectangle(_brushCache[r.Slice.Elements.First().Color], r.PaintRect);

                //gfx.DrawString(r.Slice.Elements.First().Object.ToString(), font,
                //    new SolidBrush(Control.DefaultForeColor), r.X, r.Y);
            }
        }

        private void PopulateWithDemoData()
        {
            ObjectNameGetter = o => o.ToString();
            ObjectValueGetter = o => (int) o;
            Populate(new[] {10, 9, 8, 7, 6, 5, 3, 3, 3, 1}.Cast<object>());
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (_currentSlice != null)
                UpdateRectangles();
        }

        private void UpdateRectangles()
        {
            _rectangles = SliceMaker.GetRectangles(_currentSlice, ClientSize.Width, ClientSize.Height).ToList();
            foreach (var r in _rectangles)
                CalculatePaintRect(r);
        }

        /// <summary>
        /// Calculate area to be drawn for this rectangle.
        /// Make sure to leave a 1px border around the map and between other rectangles.
        /// </summary>
        private void CalculatePaintRect(SliceRectangle<object> r)
        {
            r.PaintRect = new Rectangle(r.X == 0 ? 1 : r.X, r.Y == 0 ? 1 : r.Y, r.Width - (r.X == 0 ? 2 : 1), r.Height - (r.Y == 0 ? 2 : 1));

            r.PaintRect.Offset(ClientRectangle.Location);

            // Avoid unnecessary drawing
            if (r.PaintRect.Height < 2 || r.PaintRect.Width < 2)
                r.PaintRect = Rectangle.Empty;
        }

        public bool UseLogValueScaling { get; set; }

        public void Populate(IEnumerable<object> objects)
        {
            // todo grab stuff from delegates that are set to this, delegate for items and for selected

            if (Disposing || IsDisposed) return;

            if (ObjectValueGetter == null)
                throw new InvalidOperationException(nameof(ObjectValueGetter) + " can't be null");
            if (ObjectNameGetter == null)
                throw new InvalidOperationException(nameof(ObjectNameGetter) + " can't be null");

            var elements = objects.Select(x =>
            {
                var element = new Element<object>
                {
                    Object = x, Text = ObjectNameGetter(x), Value = ObjectValueGetter(x)
                };

                if (ObjectColorGetter != null)
                {
                    element.Color = ObjectColorGetter.Invoke(x);
                    if (element.Color.IsEmpty)
                        element.Color = BackColor;
                }
                else
                {
                    element.Color = BackColor;
                }

                return element;
            }).Where(x => x.Value > 0.0001).OrderByDescending(x => x.Value).ToList();

            if (UseLogValueScaling)
                ScaleValuesLog(elements);

            _currentSlice = SliceMaker.GetSlice(elements, 1, MinSliceRatio);

            foreach (var c in elements.Select(x => x.Color).Distinct())
                _brushCache[c] = new SolidBrush(c);

            UpdateRectangles();

            Refresh();
        }

        public event EventHandler<SliceClickedEventArgs> SliceClicked;

        public class SliceClickedEventArgs : EventArgs
        {
            public SliceClickedEventArgs(SliceRectangle<object> rectangle, ICollection<object> o, bool addToSelection)
            {
                Rectangle = rectangle;
                Objects = o;
                AddToSelection = addToSelection;
            }

            public SliceRectangle<object> Rectangle { get; }
            public ICollection<object> Objects { get; }
            public bool AddToSelection { get; }
        }

        private void ScaleValuesLog(ICollection<Element<object>> elements)
        {
            var max = elements.Max(x => x.Value);

            foreach (var element in elements)
            {
                element.Value = (Math.Log10(element.Value/max + 0.3174) + 0.5)*1.6;
            }
        }

        protected virtual void OnSliceClicked(SliceClickedEventArgs e)
        {
            SliceClicked?.Invoke(this, e);
        }

        private void OnSliceRightClicked(SliceClickedEventArgs e)
        {
            SliceRightClicked?.Invoke(this, e);
        }

        public void SetSelectedObjects(IEnumerable<object> objects)
        {
            _selectedObjects = new HashSet<object>(objects);
            Refresh();
        }
    }
}