using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Lab3
{
    public class CustomKernelForm : Form
    {
        private ComboBox cmbSize = null!;
        private Panel pnlKernel = null!;
        private Button btnApply = null!;

        private TextBox[,] cells = new TextBox[0, 0];

        private int kernelSize = 3;

        public double[,] Kernel { get; private set; }
            = new double[3, 3];

        public CustomKernelForm()
        {
            Text = "Власне ядро фільтра";
            ClientSize = new Size(380, 430);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            Label lblSize = new Label();
            lblSize.Text = "Розмір ядра:";
            lblSize.Location = new Point(20, 20);
            lblSize.AutoSize = true;
            Controls.Add(lblSize);

            cmbSize = new ComboBox();
            cmbSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSize.Items.AddRange(new object[]
            {
                "3 x 3",
                "5 x 5",
                "7 x 7",
                "9 x 9"
            });
            cmbSize.SelectedIndex = 0;
            cmbSize.Location = new Point(120, 16);
            cmbSize.Width = 100;
            cmbSize.SelectedIndexChanged += CmbSize_SelectedIndexChanged;
            Controls.Add(cmbSize);

            pnlKernel = new Panel();
            pnlKernel.Location = new Point(20, 60);
            pnlKernel.Size = new Size(340, 290);
            pnlKernel.AutoScroll = true;
            Controls.Add(pnlKernel);

            btnApply = new Button();
            btnApply.Text = "Застосувати";
            btnApply.Size = new Size(140, 35);
            btnApply.Location = new Point(120, 370);
            btnApply.Click += BtnApply_Click;
            Controls.Add(btnApply);

            CreateKernelGrid();
        }

        private void CmbSize_SelectedIndexChanged(
            object? sender, EventArgs e)
        {
            kernelSize = cmbSize.SelectedIndex switch
            {
                0 => 3,
                1 => 5,
                2 => 7,
                3 => 9,
                _ => 3
            };

            CreateKernelGrid();
        }

        private void CreateKernelGrid()
        {
            pnlKernel.Controls.Clear();

            cells = new TextBox[kernelSize, kernelSize];

            int cellSize = kernelSize >= 9 ? 32 : 42;
            int gap = 5;

            int gridWidth = kernelSize * (cellSize + gap) - gap;

            int startX = Math.Max(0, (320 - gridWidth) / 2);
            int startY = 10;

            for (int y = 0; y < kernelSize; y++)
            {
                for (int x = 0; x < kernelSize; x++)
                {
                    TextBox cell = new TextBox();

                    cell.Size = new Size(cellSize, 25);
                    cell.Location = new Point(
                        startX + x * (cellSize + gap),
                        startY + y * (30 + gap));

                    cell.Text = (x == kernelSize / 2 &&
                                 y == kernelSize / 2)
                        ? "1"
                        : "0";

                    cell.TextAlign = HorizontalAlignment.Center;

                    cells[y, x] = cell;
                    pnlKernel.Controls.Add(cell);
                }
            }
        }

        private void BtnApply_Click(object? sender, EventArgs e)
        {
            double[,] kernel =
                new double[kernelSize, kernelSize];

            for (int y = 0; y < kernelSize; y++)
            {
                for (int x = 0; x < kernelSize; x++)
                {
                    string text = cells[y, x].Text.Trim()
                        .Replace(',', '.');

                    if (!double.TryParse(
                        text,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out double value)
                        || double.IsNaN(value)
                        || double.IsInfinity(value))
                    {
                        MessageBox.Show(
                            $"Некоректне значення в рядку {y + 1}, " +
                            $"стовпці {x + 1}.");

                        return;
                    }

                    kernel[y, x] = value;
                }
            }

            Kernel = kernel;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}