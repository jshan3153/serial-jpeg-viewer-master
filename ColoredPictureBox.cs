using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleSerial
{
    class ColoredPictureBox : PictureBox
    {
        // Outline color property
        public Color OutlineColor { get; set; } = Color.Green;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw the outline
            using (var pen = new Pen(OutlineColor, 5)) // Set the outline thickness
            {
                e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            }
        }
    }
}
