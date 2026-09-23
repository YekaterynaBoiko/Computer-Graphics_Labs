using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lab2
{
    public class ImageProcessor
    {
        public static byte[,] GetRedChannel(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;

            byte[,] channel = new byte[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    channel[y, x] = pixel.R;
                }
            }

            return channel;
        }

        public static byte[,] GetGreenChannel(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;

            byte[,] channel = new byte[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    channel[y, x] = pixel.G;
                }
            }

            return channel;
        }

        public static byte[,] GetBlueChannel(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;

            byte[,] channel = new byte[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    channel[y, x] = pixel.B;
                }
            }

            return channel;
        }

        public static Bitmap CreateChannelImage(Bitmap image, char channel)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);

                    byte value;
                    if (channel == 'R')
                        value = pixel.R;
                    else if (channel == 'G')
                        value = pixel.G;
                    else
                        value = pixel.B;

                    result.SetPixel(x, y, Color.FromArgb(value, value, value));
                }
            }

            return result;
        }

        public static Bitmap CreateColorModelChannelImage(Bitmap image, int channel, string model)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);

                    double r = pixel.R / 255.0;
                    double g = pixel.G / 255.0;
                    double b = pixel.B / 255.0;

                    double value = 0;

                    if (model == "HSL")
                    {
                        double[] hsl = ColorConverter.RGBtoHSL(r, g, b);
                        value = hsl[channel];
                    }
                    else if (model == "HSV")
                    {
                        double[] hsv = ColorConverter.RGBtoHSV(r, g, b);
                        value = hsv[channel];
                    }
                    else if (model == "YIQ")
                    {
                        double[] yiq = ColorConverter.RGBtoYIQ(r, g, b);

                        if (channel == 0)
                            value = yiq[0];
                        else if (channel == 1)
                            value = (yiq[1] + 0.5957) / 1.1914;
                        else
                            value = (yiq[2] + 0.5226) / 1.0452;
                    }
                    int gray = (int)(value * 255);

                    if (gray < 0)
                        gray = 0;

                    if (gray > 255)
                        gray = 255;

                    result.SetPixel(
                        x,
                        y,
                        Color.FromArgb(gray, gray, gray));
                }
            }
            return result;
        }

        // обчислення математичного сподівання
        public static double CalculateMean(byte[,] channel)
        {
            int height = channel.GetLength(0);
            int width = channel.GetLength(1);

            double sum = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sum += channel[y, x];
                }
            }
            return sum / (width * height);
        }

        // обчислення дисперсії
        public static double CalculateVariance(byte[,] channel)
        {
            int height = channel.GetLength(0);
            int width = channel.GetLength(1);

            double mean = CalculateMean(channel);
            double sum = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double difference = channel[y, x] - mean;
                    sum += difference * difference;
                }
            }
            return sum / (width * height);
        }

        public static double CalculateMean(double[,] channel, Rectangle selectedRegion,
            bool circularSelection, Point brushCenter, int brushRadius)
        {
            int height = channel.GetLength(0);
            int width = channel.GetLength(1);

            double sum = 0;
            int count = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!IsInsideRegion(
                        x,
                        y,
                        selectedRegion,
                        circularSelection,
                        brushCenter,
                        brushRadius))
                    {
                        continue;
                    }

                    sum += channel[y, x];
                    count++;
                }
            }

            if (count == 0)
                return 0;

            return sum / count;
        }


        public static double CalculateVariance(double[,] channel, Rectangle selectedRegion,
            bool circularSelection, Point brushCenter, int brushRadius)
        {
            int height = channel.GetLength(0);
            int width = channel.GetLength(1);

            double mean = CalculateMean(channel, selectedRegion, circularSelection, brushCenter, brushRadius);
            double sum = 0;
            int count = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!IsInsideRegion(
                        x,
                        y,
                        selectedRegion,
                        circularSelection,
                        brushCenter,
                        brushRadius))
                    {
                        continue;
                    }

                    double difference = channel[y, x] - mean;
                    sum += difference * difference;
                    count++;
                }
            }

            if (count == 0)
                return 0;

            return sum / count;
        }


        public static int[] CalculateHistogram(Bitmap image, char channel)
        {
            int[] histogram = new int[256];

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);

                    int value;

                    if (channel == 'R')
                        value = pixel.R;
                    else if (channel == 'G')
                        value = pixel.G;
                    else
                        value = pixel.B;

                    histogram[value]++;
                }
            }
            return histogram;
        }

        public static Bitmap DrawHistogram(int[] histogram, int width, int height, Color lineColor)
        {
            Bitmap result = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.Clear(Color.White);

                int maxValue = 1;

                for (int i = 0; i < histogram.Length; i++)
                {
                    if (histogram[i] > maxValue)
                        maxValue = histogram[i];
                }

                float xStep = (float)width / 256;

                for (int i = 0; i < 256; i++)
                {
                    float barHeight =
                        (float)histogram[i] / maxValue * (height - 20);

                    float x = i * xStep;
                    float y = height - barHeight;

                    using (Pen pen = new Pen(lineColor))
                    {
                        g.DrawLine(pen,
                            x, height, x, y);
                    }
                }
            }
            return result;
        }

        public static byte[,] CorrectChannel(byte[,] sourceChannel,
            byte[,] targetChannel)
        {
            double sourceMean = CalculateMean(sourceChannel);
            double targetMean = CalculateMean(targetChannel);

            double sourceVariance = CalculateVariance(sourceChannel);
            double targetVariance = CalculateVariance(targetChannel);

            double sourceStd = Math.Sqrt(sourceVariance);
            double targetStd = Math.Sqrt(targetVariance);

            byte[,] result = new byte[
                targetChannel.GetLength(0),
                targetChannel.GetLength(1)];

            for (int y = 0; y < targetChannel.GetLength(0); y++)
            {
                for (int x = 0; x < targetChannel.GetLength(1); x++)
                {
                    double value = targetChannel[y, x];

                    double correctedValue =
                        (value - targetMean) *
                        (sourceStd / targetStd) +
                        sourceMean;

                    correctedValue = Math.Max(0, Math.Min(255, correctedValue));

                    result[y, x] = (byte)correctedValue;
                }
            }

            return result;
        }

        public static Bitmap CorrectImage(Bitmap source, Bitmap target)
        {
            // Канали з SOURCE
            byte[,] sourceR = GetRedChannel(source);
            byte[,] sourceG = GetGreenChannel(source);
            byte[,] sourceB = GetBlueChannel(source);

            // Отримуємо канали TARGET
            byte[,] targetR = GetRedChannel(target);
            byte[,] targetG = GetGreenChannel(target);
            byte[,] targetB = GetBlueChannel(target);

            // Виконуємо статистичну корекцію кожного каналу
            byte[,] resultR = CorrectChannel(sourceR, targetR);
            byte[,] resultG = CorrectChannel(sourceG, targetG);
            byte[,] resultB = CorrectChannel(sourceB, targetB);

            // Створюємо результуюче зображення
            Bitmap result = new Bitmap(target.Width, target.Height);

            for (int y = 0; y < target.Height; y++)
            {
                for (int x = 0; x < target.Width; x++)
                {
                    Color pixel = Color.FromArgb(
                        resultR[y, x],
                        resultG[y, x],
                        resultB[y, x]);

                    result.SetPixel(x, y, pixel);
                }
            }

            return result;
        }

        private static bool IsInsideRegion(int x, int y, Rectangle selectedRegion, bool circularSelection, 
            Point brushCenter, int brushRadius)
        {
            if (circularSelection)
            {
                double dx = x - brushCenter.X; 
                double dy = y - brushCenter.Y;

                return dx * dx + dy * dy
                        <= brushRadius * brushRadius;
            }

            return selectedRegion.Contains(x, y);
        }
        public static Bitmap CorrectImageLab(Bitmap source, 
            Bitmap target, bool correctL, bool correctA, bool correctB,
            Rectangle selectedRegion, bool circularSelection, 
            Point brushCenter, int brushRadius)
        {
            int width = target.Width;
            int height = target.Height;

            Bitmap result = new Bitmap(width, height);

            // Масиви для компонентів Lab
            double[,] sourceL = new double[height, width];
            double[,] sourceA = new double[height, width];
            double[,] sourceB = new double[height, width];

            double[,] targetL = new double[height, width];
            double[,] targetA = new double[height, width];
            double[,] targetB = new double[height, width];

            // Перетворюємо SOURCE і TARGET у Lab
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool insideRegion;
                    if (circularSelection)
                    {
                        double dx = x - brushCenter.X;
                        double dy = y - brushCenter.Y;

                        insideRegion =
                            dx * dx + dy * dy
                            <= brushRadius * brushRadius;
                    }
                    else
                    {
                        insideRegion = selectedRegion.Contains(x, y);
                    }

                    if (!insideRegion)
                        continue;

                    Color sourcePixel = source.GetPixel(x, y);
                    Color targetPixel = target.GetPixel(x, y);

                    double scale = 235.0 / 255.0;
                    double sr = Math.Max(sourcePixel.R / 255.0, 0.01176);
                    double sg = Math.Max(sourcePixel.G / 255.0, 0.01176);
                    double sb = Math.Max(sourcePixel.B / 255.0, 0.01176);

                    double tr = Math.Max(targetPixel.R / 255.0, 0.01176);
                    double tg = Math.Max(targetPixel.G / 255.0, 0.01176);
                    double tb = Math.Max(targetPixel.B / 255.0, 0.01176);

                    double[] sourceLms = ColorConverter.RGBtoLMS(sr, sg, sb);
                    double[] targetLms = ColorConverter.RGBtoLMS(tr, tg, tb);

                    double[] sourceLab = ColorConverter.LMStoLab(
                        sourceLms[0], sourceLms[1], sourceLms[2]);

                    double[] targetLab = ColorConverter.LMStoLab(
                        targetLms[0], targetLms[1], targetLms[2]);

                    sourceL[y, x] = sourceLab[0];
                    sourceA[y, x] = sourceLab[1];
                    sourceB[y, x] = sourceLab[2];

                    targetL[y, x] = targetLab[0];
                    targetA[y, x] = targetLab[1];
                    targetB[y, x] = targetLab[2];
                }
            }

            // Статистична корекція трьох компонентів Lab
            if (correctL)
                CorrectLabChannel(sourceL, targetL, selectedRegion, 
                    circularSelection, brushCenter, brushRadius);
            if (correctA)
                CorrectLabChannel(sourceA, targetA, selectedRegion,
                    circularSelection, brushCenter, brushRadius);
            if (correctB)
                CorrectLabChannel(sourceB, targetB, selectedRegion,
                    circularSelection, brushCenter, brushRadius);

            // Повертаємося Lab → LMS → RGB
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool insideRegion;
                    if (circularSelection)
                    {
                        double dx = x - brushCenter.X;
                        double dy = y - brushCenter.Y;

                        insideRegion =
                            dx * dx + dy * dy
                            <= brushRadius * brushRadius;
                    }
                    else
                    {
                        insideRegion = selectedRegion.Contains(x, y);
                    }
                    
                    if (!insideRegion)
                    {
                        result.SetPixel(x, y, target.GetPixel(x, y));
                        continue;
                    }

                    double[] lms = ColorConverter.LabToLMS(
                    targetL[y, x],
                    targetA[y, x],
                    targetB[y, x]);

                    double[] rgb = ColorConverter.LMSToRGB(
                        lms[0], lms[1], lms[2]);

                    int r = (int)Math.Round(Math.Max(0, Math.Min(1, rgb[0])) * 255);
                    int g = (int)Math.Round(Math.Max(0, Math.Min(1, rgb[1])) * 255);
                    int b = (int)Math.Round(Math.Max(0, Math.Min(1, rgb[2])) * 255);

                    result.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            return result;
        }

        private static void CorrectLabChannel(double[,] source, double[,] target,
            Rectangle selectedRegion, bool circularSelection,
            Point brushCenter, int brushRadius)
        {
            int height = target.GetLength(0);
            int width = target.GetLength(1);

            double sourceMean = CalculateMean(source, selectedRegion, circularSelection, brushCenter, brushRadius);
            double targetMean = CalculateMean(target, selectedRegion, circularSelection, brushCenter, brushRadius);

            double sourceVariance = CalculateVariance(source, selectedRegion, circularSelection, brushCenter, brushRadius);
            double targetVariance = CalculateVariance(target, selectedRegion, circularSelection, brushCenter, brushRadius);

            double sourceStd = Math.Sqrt(sourceVariance);
            double targetStd = Math.Sqrt(targetVariance);

            if (targetStd < 0.000001)
                targetStd = 1;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!IsInsideRegion(
                        x,
                        y,
                        selectedRegion,
                        circularSelection,
                        brushCenter,
                        brushRadius))
                    {
                        continue;
                    }

                    target[y, x] =
                        (target[y, x] - targetMean)
                        * (sourceStd / targetStd)
                        + sourceMean;
                }
            }
        }
    }
}