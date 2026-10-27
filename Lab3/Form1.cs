using System.Drawing;
using System.Windows.Forms;
using System;
namespace Lab3
{
    public partial class Form1 : Form
    {
        private Bitmap? originalImage;
        private Bitmap? resultImage;

        private Button btnOpen;
        private Button btnBlur;
        private Button btnGaussian;
        private Button btnSharpen;
        private Button btnEdge;
        private Button btnEmboss;
        private Button btnMedian;
        private Button btnCustomKernel;
        private Button btnBezier;
        private Button btnBrightnessCurve;

        private Label lblOriginal;
        private Label lblResult;
        private Label lblBlurSize;

        private PictureBox picOriginal;
        private PictureBox picResult;

        private ComboBox cmbBlurSize;

        public Form1()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {

            Text = "Lab 3 - Image Processing";
            Size = new Size(1100, 810);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(800, 500);

            btnOpen = new Button();
            btnOpen.Text = "Відкрити зображення";
            btnOpen.Location = new Point(20, 510);
            btnOpen.Size = new Size(200, 40);
            btnOpen.Click += btnOpen_Click;

            btnBlur = new Button();
            btnBlur.Text = "Розмиття Box (Box Blur)";
            btnBlur.Location = new Point(20, 560);
            btnBlur.Size = new Size(200, 40);
            btnBlur.Click += btnBlur_Click;

            btnGaussian = new Button();
            btnGaussian.Text = "Розмиття Гауса";
            btnGaussian.Location = new Point(20, 610);
            btnGaussian.Size = new Size(200, 40);
            btnGaussian.Click += btnGaussian_Click;

            btnSharpen = new Button();
            btnSharpen.Text = "Підвищення різкості";
            btnSharpen.Location = new Point(20, 660);
            btnSharpen.Size = new Size(200, 40);
            btnSharpen.Click += btnSharpen_Click;

            btnEdge = new Button();
            btnEdge.Text = "Виявлення контурів";
            btnEdge.Location = new Point(20, 710);
            btnEdge.Size = new Size(200, 40);
            btnEdge.Click += btnEdge_Click;

            btnEmboss = new Button();
            btnEmboss.Text = "Тиснення";
            btnEmboss.Location = new Point(230, 510);
            btnEmboss.Size = new Size(190, 38);
            btnEmboss.Click += btnEmboss_Click;

            btnMedian = new Button();
            btnMedian.Text = "Медіанний фільтр";
            btnMedian.Location = new Point(230, 560);
            btnMedian.Size = new Size(190, 38);
            btnMedian.Click += btnMedian_Click;

            btnCustomKernel = new Button();
            btnCustomKernel.Text = "Власне ядро";
            btnCustomKernel.Location = new Point(230, 610);
            btnCustomKernel.Size = new Size(190, 38);
            btnCustomKernel.Click += btnCustomKernel_Click;

            btnBrightnessCurve = new Button();
            btnBrightnessCurve.Text = "Корекція яскравості";
            btnBrightnessCurve.Location = new Point(230, 710);
            btnBrightnessCurve.Size = new Size(190, 38);
            btnBrightnessCurve.Click += btnBrightnessCurve_Click;

            btnBezier = new Button();
            btnBezier.Text = "Крива Безьє";
            btnBezier.Location = new Point(230, 660);
            btnBezier.Size = new Size(190, 38);
            btnBezier.Click += BtnBezier_Click;

            lblOriginal = new Label();
            lblOriginal.Text = "Оригінал";
            lblOriginal.Location = new Point(20, 20);
            lblOriginal.AutoSize = true;

            lblResult = new Label();
            lblResult.Text = "Результат";
            lblResult.Location = new Point(550, 20);
            lblResult.AutoSize = true;

            lblBlurSize = new Label();
            lblBlurSize.Text = "Розмір ядра";
            lblBlurSize.Location = new Point(430, 510);
            lblBlurSize.AutoSize = true;

            picOriginal = new PictureBox();
            picOriginal.Location = new Point(20, 50);
            picOriginal.Size = new Size(500, 450);
            picOriginal.BorderStyle = BorderStyle.FixedSingle;
            picOriginal.SizeMode = PictureBoxSizeMode.Zoom;

            picResult = new PictureBox();
            picResult.Location = new Point(550, 50);
            picResult.Size = new Size(500, 450);
            picResult.BorderStyle = BorderStyle.FixedSingle;
            picResult.SizeMode = PictureBoxSizeMode.Zoom;

            cmbBlurSize = new ComboBox();
            cmbBlurSize.Location = new Point(430, 540);
            cmbBlurSize.Size = new Size(100, 30);
            cmbBlurSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBlurSize.Items.Add("3x3");
            cmbBlurSize.Items.Add("5x5");
            cmbBlurSize.Items.Add("9x9");
            cmbBlurSize.SelectedIndex = 0;

            Controls.Add(cmbBlurSize);

            Controls.Add(btnOpen);
            Controls.Add(btnBlur);
            Controls.Add(btnGaussian);
            Controls.Add(btnSharpen);
            Controls.Add(btnEdge);
            Controls.Add(btnEmboss);
            Controls.Add(btnMedian);
            Controls.Add(btnCustomKernel);
            Controls.Add(btnBezier);
            Controls.Add(btnBrightnessCurve);

            Controls.Add(lblOriginal);
            Controls.Add(lblResult);
            Controls.Add(lblBlurSize);

            Controls.Add(picOriginal);
            Controls.Add(picResult);
        }


        private void btnOpen_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Зображення|*.jpg;*.jpeg;*.png;*.bmp";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bitmap loaded;

                    using (Image temp = Image.FromFile(dialog.FileName))
                    {
                        loaded = new Bitmap(temp);
                    }

                    picOriginal.Image = null;
                    picResult.Image = null;

                    originalImage?.Dispose();
                    resultImage?.Dispose();

                    originalImage = loaded;
                    resultImage = new Bitmap(loaded);

                    picOriginal.Image = originalImage;
                    picResult.Image = resultImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Не вдалося відкрити зображення:\n" + ex.Message,
                        "Помилка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            picOriginal.Image = null;
            picResult.Image = null;

            originalImage?.Dispose();
            resultImage?.Dispose();

            base.OnFormClosed(e);
        }

        private Bitmap ApplyBoxBlur(Bitmap source, int kernelSize)
        {
            int width = source.Width;
            int height = source.Height;

            Bitmap result = new Bitmap(width, height);

            int radius = kernelSize / 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int sumR = 0;
                    int sumG = 0;
                    int sumB = 0;
                    int count = 0;

                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        for (int dx = -radius; dx <= radius; dx++)
                        {
                            int nx = x + dx;
                            int ny = y + dy;

                            // Не виходимо за межі зображення
                            if (nx < 0 || nx >= width ||
                                ny < 0 || ny >= height)
                            {
                                continue;
                            }
                            Color pixel = source.GetPixel(nx, ny);

                            sumR += pixel.R;
                            sumG += pixel.G;
                            sumB += pixel.B;
                            count++;
                        }
                    }
                    int r = sumR / count;
                    int g = sumG / count;
                    int b = sumB / count;

                    result.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            return result;
        }

        private Bitmap ApplyGaussianBlur(Bitmap source)
        {
            int[,] kernel =
            {
                {1, 2, 1 },
                {2, 4, 2 },
                {1, 2, 1 }
            };

            int width = source.Width;
            int height = source.Height;

            Bitmap result = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int sumR = 0;
                    int sumG = 0;
                    int sumB = 0;

                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            int nx = Math.Clamp(x + dx, 0, width - 1);
                            int ny = Math.Clamp(y + dy, 0, height - 1);

                            Color pixel = source.GetPixel(nx, ny);
                            int weight = kernel[dy + 1, dx + 1];

                            sumR += pixel.R * weight;
                            sumG += pixel.G * weight;
                            sumB += pixel.B * weight;
                        }
                    }

                    int r = sumR / 16;
                    int g = sumG / 16;
                    int b = sumB / 16;

                    result.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            return result;
        }

        private Bitmap ApplySharpen(Bitmap source)
        {
            int[,] kernel =
            {
                {0, -1, 0 },
                {-1, 5, -1 },
                {0, -1, 0 }
            };

            int width = source.Width;
            int height = source.Height;

            Bitmap result = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int sumR = 0;
                    int sumG = 0;
                    int sumB = 0;

                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            int nx = Math.Clamp(x + dx, 0, width - 1);
                            int ny = Math.Clamp(y + dy, 0, height - 1);

                            Color pixel = source.GetPixel(nx, ny);
                            int weight = kernel[dy + 1, dx + 1];

                            sumR += pixel.R * weight;
                            sumG += pixel.G * weight;
                            sumB += pixel.B * weight;
                        }
                    }

                    int r = Math.Clamp(sumR, 0, 255);
                    int g = Math.Clamp(sumG, 0, 255);
                    int b = Math.Clamp(sumB, 0, 255);

                    result.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            return result;
        }

        private Bitmap ApplyEdgeDetection(Bitmap source)
        {
            int[,] kernel =
            {
                {0, -1, 0},
                {-1, 4, -1 },
                {0, -1, 0 }
            };

            int width = source.Width;
            int height = source.Height;

            Bitmap result = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int sum = 0;

                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            int nx = Math.Clamp(x + dx, 0, width - 1);
                            int ny = Math.Clamp(y + dy, 0, height - 1);

                            Color pixel = source.GetPixel(nx, ny);

                            // Яскравість пікселя
                            int gray = (pixel.R + pixel.G + pixel.B) / 3;

                            int weight = kernel[dy + 1, dx + 1];
                             
                            sum += gray * weight;
                        }
                    }

                    int edge = Math.Clamp(Math.Abs(sum) * 2, 0, 255);

                    result.SetPixel(
                        x, y,
                        Color.FromArgb(edge, edge, edge));
                }
            }
            return result;
        }

        private Bitmap ApplyEmboss(Bitmap source)
        {
            int[,] kernel =
            {
                {-2, -1, 0},
                {-1,  1, 1},
                { 0,  1, 2}
            };

            int width = source.Width;
            int height = source.Height;

            Bitmap result = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int sum = 0;
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            int nx = Math.Clamp(x + dx, 0, width - 1);
                            int ny = Math.Clamp(y + dy, 0, height - 1);

                            Color pixel = source.GetPixel(nx, ny);

                            int gray = (pixel.R + pixel.G + pixel.B) / 3;
                            int weight = kernel[dy + 1, dx + 1];

                            sum += gray * weight;
                        }
                    }
                    int value = Math.Clamp(sum + 128, 0, 255);

                    result.SetPixel(
                        x, y,
                        Color.FromArgb(value, value, value));
                }
            }
            return result;
        }

        private Bitmap ApplyMedianBlur(Bitmap source, int kernelSize)
        {
            int width = source.Width;
            int height = source.Height;

            Bitmap result = new Bitmap(width, height);

            int radius = kernelSize / 2;
            int count = kernelSize * kernelSize;

            int[] reds = new int[count];
            int[] greens = new int[count];
            int[] blues = new int[count];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = 0;

                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        for (int dx = -radius; dx <= radius; dx++)
                        {
                            int nx = Math.Clamp(x + dx, 0, width - 1);
                            int ny = Math.Clamp(y + dy, 0, height - 1);

                            Color pixel = source.GetPixel(nx, ny);

                            reds[index] = pixel.R;
                            greens[index] = pixel.G;
                            blues[index] = pixel.B;

                            index++;
                        }
                    }

                    Array.Sort(reds);
                    Array.Sort(greens);
                    Array.Sort(blues);

                    int mid = count / 2;

                    result.SetPixel(
                        x, y,
                        Color.FromArgb(
                            reds[mid],
                            greens[mid],
                            blues[mid]));
                }
            }
            return result;
        }

        private Bitmap ApplyCustomKernel(Bitmap source, double[,] kernel)
        {
            int kernelHeight = kernel.GetLength(0);
            int kernelWidth = kernel.GetLength(1);

            if (kernelWidth % 2 == 0 || kernelHeight % 2 == 0)
                throw new ArgumentException("Розмір ядра має бути непарним.");

            int width = source.Width;
            int height = source.Height;

            Bitmap result = new Bitmap(width, height);

            int offsetX = kernelWidth / 2;
            int offsetY = kernelHeight / 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double sumR = 0;
                    double sumG = 0;
                    double sumB = 0;

                    for (int ky = 0; ky < kernelHeight; ky++)
                    {
                        for (int kx = 0; kx < kernelWidth; kx++)
                        {
                            int nx = Math.Clamp(x + kx - offsetX, 0, width - 1);
                            int ny = Math.Clamp(y + ky - offsetY, 0, height - 1);

                            Color pixel = source.GetPixel(nx, ny);
                            double weight = kernel[ky, kx];

                            sumR += pixel.R * weight;
                            sumG += pixel.G * weight;
                            sumB += pixel.B * weight;
                        }
                    }

                    int r = Math.Clamp((int)Math.Round(sumR), 0, 255);
                    int g = Math.Clamp((int)Math.Round(sumG), 0, 255);
                    int b = Math.Clamp((int)Math.Round(sumB), 0, 255);

                    result.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            return result;
        }

        private void btnBlur_Click(object? sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Спочатку завантажте зображення!");
                return;
            }

            int kernelSize = cmbBlurSize.SelectedIndex switch
            {
                0 => 3,
                1 => 5,
                2 => 9,
                _ => 3
            };

            Bitmap blurred = ApplyBoxBlur(originalImage, kernelSize);

            picResult.Image = null;
            resultImage?.Dispose();

            resultImage = blurred;
            picResult.Image = resultImage;
        }

        private void btnGaussian_Click(object? sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Спочатку завантажте зображення!");
                return;
            }
            Bitmap blurred = ApplyGaussianBlur(originalImage);

            picResult.Image = null;
            resultImage?.Dispose();

            resultImage = blurred;
            picResult.Image = resultImage;
        }

        private void btnSharpen_Click(object? sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Спочатку завантажте зображення!");
                return;
            }

            Bitmap sharpened = ApplySharpen(originalImage);

            picResult.Image = null;
            resultImage?.Dispose();

            resultImage = sharpened;
            picResult.Image = resultImage;
        }

        private void btnEdge_Click(object? sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Спочатку завантажте зображення!");
                return;
            }

            Bitmap edges = ApplyEdgeDetection(originalImage);

            picResult.Image = null;
            resultImage?.Dispose();

            resultImage = edges;
            picResult.Image = resultImage;

        }

        private Bitmap ApplyBrightnessCurve(Bitmap source,byte[] lut)
        {
            Bitmap result = new Bitmap(
                source.Width,
                source.Height);

            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    Color pixel = source.GetPixel(x, y);

                    int r = lut[pixel.R];
                    int g = lut[pixel.G];
                    int b = lut[pixel.B];

                    result.SetPixel(
                        x,
                        y,
                        Color.FromArgb(
                            pixel.A,
                            r,
                            g,
                            b));
                }
            }
            return result;
        }

        private void btnEmboss_Click(object? sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Спочатку завантажте зображення!");
                return;
            }

            Bitmap embossed = ApplyEmboss(originalImage);

            picResult.Image = null;
            resultImage?.Dispose();

            resultImage = embossed;
            picResult.Image = resultImage;
        }

        private void btnMedian_Click(object? sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Спочатку завантажте зображення!");
                return;
            }

            int kernelSize = cmbBlurSize.SelectedIndex switch
            {
                0 => 3,
                1 => 5,
                2 => 9,
                _ => 3
            };

            Bitmap median = ApplyMedianBlur(originalImage, kernelSize);

            picResult.Image = null;
            resultImage?.Dispose();

            resultImage = median;
            picResult.Image = resultImage;
        }


        private void btnCustomKernel_Click(object? sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Спочатку завантажте зображення!");
                return;
            }

            using CustomKernelForm form = new CustomKernelForm();

            if (form.ShowDialog(this) != DialogResult.OK)
                return;

            Bitmap filtered = ApplyCustomKernel(
                originalImage,
                form.Kernel);

            picResult.Image = null;
            resultImage?.Dispose();

            resultImage = filtered;
            picResult.Image = resultImage;
        }

        private void BtnBezier_Click(object? sender, EventArgs e)
        {
            using BezierForm form = new BezierForm();
            form.ShowDialog(this);
        }


        private void btnBrightnessCurve_Click(object? sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Спочатку відкрий зображення!");
                return;
            }

            using (BezierForm form = new BezierForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    byte[] lut = form.GetBrightnessLut();

                    Bitmap corrected = ApplyBrightnessCurve(originalImage, lut);

                    picResult.Image?.Dispose();
                    picResult.Image = corrected;
                    resultImage = corrected;
                }
            }
        }
    }
}
