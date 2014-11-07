using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing.Drawing2D;

namespace VolumeMixer
{
    [DefaultBindingProperty("IntValue")]
    public partial class ucGauge : UserControl
    {
        public ucGauge()
        {
            InitializeComponent();

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }

        private int minimum = 0;

        [DefaultValue(0), RefreshProperties(RefreshProperties.Repaint)]
        public int Minimum
        {
            get { return minimum; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Minimum");
                }

                minimum = value;
                RecalculateSpan();
            }
        }

        private int maximum = 100;

        [DefaultValue(100), RefreshProperties(RefreshProperties.Repaint)]
        public int Maximum
        {
            get { return maximum; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Maximum");
                }

                maximum = value;
                RecalculateSpan();
            }
        }

        private int span = 100;

        private float normValue = 0.0f;

        [Bindable(true), DefaultValue(0)]
        public int IntValue
        {
            get
            {
                return minimum + (int)Math.Round(normValue * span);
            }
            set
            {
                if (value < minimum || value > maximum)
                {
                    throw new ArgumentOutOfRangeException("IntValue");
                }

                this.normValue = (value - minimum) / (float)(span);
                FireValueChanged();
            }
        }

        [Bindable(true), DefaultValue(0.0f)]
        public float FloatValue
        {
            get
            {
                return minimum + normValue * span;
            }
            set
            {
                if (value < minimum || value > maximum)
                {
                    throw new ArgumentOutOfRangeException("FloatValue");
                }

                this.normValue = (value - minimum) / (float)(span);
                FireValueChanged();
            }
        }

        [Bindable(true), DefaultValue(0.0f)]
        public float NormalizedFloatValue
        {
            get
            {
                return normValue;
            }
            set
            {
                if (value < 0.0f || value > 1.0f)
                {
                    throw new ArgumentOutOfRangeException("FloatValue");
                }

                this.normValue = value;
                FireValueChanged();
            }
        }

        private Orientation orientation = Orientation.Horizontal;

        [DefaultValue(Orientation.Horizontal)]
        public Orientation Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }
        

        protected override Size DefaultSize
        {
            get
            {
                if (orientation == Orientation.Vertical)
                {
                    return new Size(10, 100);
                }
                else if (orientation == Orientation.Horizontal)
                {
                    return new Size(100, 10);
                }
                else
                {
                    return new Size(100, 10);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, Border3DStyle.SunkenOuter);

            Rectangle gaugeValueRectangle = this.ClientRectangle;
            gaugeValueRectangle.Inflate(-1, -1);

            int fill = (int)Math.Round((orientation == Orientation.Vertical ? gaugeValueRectangle.Height : gaugeValueRectangle.Width) * normValue);
            if (fill == 0)
            {
                return;
            }

            if (orientation == System.Windows.Forms.Orientation.Vertical)
            {
                gaugeValueRectangle.Y = gaugeValueRectangle.Height - fill + 1;
                gaugeValueRectangle.Height = fill;
            }
            else
            {
                gaugeValueRectangle.Width = fill;
            }

            Color startColor = Color.FromArgb(51, 154, 51);
            Color endColor = Color.FromArgb(51, 204, 51);

            //using (var gradBrush = new LinearGradientBrush(startPoint, endPoint, startColor, endColor))
            //using (var gradBrush = new SolidBrush(endColor))
            using (var gradBrush = new LinearGradientBrush(gaugeValueRectangle, startColor, endColor, orientation == Orientation.Vertical ? -90.0f : 0.0f))
            {
                gradBrush.WrapMode = WrapMode.TileFlipXY;
                e.Graphics.FillRectangle(gradBrush, gaugeValueRectangle);
            }
        }

        private void FireValueChanged()
        {
            Refresh();
        }

        private void RecalculateSpan()
        {
            span = maximum - minimum;
        }
    }
}
