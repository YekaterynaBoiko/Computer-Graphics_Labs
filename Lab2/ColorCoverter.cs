using System;
namespace Lab2
{
    public class ColorConverter
    {
        public static double[] RGBtoLMS(double r, double g, double b)
        {
            double l = 0.3811 * r
                + 0.5783 * g
                + 0.0405 * b;

            double m = 0.1967 * r
                + 0.7244 * g
                + 0.0782 * b;

            double s = 0.0241 * r
                + 0.1288 * g
                + 0.8444 * b;
            return new double[] { l, m, s };
        }

        public static double[] LMStoLab(double l, double m, double s)
        {
            l = Math.Max(l, 0.01176);
            m = Math.Max(m, 0.01176);
            s = Math.Max(s, 0.01176);

            double logL = Math.Log10(l);
            double logM = Math.Log10(m);
            double logS = Math.Log10(s);

            double L = (logL + logM + logS) / Math.Sqrt(3);
            double A = (logL + logM - 2 * logS) / Math.Sqrt(6);
            double B = (logL - logM) / Math.Sqrt(2);

            return new double[] { L, A, B };
        }

        public static double[] LabToLMS(double L, double A, double B)
        {
            double logL =
                L / Math.Sqrt(3)
                + A / Math.Sqrt(6)
                + B / Math.Sqrt(2);

            double logM =
                L / Math.Sqrt(3)
                + A / Math.Sqrt(6)
                - B / Math.Sqrt(2);

            double logS =
                L / Math.Sqrt(3)
                - 2 * A / Math.Sqrt(6);

            double l = Math.Pow(10, logL);
            double m = Math.Pow(10, logM);
            double s = Math.Pow(10, logS);

            return new double[] { l, m, s };
        }

        public static double[] LMSToRGB(double l, double m, double s)
        {
            double r =
                4.4679 * l
                - 3.5873 * m
                + 0.1193 * s;

            double g =
                -1.2186 * l
                + 2.3809 * m
                - 0.1624 * s;

            double b =
                0.0497 * l
                - 0.2439 * m
                + 1.2045 * s;

            return new double[] { r, g, b };
        }

        public static double[] RGBtoHSL(double r, double g, double b)
        {
            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            double h = 0;
            double s = 0;
            double l = (max + min) / 2;

            if (max != min)
            {
                double d = max - min;

                s = l > 0.5
                    ? d / (2 - max - min)
                    : d / (max + min);

                if (max == r)
                {
                    h = (g - b) / d;

                    if (g < b)
                        h += 6;
                }
                else if (max == g)
                {
                    h = (b - r) / d + 2;
                }
                else
                {
                    h = (r - g) / d + 4;
                }

                h /= 6;
            }

            return new double[] { h, s, l };

        }

        public static double[] RGBtoHSV(double r, double g, double b)
        {
            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            double h = 0;
            double s = 0;
            double v = max;

            double d = max - min;

            if (max != 0)
                s = d / max;

            if (d != 0)
            {
                if (max == r)
                {
                    h = (g - b) / d;

                    if (h < 0)
                        h += 6;
                }
                else if (max == g)
                {
                    h = (b - r) / d + 2;
                }
                else
                {
                    h = (r - g) / d + 4;
                }

                h /= 6;
            }
            return new double[] { h, s, v };
        }

        public static double[] RGBtoYIQ(double r, double g, double b)
        {
            double y = 0.299 * r + 0.587 * g + 0.114 * b;
            double i = 0.596 * r
                        - 0.274 * g
                        - 0.322 * b;

            double q = 0.211 * r
                     - 0.523 * g
                     + 0.312 * b;

            return new double[] { y, i, q };
        }
    }
}