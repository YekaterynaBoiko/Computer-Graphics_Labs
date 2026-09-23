using System.Net.Http.Headers;
using System.Security;
using System.Drawing;

namespace Lab2
{
    public partial class Form1 : Form
    {
        private PictureBox pictureSource;
        private PictureBox pictureTarget;
        private PictureBox pictureResult;
        private PictureBox pictureRed;
        private PictureBox pictureGreen;
        private PictureBox pictureBlue;
        private PictureBox pictureHistogram;

        private Button btnLoadSource;
        private Button btnLoadTarget;
        private Button btnCorrect;

        private ComboBox comboHistogramChannel;
        private ComboBox comboHistogramImage;
        private ComboBox comboColorModel;

        private Label redLabel;
        private Label greenLabel;
        private Label blueLabel;

        private CheckBox checkL;
        private CheckBox checkA;
        private CheckBox checkB;

        private Point selectionStart;
        private Point selectionEnd;
        private bool isSelecting = false;
        private Rectangle selectedRegion = Rectangle.Empty;
        private bool circularSelection = false;
        private int brushRadius = 30;
        private Point brushCenter;

        public Form1()
        {
            InitializeComponent();
            CreateInterface();
        }

        private void CreateInterface()
        {
            Text = "Статистична корекція кольору";
            Width = 1200;
            Height = 720;
            StartPosition = FormStartPosition.CenterScreen;

            // Source
            pictureSource = new PictureBox();
            pictureSource.Location = new Point(30, 50);
            pictureSource.Size = new Size(350, 300);
            pictureSource.SizeMode = PictureBoxSizeMode.Zoom;
            pictureSource.BorderStyle = BorderStyle.FixedSingle;

            // Target
            pictureTarget = new PictureBox();
            pictureTarget.Location = new Point(420, 50);
            pictureTarget.Size = new Size(350, 300);
            pictureTarget.SizeMode = PictureBoxSizeMode.Zoom;
            pictureTarget.BorderStyle = BorderStyle.FixedSingle;
            pictureTarget.MouseDown += PictureTarget_MouseDown;
            pictureTarget.MouseMove += PictureTarget_MouseMove;
            pictureTarget.MouseUp += PictureTarget_MouseUp;
            pictureTarget.Paint += PictureTarget_Paint;

            // Result
            pictureResult = new PictureBox();
            pictureResult.Location = new Point(810, 50);
            pictureResult.Size = new Size(350, 300);
            pictureResult.SizeMode = PictureBoxSizeMode.Zoom;
            pictureResult.BorderStyle = BorderStyle.FixedSingle;

            // Red channel
            pictureRed = new PictureBox();
            pictureRed.Location = new Point(30, 470);
            pictureRed.Size = new Size(200, 150);
            pictureRed.SizeMode = PictureBoxSizeMode.Zoom;
            pictureRed.BorderStyle = BorderStyle.FixedSingle;

            // Green channel
            pictureGreen = new PictureBox();
            pictureGreen.Location = new Point(250, 470);
            pictureGreen.Size = new Size(200, 150);
            pictureGreen.SizeMode = PictureBoxSizeMode.Zoom;
            pictureGreen.BorderStyle = BorderStyle.FixedSingle;

            // Blue channel
            pictureBlue = new PictureBox();
            pictureBlue.Location = new Point(470, 470);
            pictureBlue.Size = new Size(200, 150);
            pictureBlue.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBlue.BorderStyle = BorderStyle.FixedSingle;

            // Histogram
            pictureHistogram = new PictureBox();
            pictureHistogram.Location = new Point(700, 470);
            pictureHistogram.Size = new Size(450, 150);
            pictureHistogram.BorderStyle = BorderStyle.FixedSingle;

            // Кнопка Source
            btnLoadSource = new Button();
            btnLoadSource.Text = "Завантажити Source";
            btnLoadSource.Location = new Point(100, 370);
            btnLoadSource.Size = new Size(200, 40);
            btnLoadSource.Click += LoadSource;

            // Кнопка Target
            btnLoadTarget = new Button();
            btnLoadTarget.Text = "Завантажити Target";
            btnLoadTarget.Location = new Point(495, 370);
            btnLoadTarget.Size = new Size(200, 40);
            btnLoadTarget.Click += LoadTarget;

            // Кнопка корекції
            btnCorrect = new Button();
            btnCorrect.Text = "Виконати корекцію";
            btnCorrect.Location = new Point(850, 370);
            btnCorrect.Size = new Size(200, 40);
            btnCorrect.Click += CorrectImage;

            // Підписи
            Label sourceLabel = new Label();
            sourceLabel.Text = "SOURCE";
            sourceLabel.Location = new Point(170, 20);
            sourceLabel.AutoSize = true;

            Label targetLabel = new Label();
            targetLabel.Text = "TARGET";
            targetLabel.Location = new Point(560, 20);
            targetLabel.AutoSize = true;

            Label resultLabel = new Label();
            resultLabel.Text = "RESULT";
            resultLabel.Location = new Point(950, 20);
            resultLabel.AutoSize = true;

            redLabel = new Label();
            redLabel.Text = "Червоний канал (R)";
            redLabel.Location = new Point(70, 630);
            redLabel.AutoSize = true;

            greenLabel = new Label();
            greenLabel.Text = "Зелений канал (G)";
            greenLabel.Location = new Point(290, 630);
            greenLabel.AutoSize = true;

            blueLabel = new Label();
            blueLabel.Text = "Синій канал (B)";
            blueLabel.Location = new Point(510, 630);
            blueLabel.AutoSize = true;

            checkL = new CheckBox();
            checkL.Text = "L";
            checkL.Location = new Point(800, 440);
            checkL.AutoSize = true;
            checkL.Checked = true;
            Controls.Add(checkL);

            checkA = new CheckBox();
            checkA.Text = "A";
            checkA.Location = new Point(860, 440);
            checkA.AutoSize = true;
            checkA.Checked = true;
            Controls.Add(checkA);

            checkB = new CheckBox();
            checkB.Text = "B";
            checkB.Location = new Point(920, 440);
            checkB.AutoSize = true;
            checkB.Checked = true;
            Controls.Add(checkB);

            ComboBox comboSelectionMode = new ComboBox();
            comboSelectionMode.Location = new Point(960, 430);
            comboSelectionMode.Size = new Size(200, 40);
            comboSelectionMode.Items.Add("Прямокутна область");
            comboSelectionMode.Items.Add("Круговий пензель");
            comboSelectionMode.SelectedIndex = 0;
            comboSelectionMode.SelectedIndexChanged += (sender, e) =>
            {
                circularSelection = comboSelectionMode.SelectedIndex == 1;

                selectedRegion = Rectangle.Empty;
                selectionStart = Point.Empty;
                selectionEnd = Point.Empty;

                pictureTarget.Invalidate();
            };
            Controls.Add(comboSelectionMode);

            Label labelCorrectionChannels = new Label();
            labelCorrectionChannels.Text = "Канали для корекції:";
            labelCorrectionChannels.Location = new Point(700, 410);
            labelCorrectionChannels.AutoSize = true;
            Controls.Add(labelCorrectionChannels);

            comboHistogramChannel = new ComboBox();
            comboHistogramChannel.Location = new Point(875, 630);
            comboHistogramChannel.Size = new Size(200, 30);
            comboHistogramChannel.Items.Add("Червоний канал (R)");
            comboHistogramChannel.Items.Add("Зелений канал (G)");
            comboHistogramChannel.Items.Add("Синій канал (B)");
            comboHistogramChannel.SelectedIndex = 0;
            comboHistogramChannel.SelectedIndexChanged += HistogramChannelChanged;
            Controls.Add(comboHistogramChannel);

            comboHistogramImage = new ComboBox();
            comboHistogramImage.Location = new Point(290, 430);
            comboHistogramImage.Size = new Size(200, 30);
            comboHistogramImage.Items.Add("SOURCE");
            comboHistogramImage.Items.Add("TARGET");
            comboHistogramImage.Items.Add("RESULT");
            comboHistogramImage.SelectedIndex = 1;
            comboHistogramImage.SelectedIndexChanged += HistogramImageChanged;
            Controls.Add(comboHistogramImage);

            comboColorModel = new ComboBox();
            comboColorModel.Location = new Point(185, 430);
            comboColorModel.Size = new Size(80, 30);
            comboColorModel.Items.Add("RGB");
            comboColorModel.Items.Add("HSL");
            comboColorModel.Items.Add("HSV");
            comboColorModel.Items.Add("YIQ");
            comboColorModel.SelectedIndex = 0;

            Controls.Add(comboColorModel);
            comboColorModel.SelectedIndexChanged += ColorModelChanged;

            // Додаємо елементи на форму
            Controls.Add(pictureSource);
            Controls.Add(pictureTarget);
            Controls.Add(pictureResult);
            Controls.Add(pictureRed);
            Controls.Add(pictureGreen);
            Controls.Add(pictureBlue);
            Controls.Add(pictureHistogram);

            Controls.Add(btnLoadSource);
            Controls.Add(btnLoadTarget);
            Controls.Add(btnCorrect);

            Controls.Add(sourceLabel);
            Controls.Add(targetLabel);
            Controls.Add(resultLabel);
            Controls.Add(redLabel);
            Controls.Add(greenLabel);
            Controls.Add(blueLabel);
        }

        private void PictureTarget_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) 
                return;

            selectionStart = e.Location;
            selectionEnd = e.Location;
            isSelecting = true;

            if (circularSelection)
            {
                selectedRegion = GetBrushRectangle(e.Location);
                brushCenter = new Point(
                    selectedRegion.X + selectedRegion.Width / 2,
                    selectedRegion.Y + selectedRegion.Height / 2);
            }

            pictureTarget.Invalidate();
        }

        private Rectangle GetBrushRectangle(Point point)
        {
            if (pictureTarget.Image == null)
                return Rectangle.Empty;

            Bitmap image = (Bitmap)pictureTarget.Image;

            int centerX = point.X * image.Width
                          / pictureTarget.ClientSize.Width;

            int centerY = point.Y * image.Height
                          / pictureTarget.ClientSize.Height;

            int diameter = brushRadius * 2;

            int x = centerX - brushRadius;
            int y = centerY - brushRadius;

            x = Math.Max(0, x);
            y = Math.Max(0, y);

            if (x + diameter > image.Width)
                x = image.Width - diameter;

            if (y + diameter > image.Height)
                y = image.Height - diameter;

            return new Rectangle(
                x,
                y,
                Math.Min(diameter, image.Width),
                Math.Min(diameter, image.Height));
        }

        private void PictureTarget_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isSelecting)
                return;

            selectionEnd = e.Location;

            if (circularSelection)
            {
                selectedRegion = GetBrushRectangle(e.Location);

                brushCenter = new Point(
                    selectedRegion.X + selectedRegion.Width / 2,
                    selectedRegion.Y + selectedRegion.Height / 2);
            }
            pictureTarget.Invalidate();
        }

        private void PictureTarget_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            selectionEnd = e.Location;
            isSelecting = false;

            if (!circularSelection)
            {
                selectedRegion = GetImageSelectionRectangle();
            }
            pictureTarget.Invalidate();
        }

        private void PictureTarget_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (!isSelecting && selectionStart == selectionEnd)
                return;

            using (Pen pen = new Pen(Color.Red, 2))
            {
                if (circularSelection)
                {
                    int diameter = brushRadius * 2;

                    e.Graphics.DrawEllipse(
                        pen,
                        selectionEnd.X - brushRadius,
                        selectionEnd.Y - brushRadius,
                        diameter,
                        diameter);
                }
                else
                {
                    int x = Math.Min(selectionStart.X, selectionEnd.X);
                    int y = Math.Min(selectionStart.Y, selectionEnd.Y);
                    int width = Math.Abs(selectionEnd.X - selectionStart.X);
                    int height = Math.Abs(selectionEnd.Y - selectionStart.Y);

                    e.Graphics.DrawRectangle(
                        pen,
                        x,
                        y,
                        width,
                        height);
                }
            }
        }

        private Rectangle GetImageSelectionRectangle()
        {
            if (pictureTarget.Image == null)
                return Rectangle.Empty;

            Bitmap image = (Bitmap)pictureTarget.Image;

            float imageRatio =
                (float)image.Width / image.Height;

            float boxRatio =
                (float)pictureTarget.ClientSize.Width /
                pictureTarget.ClientSize.Height;

            int displayedWidth;
            int displayedHeight;
            int offsetX;
            int offsetY;

            if (imageRatio > boxRatio)
            {
                displayedWidth = pictureTarget.ClientSize.Width;
                displayedHeight = (int)(displayedWidth / imageRatio);

                offsetX = 0;
                offsetY =
                    (pictureTarget.ClientSize.Height - displayedHeight) / 2;
            }
            else
            {
                displayedHeight = pictureTarget.ClientSize.Height;
                displayedWidth = (int)(displayedHeight * imageRatio);

                offsetX =
                    (pictureTarget.ClientSize.Width - displayedWidth) / 2;
                offsetY = 0;
            }

            int x1 = (selectionStart.X - offsetX) * image.Width
                     / displayedWidth;

            int y1 = (selectionStart.Y - offsetY) * image.Height
                     / displayedHeight;

            int x2 = (selectionEnd.X - offsetX) * image.Width
                     / displayedWidth;

            int y2 = (selectionEnd.Y - offsetY) * image.Height
                     / displayedHeight;

            x1 = Math.Max(0, Math.Min(image.Width - 1, x1));
            y1 = Math.Max(0, Math.Min(image.Height - 1, y1));
            x2 = Math.Max(0, Math.Min(image.Width - 1, x2));
            y2 = Math.Max(0, Math.Min(image.Height - 1, y2));

            return new Rectangle(
                Math.Min(x1, x2),
                Math.Min(y1, y2),
                Math.Abs(x2 - x1),
                Math.Abs(y2 - y1));
        }
        private void ColorModelChanged(object sender, EventArgs e)
        {
            if (comboColorModel.SelectedIndex == 0)
            {
                redLabel.Text = "Червоний канал (R)";
                greenLabel.Text = "Зелений канал (G)";
                blueLabel.Text = "Синій канал (B)";
            }
            else if (comboColorModel.SelectedIndex == 1)
            {
                redLabel.Text = "Канал H";
                greenLabel.Text = "Канал S";
                blueLabel.Text = "Канал L";
            }
            else if (comboColorModel.SelectedIndex == 2)
            {
                redLabel.Text = "Канал H";
                greenLabel.Text = "Канал S";
                blueLabel.Text = "Канал V";
            }
            else if (comboColorModel.SelectedIndex == 3)
            {
                redLabel.Text = "Канал Y";
                greenLabel.Text = "Канал I";
                blueLabel.Text = "Канал Q";
            }
        }

        private void HistogramImageChanged(object sender, EventArgs e)
        {
            UpdateHistogram();
            UpdateChannelImages();
        }

        private void UpdateChannelImages()
        {
            Bitmap image = null;

            if (comboHistogramImage.SelectedIndex == 0)
                image = (Bitmap)pictureSource.Image;
            else if (comboHistogramImage.SelectedIndex == 1)
                image = (Bitmap)pictureTarget.Image;
            else if (comboHistogramImage.SelectedIndex == 2)
                image = (Bitmap)pictureResult.Image;

            if (image == null)
                return;

            if (comboColorModel.SelectedIndex == 0)
            {
                pictureRed.Image = ImageProcessor.CreateChannelImage(image, 'R');
                pictureGreen.Image = ImageProcessor.CreateChannelImage(image, 'G');
                pictureBlue.Image = ImageProcessor.CreateChannelImage(image, 'B');
            }
            else if (comboColorModel.SelectedIndex == 1)
            {
                pictureRed.Image = ImageProcessor.CreateColorModelChannelImage(image, 0, "HSL");
                pictureGreen.Image = ImageProcessor.CreateColorModelChannelImage(image, 1, "HSL");
                pictureBlue.Image = ImageProcessor.CreateColorModelChannelImage(image, 2, "HSL");
            }
            else if (comboColorModel.SelectedIndex == 2)
            {
                pictureRed.Image = ImageProcessor.CreateColorModelChannelImage(image, 0, "HSV");
                pictureGreen.Image = ImageProcessor.CreateColorModelChannelImage(image, 1, "HSV");
                pictureBlue.Image = ImageProcessor.CreateColorModelChannelImage(image, 2, "HSV");
            }
            else if (comboColorModel.SelectedIndex == 3)
            {
                pictureRed.Image = ImageProcessor.CreateColorModelChannelImage(image, 0, "YIQ");
                pictureGreen.Image = ImageProcessor.CreateColorModelChannelImage(image, 1, "YIQ");
                pictureBlue.Image = ImageProcessor.CreateColorModelChannelImage(image, 2, "YIQ");
            }
        }

        private void UpdateHistogram()
        {
            Bitmap image = null;

            if (comboHistogramImage.SelectedIndex == 0)
                image = (Bitmap)pictureSource.Image;
            else if (comboHistogramImage.SelectedIndex == 1)
                image = (Bitmap)pictureTarget.Image;
            else if (comboHistogramImage.SelectedIndex == 2)
                image = (Bitmap)pictureResult.Image;

            if (image == null)
                return;

            char channel = 'R';
            Color histogramColor = Color.Red;

            if (comboHistogramChannel.SelectedIndex == 1)
            {
                channel = 'G';
                histogramColor = Color.Green;
            }
            else if (comboHistogramChannel.SelectedIndex == 2)
            {
                channel = 'B';
                histogramColor = Color.Blue;
            }

            int[] histogram =
                ImageProcessor.CalculateHistogram(image, channel);

            pictureHistogram.Image =
                ImageProcessor.DrawHistogram(
                    histogram,
                    450,
                    150,
                    histogramColor);
        }

        private void HistogramChannelChanged(object sender, EventArgs e)
        {
            UpdateHistogram();
        }

        private void LoadSource(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files|*.bmp;*.jpg;*.jpeg;*.png";

            dialog.InitialDirectory = Environment.GetFolderPath(
                Environment.SpecialFolder.MyPictures);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pictureSource.Image = new Bitmap(dialog.FileName);
                UpdateHistogram();
            }
        }

        private void LoadTarget(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Image files|*.bmp;*.jpg;*.jpeg;*.png";

            dialog.InitialDirectory = Environment.GetFolderPath(
                Environment.SpecialFolder.MyPictures);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap image = new Bitmap(dialog.FileName);

                pictureTarget.Image = image;

                pictureRed.Image = ImageProcessor.CreateChannelImage(
                    image, 'R');

                pictureGreen.Image = ImageProcessor.CreateChannelImage(
                    image, 'G');

                pictureBlue.Image = ImageProcessor.CreateChannelImage(
                    image, 'B');

                UpdateHistogram();
            }
        }

        private void CorrectImage(object sender, EventArgs e)
        {
            if (pictureSource.Image == null || pictureTarget.Image == null)
            {
                MessageBox.Show("Спочатку завантажте SOURCE і TARGET.");
                return;
            }

            if (selectedRegion == Rectangle.Empty)
            {
                MessageBox.Show("Спочатку виділіть область на TARGET.");
                return;
            }

            Bitmap source = (Bitmap)pictureSource.Image;
            Bitmap target = (Bitmap)pictureTarget.Image;
            Bitmap result = ImageProcessor.CorrectImageLab(source, target,
                checkL.Checked, checkA.Checked, checkB.Checked, selectedRegion,
                circularSelection, brushCenter, brushRadius);
            pictureResult.Image = result;

            comboHistogramImage.SelectedIndex = 2;
            UpdateHistogram();
        }
    }
}
