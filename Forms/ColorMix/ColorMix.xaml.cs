using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TheoryOfTelevision
{
    public class GraphPoint
    {
        private const double xPixMin = 45;
        private const double xPixMax = 454;
        private const double yPixMin = 29;
        private const double yPixMax = 490;

        public double valLeftPix;
        public double valTopPix;

        public double Xval;
        public double Yval;
        public double Zval;

        public double Rval;
        public double Gval;
        public double Bval;

        public GraphPoint()
        {
            SetPixel(0, 0);
        }

        public GraphPoint(double leftPix, double topPix)
        {
            SetPixel(leftPix, topPix);
        }

        public void SetPixel(double leftPix, double topPix)
        {
            this.valLeftPix = leftPix;
            this.valTopPix = topPix;

            this.Xval = ((valLeftPix / 409) * 0.8);
            this.Yval = (0.9 - (valTopPix / 461) * 0.9);
            this.Zval = (1 - Xval - Yval);

            ConvertionToRGB();
        }

        public bool SetXYZ(double X, double Y)
        {
            this.Xval = X;
            this.Yval = Y;
            this.Zval = 1 - X - Y;

            if (this.Zval < 0) return false;

            this.valLeftPix = ((this.Xval * (double)409) / 0.8) + xPixMin;
            this.valTopPix = (((0.9 - this.Yval) * (double)461) / 0.9) + yPixMin;

            ConvertionToRGB();
            return true;
        }

        public void SetRGB(double R, double G, double B)
        {
            this.Rval = R;
            this.Gval = G;
            this.Bval = B;

            ConvertionToXYZ();

            this.valLeftPix = ((this.Xval * (double)409) / 0.8) + xPixMin;
            this.valTopPix = (((0.9 - this.Yval) * (double)461) / 0.9) + yPixMin;
        }

        private void ConvertionToRGB()
        {
            double Rval;
            double Gval;
            double Bval;

            Rval = (Xval * (3.2406)) + (Yval * (-1.5372)) + (Zval * (-0.4986));
            Gval = (Xval * (-0.9689)) + (Yval * (1.8758)) + (Zval * (0.0415));
            Bval = (Xval * (0.0557)) + (Yval * (-0.2040)) + (Zval * (1.0570));

            if (Math.Abs(Rval) > 0.0031308) Rval = (1.055 * Math.Pow(Rval, (1 / 2.4))) - 0.055;
            else Rval = 12.92 * Rval;

            if (Math.Abs(Gval) > 0.0031308) Gval = (1.055 * Math.Pow(Gval, (1 / 2.4))) - 0.055;
            else Gval = 12.92 * Gval;

            if (Math.Abs(Bval) > 0.0031308) Bval = (1.055 * Math.Pow(Bval, (1 / 2.4))) - 0.055;
            else Bval = 12.92 * Bval;

            double sum = Rval + Gval + Bval;
            this.Rval = Rval * 255 / sum;
            this.Gval = Gval * 255 / sum;
            this.Bval = Bval * 255 / sum;

            this.Rval = Limitation(this.Rval, 0, 255);
            this.Gval = Limitation(this.Gval, 0, 255);
            this.Bval = Limitation(this.Bval, 0, 255);
        }

        private void ConvertionToXYZ()
        {
            double var_R;
            double var_G;
            double var_B;

            var_R = (this.Rval / 255);
            var_G = (this.Gval / 255);
            var_B = (this.Bval / 255);

            if (var_R > 0.04045) var_R = Math.Pow(((var_R + 0.055) / 1.055), 2.4);
            else var_R = var_R / 12.92;
            if (var_G > 0.04045) var_G = Math.Pow(((var_G + 0.055) / 1.055), 2.4);
            else var_G = var_G / 12.92;
            if (var_B > 0.04045) var_B = Math.Pow(((var_B + 0.055) / 1.055), 2.4);
            else var_B = var_B / 12.92;

            this.Xval = var_R * 0.4124 + var_G * 0.3576 + var_B * 0.1805;
            this.Yval = var_R * 0.2126 + var_G * 0.7152 + var_B * 0.0722;
            this.Zval = var_R * 0.0193 + var_G * 0.1192 + var_B * 0.9505;

            double sum = this.Xval + this.Yval + this.Zval;

            this.Xval /= sum;
            this.Yval /= sum;
            this.Zval /= sum;
        }

        private double Limitation(double value, double min, double max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }
    }

    public partial class ColorMix : Window
    {
        private const double xPixMin = 45;
        private const double xPixMax = 454;
        private const double yPixMin = 29;
        private const double yPixMax = 490;

        private GraphPoint E = new GraphPoint();
        private GraphPoint R = new GraphPoint();
        private GraphPoint G = new GraphPoint();
        private GraphPoint B = new GraphPoint();

        private GraphPoint P = new GraphPoint(0, 0);

        public ColorMix()
        {
            InitializeComponent();

            E.SetXYZ(0.33333, 0.33333);
            Canvas.SetLeft(pointE, Math.Round(E.valLeftPix, 3));
            Canvas.SetTop(pointE, Math.Round(E.valTopPix, 3));

            R.SetRGB(255, 0, 0);
            Canvas.SetLeft(pointR, Math.Round(R.valLeftPix, 3));
            Canvas.SetTop(pointR, Math.Round(R.valTopPix, 3));

            G.SetRGB(0, 255, 0);
            Canvas.SetLeft(pointG, Math.Round(G.valLeftPix, 3));
            Canvas.SetTop(pointG, Math.Round(G.valTopPix, 3));

            B.SetRGB(0, 0, 255);
            Canvas.SetLeft(pointB, Math.Round(B.valLeftPix, 3));
            Canvas.SetTop(pointB, Math.Round(B.valTopPix, 3));
        }

        private void RefreshAllData()
        {
            Xvalue.Content = "X: " + Convert.ToString(Math.Round(P.Xval, 2));
            Yvalue.Content = "Y: " + Convert.ToString(Math.Round(P.Yval, 2));
            Zvalue.Content = "Z: " + Convert.ToString(Math.Round(P.Zval, 2));

            Rvalue.Content = "R: " + Convert.ToString(Math.Round(P.Rval, 0));
            Gvalue.Content = "G: " + Convert.ToString(Math.Round(P.Gval, 0));
            Bvalue.Content = "B: " + Convert.ToString(Math.Round(P.Bval, 0));

            byte max = 0;

            if ((!Double.IsNaN(P.Rval)) && (!Double.IsNaN(P.Gval)) && (!Double.IsNaN(P.Bval)))
            {
                List<double> tmp = new List<double>();
                tmp.Add(P.Rval);
                tmp.Add(P.Gval);
                tmp.Add(P.Bval);
                max = Convert.ToByte(tmp.Max());
            }

            ColorBlock.Background = new SolidColorBrush(Color.FromArgb(255, (byte)Math.Round(P.Rval, 0), (byte)Math.Round(P.Gval, 0), (byte)Math.Round(P.Bval, 0))); 

            calcLineMonochrom();
        }

        private void calcLineMonochrom()
        {
            double dX = Canvas.GetLeft(Point_1) - Canvas.GetLeft(lineMonochrom) + 3;
            double dY = Canvas.GetTop(Point_1) - Canvas.GetTop(lineMonochrom) + 3;

            lineMonochrom.X2 = Canvas.GetLeft(Point_1) - Canvas.GetLeft(lineMonochrom) + 3;
            lineMonochrom.Y2 = Canvas.GetTop(Point_1) - Canvas.GetTop(lineMonochrom) + 3;

            waveLen.Content = Convert.ToString(Math.Round(getWaveLen(dX, dY), 0)) + " нм";

            dlineMonochrom.X2 = (Canvas.GetLeft(Point_1) - Canvas.GetLeft(lineMonochrom) + 3)* 1000;
            dlineMonochrom.Y2 = (Canvas.GetTop(Point_1) - Canvas.GetTop(lineMonochrom) + 3)* 1000;

            lineMonochrom.Visibility = Visibility.Visible;
            dlineMonochrom.Visibility = Visibility.Visible;
        }

        private double getWaveLen(double dX, double dY)
        {
            double K = dY / dX;

            if ((dX <= 0) && (dY >= 0))
            {
                if ((K >= (-2.12)) && (K < (-1.58)))
                    return LinFunc(-2.12, -1.58, 380, 460, K);

                if ((K >= (-1.58)) && (K < (-1.31)))
                    return LinFunc(-1.58, -1.31, 460, 470, K);                          //              ^
                                                                                        //              |
                if ((K >= (-1.31)) && (K < (-0.83)))                                    //      X-      |       X+
                    return LinFunc(-1.31, -0.83, 470, 480, K);                          //      Y-      |       Y-
                                                                                        //   -----------|------------->
                if ((K >= (-0.83)) && (K < (-0.13)))                                    //      X-      |       X+
                    return LinFunc(-0.83, -0.13, 480, 490, K);                          //      Y+      |       Y+
                                                                                        //              |
                if ((K >= (-0.13)) && (K < (0)))
                    return LinFunc(-0.13, 0, 490, 492, K);
            }

            if ((dX <= 0) && (dY <= 0))                                                               
            {
                if ((K > 10) || (Double.IsInfinity(K)))
                    K = 10;

                if ((K >= 0) && (K < 0.65))
                    return LinFunc(0, 0.65, 492, 500, K);

                if ((K >= 0.65) && (K < 1.91))
                    return LinFunc(0.65, 1.91, 500, 520, K);

                if ((K >= 1.91) && (K < 4.03))
                    return LinFunc(1.91, 4.03, 520, 540, K);

                if ((K >= 4.03) && (K <= 10))
                    if (LinFunc(4.03, 10, 540, 554, K) == 0)
                        return 554;
                    else
                        return LinFunc(4.03, 10, 540, 554, K);
            }                                                                                       
                     
            if ((dX >= 0) && (dY <= 0))                                                              
            {
                if ((K < -100) || (Double.IsInfinity(K))) 
                    K = -100;

                if ((K >= (-100)) && (K < (-7.73)))
                    if (LinFunc(-100, -7.73, 554, 560, K) == 0)
                        return 554;
                    else
                        return LinFunc(-100, -7.73, 554, 560, K);

                if ((K >= (-7.73)) && (K < (-0.86)))
                    return LinFunc(-7.73, -0.86, 560, 580, K);

                if ((K >= (-0.86)) && (K < (-0.13)))
                    return LinFunc(-0.86, -0.13, 580, 600, K);

                if ((K >= (-0.13)) && (K < 0))
                    return LinFunc(-0.13, 0, 600, 613, K);
            }

            if ((dX >= 0) && (dY >= 0))
            {
                if ((K >= 0) && (K < 0.07))
                    return LinFunc(0, 0.07, 613, 620, K);

                if ((K >= 0.07) && (K < 0.18))
                    return LinFunc(0.07, 0.18, 620, 700, K);
            }

            return 0;
        }

        private double LinFunc(double k0, double k1, double wave0, double wave1, double K)
        {
            double step = (k1 - k0) / (wave1 - wave0);
            return (wave0 + ((K - k0) / step));
        }

        private void Point_1_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            
            UIElement thumb = e.Source as UIElement;

            Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) + e.HorizontalChange);
            Canvas.SetTop(thumb, Canvas.GetTop(thumb) + e.VerticalChange);

            if (Canvas.GetLeft(thumb) < xPixMin) Canvas.SetLeft(thumb, xPixMin);
            if (Canvas.GetLeft(thumb) > xPixMax) Canvas.SetLeft(thumb, xPixMax);
            if (Canvas.GetTop(thumb) < yPixMin) Canvas.SetTop(thumb, yPixMin);
            if (Canvas.GetTop(thumb) > yPixMax) Canvas.SetTop(thumb, yPixMax);

            double XValPix = Canvas.GetLeft(Point_1) - Canvas.GetLeft(HorizontalLine) + 3;
            double YValPix = Canvas.GetTop(Point_1) - Canvas.GetTop(HorizontalLine) + 3;

            HorizontalLine.X1 = 0;
            VerticalLine.Y2 = 460;
            HorizontalLine.X2 = VerticalLine.X1 = VerticalLine.X2 = XValPix;
            HorizontalLine.Y1 = HorizontalLine.Y2 = VerticalLine.Y1 = YValPix;

            P.SetPixel(XValPix, YValPix);
            RefreshAllData();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            double step = 0.5;

            if (e.Key == Key.Up)
            {
                Canvas.SetTop(Point_1, Canvas.GetTop(Point_1) - step);
            }

            if (e.Key == Key.Down)
            {
                Canvas.SetTop(Point_1, Canvas.GetTop(Point_1) + step);
            }

            if (e.Key == Key.Left)
            {
                Canvas.SetLeft(Point_1, Canvas.GetLeft(Point_1) - step);
            }

            if (e.Key == Key.Right)
            {
                Canvas.SetLeft(Point_1, Canvas.GetLeft(Point_1) + step);
            }


            if (Canvas.GetLeft(Point_1) < xPixMin) Canvas.SetLeft(Point_1, xPixMin);
            if (Canvas.GetLeft(Point_1) > xPixMax) Canvas.SetLeft(Point_1, xPixMax);
            if (Canvas.GetTop(Point_1) < yPixMin) Canvas.SetTop(Point_1, yPixMin);
            if (Canvas.GetTop(Point_1) > yPixMax) Canvas.SetTop(Point_1, yPixMax);

            double XValPix = Canvas.GetLeft(Point_1) - Canvas.GetLeft(HorizontalLine) + 3;
            double YValPix = Canvas.GetTop(Point_1) - Canvas.GetTop(HorizontalLine) + 3;

            HorizontalLine.X1 = 0;
            VerticalLine.Y2 = 460;
            HorizontalLine.X2 = VerticalLine.X1 = VerticalLine.X2 = XValPix;
            HorizontalLine.Y1 = HorizontalLine.Y2 = VerticalLine.Y1 = YValPix;

            P.SetPixel(XValPix, YValPix);

            RefreshAllData();
        }

        private void Canv_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point tmp = Mouse.GetPosition(Canv);

            if((tmp.X > xPixMin) && (tmp.X < xPixMax) && (tmp.Y > yPixMin) && (tmp.Y < yPixMax))
            {
                Canvas.SetLeft(Point_1, tmp.X - 3);
                Canvas.SetTop(Point_1, tmp.Y - 3);
            }

            double XValPix = Canvas.GetLeft(Point_1) - Canvas.GetLeft(HorizontalLine) + 3;
            double YValPix = Canvas.GetTop(Point_1) - Canvas.GetTop(HorizontalLine) + 3;

            HorizontalLine.X1 = 0;
            VerticalLine.Y2 = 460;
            HorizontalLine.X2 = VerticalLine.X1 = VerticalLine.X2 = XValPix;
            HorizontalLine.Y1 = HorizontalLine.Y2 = VerticalLine.Y1 = YValPix;

            P.SetPixel(XValPix, YValPix);

            RefreshAllData();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Действительно хотите выйти?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Application.Current.MainWindow.Show();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
