
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Lab3
{
    public partial class BezierForm : Form
    {
        private const int GraphSize = 400;
        private const int Margin = 65;

        private PointF p0 = new PointF(0, 0);
        private PointF p1 = new PointF(255f / 3f, 255f / 3f);
        private PointF p2 = new PointF(255f * 2f / 3f, 255f * 2f / 3f);
        private PointF p3 = new PointF(255, 255);

        private int selectedPoint = -1;

        private readonly Panel graphPanel;
        private readonly Button btnApply;
        private readonly Button btnReset;

        public BezierForm()
        {
            InitializeComponent();

            Text = "Корекція яскравості — крива Безьє";
            ClientSize = new Size(570, 565);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            graphPanel = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(550, 475),
                BackColor = Color.White
            };

            graphPanel.Paint += GraphPanel_Paint;
            graphPanel.MouseDown += GraphPanel_MouseDown;
            graphPanel.MouseMove += GraphPanel_MouseMove;
            graphPanel.MouseUp += GraphPanel_MouseUp;

            Controls.Add(graphPanel);

            btnApply = new Button
            {
                Text = "Застосувати",
                Location = new Point(310, 495),
                Size = new Size(125, 38),
                DialogResult = DialogResult.None
            };

            btnApply.Click += BtnApply_Click;
            Controls.Add(btnApply);

            btnReset = new Button
            {
                Text = "Скинути",
                Location = new Point(175, 495),
                Size = new Size(120, 38)
            };

            btnReset.Click += BtnReset_Click;
            Controls.Add(btnReset);

            AcceptButton = btnApply;
        }

        private void BtnApply_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            p0 = new PointF(0, 0);
            p1 = new PointF(255f / 3f, 255f / 3f);
            p2 = new PointF(255f * 2f / 3f, 255f * 2f / 3f);
            p3 = new PointF(255, 255);

            graphPanel.Invalidate();
        }

        private PointF ToScreen(PointF p)
        {
            float scale = GraphSize / 255f;

            return new PointF(
                Margin + p.X * scale,
                Margin + GraphSize - p.Y * scale
            );
        }

        private PointF ToGraph(PointF p)
        {
            float scale = 255f / GraphSize;

            return new PointF(
                (p.X - Margin) * scale,
                255f - (p.Y - Margin) * scale
            );
        }

        private PointF Bezier(float t)
        {
            float u = 1f - t;

            float x =
                u * u * u * p0.X +
                3f * u * u * t * p1.X +
                3f * u * t * t * p2.X +
                t * t * t * p3.X;

            float y =
                u * u * u * p0.Y +
                3f * u * u * t * p1.Y +
                3f * u * t * t * p2.Y +
                t * t * t * p3.Y;

            return new PointF(x, y);
        }

        private void GraphPanel_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float left = Margin;
            float top = Margin;
            float right = Margin + GraphSize;
            float bottom = Margin + GraphSize;

            using Pen gridPen = new Pen(Color.LightGray, 1);
            using Pen axisPen = new Pen(Color.Black, 2);
            using Font labelFont = new Font("Arial", 8);
            using Brush textBrush = new SolidBrush(Color.Black);

            for (int value = 0; value <= 255; value += 51)
            {
                float x = left + value / 255f * GraphSize;
                float y = bottom - value / 255f * GraphSize;

                g.DrawLine(gridPen, x, top, x, bottom);
                g.DrawLine(gridPen, left, y, right, y);

                g.DrawString(value.ToString(),
                    labelFont, textBrush, x - 10, bottom + 5);

                g.DrawString(value.ToString(),
                    labelFont, textBrush, left - 30, y - 7);
            }

            g.DrawLine(axisPen, left, top, left, bottom);
            g.DrawLine(axisPen, left, bottom, right, bottom);

            g.DrawString("Вхідна яскравість",
                new Font("Arial", 9, FontStyle.Bold),
                textBrush, left + 115, bottom + 27);

            g.DrawString("Вихідна яскравість",
                new Font("Arial", 9, FontStyle.Bold),
                textBrush, 5, 5);

            using Pen neutralPen = new Pen(Color.LightGray, 2)
            {
                DashStyle = DashStyle.Dash
            };

            g.DrawLine(neutralPen,
                ToScreen(new PointF(0, 0)),
                ToScreen(new PointF(255, 255)));

            using Pen controlPen = new Pen(Color.Silver, 1.5f);

            g.DrawLine(controlPen, ToScreen(p0), ToScreen(p1));
            g.DrawLine(controlPen, ToScreen(p1), ToScreen(p2));
            g.DrawLine(controlPen, ToScreen(p2), ToScreen(p3));

            PointF[] curvePoints = new PointF[1001];

            for (int i = 0; i <= 1000; i++)
            {
                float t = i / 1000f;
                curvePoints[i] = ToScreen(Bezier(t));
            }

            using Pen curvePen = new Pen(Color.Blue, 3);
            g.DrawLines(curvePen, curvePoints);

            DrawPoint(g, p0, "P0", Color.DarkGreen);
            DrawPoint(g, p1, "P1", Color.Red);
            DrawPoint(g, p2, "P2", Color.Red);
            DrawPoint(g, p3, "P3", Color.DarkGreen);
        }

        private void DrawPoint(
            Graphics g, PointF point, string name, Color color)
        {
            PointF s = ToScreen(point);

            using Brush brush = new SolidBrush(color);
            using Font font = new Font("Arial", 9, FontStyle.Bold);

            g.FillEllipse(brush, s.X - 6, s.Y - 6, 12, 12);

            g.DrawString(name, font, brush, s.X + 8, s.Y - 18);
        }

        private void GraphPanel_MouseDown(
            object? sender, MouseEventArgs e)
        {
            PointF mouse = new PointF(e.X, e.Y);

            PointF[] points = { p0, p1, p2, p3 };


            for (int i = 1; i <= 2; i++)
            {
                PointF screenPoint = ToScreen(points[i]);

                float dx = mouse.X - screenPoint.X;
                float dy = mouse.Y - screenPoint.Y;

                if (dx * dx + dy * dy <= 18 * 18)
                {
                    selectedPoint = i;
                    graphPanel.Capture = true;
                    break;
                }
            }
        }

        private void GraphPanel_MouseMove(
            object? sender, MouseEventArgs e)
        {
            if (selectedPoint == -1)
                return;

            PointF point = ToGraph(new PointF(e.X, e.Y));

            point.X = Math.Max(0, Math.Min(255, point.X));
            point.Y = Math.Max(0, Math.Min(255, point.Y));

            const float minGap = 1f;

            if (selectedPoint == 1)
            {
                point.X = Math.Min(point.X, p2.X - minGap);
                p1 = point;
            }
            else if (selectedPoint == 2)
            {
                point.X = Math.Max(point.X, p1.X + minGap);
                p2 = point;
            }

            graphPanel.Invalidate();
        }

        private void GraphPanel_MouseUp(
            object? sender, MouseEventArgs e)
        {
            selectedPoint = -1;
            graphPanel.Capture = false;
        }


        public byte[] GetBrightnessLut()
        {
            byte[] lut = new byte[256];

            for (int input = 0; input < 256; input++)
            {
                float targetX = input;
                float low = 0f;
                float high = 1f;

                for (int iteration = 0; iteration < 30; iteration++)
                {
                    float mid = (low + high) / 2f;
                    float x = Bezier(mid).X;

                    if (x < targetX)
                        low = mid;
                    else
                        high = mid;
                }

                float t = (low + high) / 2f;
                float outputY = Bezier(t).Y;

                int value = (int)Math.Round(outputY);
                value = Math.Max(0, Math.Min(255, value));

                lut[input] = (byte)value;
            }

            lut[0] = 0;
            lut[255] = 255;

            return lut;
        }
    }
}
