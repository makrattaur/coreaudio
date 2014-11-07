using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VolumeMixer
{
    public partial class ucImageCheckbox : CheckBox
    {
        public ucImageCheckbox()
        {
            InitializeComponent();
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(32, 32);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            using (var brush = new SolidBrush(BackColor))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }

            if (mouseInside || pressing)
            {
                var state = System.Windows.Forms.VisualStyles.PushButtonState.Normal;

                if (pressing)
                {
                    state = System.Windows.Forms.VisualStyles.PushButtonState.Pressed;
                }
                else if (mouseInside)
                {
                    state = System.Windows.Forms.VisualStyles.PushButtonState.Normal;
                }

                ButtonRenderer.DrawButton(e.Graphics, this.ClientRectangle, state);
            }



            if (ImageList == null || (ImageList != null && ImageList.Images.Count < 2))
            {
                using (var brush = new SolidBrush(Checked ? Color.Lime : Color.Red))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            }
            else
            {
                int index = Checked ? 1 : 0;
                e.Graphics.DrawImageUnscaled(ImageList.Images[index], new Point(8, 8));
            }
        }

        bool mouseInside = false;
        protected override void OnMouseEnter(EventArgs eventargs)
        {
            mouseInside = true;
            base.OnMouseEnter(eventargs);
        }

        protected override void OnMouseLeave(EventArgs eventargs)
        {
            mouseInside = false;
            pressing = false;
            base.OnMouseLeave(eventargs);
        }

        bool pressing = false;
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            pressing = true;
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            pressing = false;
            base.OnMouseUp(mevent);
        }
    }
}
