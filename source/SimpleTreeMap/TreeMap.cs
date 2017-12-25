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

        public TreeMap()
        {
            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw, true);
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

        protected override void OnClick(EventArgs e)
        {
            // todo select stuff, send selected events, with ctrl held select multiple
            base.OnClick(e);
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

            var mousePos = MousePosition;
            var hoveredItem = _rectangles.FirstOrDefault(x => x.Contains(mousePos));

            if (hoveredItem == null)
                toolTip1.Hide(this);
            else
                toolTip1.Show(string.Join("\n", hoveredItem.Slice.Elements.Select(x => x.Text).ToArray()), this, 999999);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var mousePos = e.Location;
            var hoveredItem = _rectangles.FirstOrDefault(x => x.Contains(mousePos));

            if (hoveredItem == null)
                toolTip1.Hide(this);
            else
                toolTip1.Show(string.Join("\n", hoveredItem.Slice.Elements.Select(x => x.Text).ToArray()), this, 999999);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // todo draw whole thing based on input, input is set by populate

            //var font = new Font("Arial", 8);
            var gfx = e.Graphics;

            gfx.FillRectangle(new SolidBrush(DefaultBackColor), ClientRectangle);

            if (_rectangles == null)
            {
                if (DesignMode)
                {
                    ObjectNameGetter = o => o.ToString();
                    ObjectValueGetter = o => (int) o;
                    Populate(new[] {10, 9, 8, 7, 6, 5, 3, 3, 3, 1}.Cast<object>());
                }
                return;
            }

            foreach (var r in _rectangles)
            {
                var fillRect = new Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1);
                fillRect.Offset(ClientRectangle.Location);

                gfx.FillRectangle(new SolidBrush(r.Slice.Elements.First().Color), fillRect);

                ControlPaint.DrawBorder(gfx, fillRect, ForeColor, ButtonBorderStyle.Outset);

                //gfx.DrawString(r.Slice.Elements.First().Object.ToString(), font,
                //    new SolidBrush(Control.DefaultForeColor), r.X, r.Y);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (_currentSlice != null)
                _rectangles = SliceMaker.GetRectangles(_currentSlice, ClientSize.Width, ClientSize.Height).ToList();
        }

        public void Populate(IEnumerable<object> objects)
        {
            // todo grab stuff from delegates that are set to this, delegate for items and for selected

            if (Disposing || IsDisposed) return;

            if (ObjectValueGetter == null)
                throw new InvalidOperationException(nameof(ObjectValueGetter) + " can't be null");
            if (ObjectNameGetter == null)
                throw new InvalidOperationException(nameof(ObjectNameGetter) + " can't be null");

            var elements = objects
                .Select(x => new Element<object>
                {
                    Object = x,
                    Text = ObjectNameGetter(x),
                    Value = ObjectValueGetter(x),
                    Color = ObjectColorGetter?.Invoke(x) ?? BackColor
                })
                .OrderByDescending(x => x.Value)
                .ToList();

            _currentSlice = SliceMaker.GetSlice(elements, 1, MinSliceRatio);

            OnResize(EventArgs.Empty);
            Refresh();
        }
    }
}