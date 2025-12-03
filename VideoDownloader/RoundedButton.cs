using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VideoDownloader
{
    public class RoundedButton : Button
    {
        private int borderRadius = 8;
        private Color hoverColor;
        private Color pressedColor;
        private bool isHovering = false;
        private bool isPressed = false;

        public int BorderRadius
        {
            get => borderRadius;
            set { borderRadius = value; Invalidate(); }
        }

        public RoundedButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.FromArgb(99, 102, 241);
            ForeColor = Color.White;
            Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            Cursor = Cursors.Hand;
            
            // Double buffering for smooth rendering
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Determine background color based on state
            Color bgColor = BackColor;
            if (isPressed)
            {
                bgColor = DarkenColor(BackColor, 0.15f);
            }
            else if (isHovering)
            {
                bgColor = LightenColor(BackColor, 0.1f);
            }

            // Clear with parent background first to prevent artifacts
            if (Parent != null)
            {
                e.Graphics.Clear(Parent.BackColor);
            }

            // Create rounded rectangle path - use full bounds
            using (GraphicsPath path = CreateRoundedRectangle(new Rectangle(0, 0, Width, Height), borderRadius))
            {
                // Fill background
                using (SolidBrush brush = new SolidBrush(bgColor))
                {
                    e.Graphics.FillPath(brush, path);
                }

                // Set clip for text
                e.Graphics.SetClip(path);
            }

            // Draw text with better quality
            TextFormatFlags flags = TextFormatFlags.HorizontalCenter | 
                                   TextFormatFlags.VerticalCenter | 
                                   TextFormatFlags.TextBoxControl;
            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor, flags);
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            // Top left arc
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            // Top right arc
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            // Bottom right arc
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            // Bottom left arc
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);

            path.CloseFigure();
            return path;
        }

        private Color LightenColor(Color color, float amount)
        {
            int r = Math.Min(255, (int)(color.R + (255 - color.R) * amount));
            int g = Math.Min(255, (int)(color.G + (255 - color.G) * amount));
            int b = Math.Min(255, (int)(color.B + (255 - color.B) * amount));
            return Color.FromArgb(color.A, r, g, b);
        }

        private Color DarkenColor(Color color, float amount)
        {
            int r = Math.Max(0, (int)(color.R * (1 - amount)));
            int g = Math.Max(0, (int)(color.G * (1 - amount)));
            int b = Math.Max(0, (int)(color.B * (1 - amount)));
            return Color.FromArgb(color.A, r, g, b);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            isHovering = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isHovering = false;
            isPressed = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            isPressed = true;
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isPressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            // Make the control transparent outside the rounded area - use full bounds
            Region = new Region(CreateRoundedRectangle(new Rectangle(0, 0, Width, Height), borderRadius));
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Region = new Region(CreateRoundedRectangle(new Rectangle(0, 0, Width, Height), borderRadius));
            Invalidate();
        }
    }
}
