using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Numerics;
using System.Drawing;
using Microsoft.Win32;
using TheoryOfTelevision.Common;

namespace TheoryOfTelevision
{
    public partial class LuminanceChannel : Window
    {
        private int imgWidth;
        private int imgHight;
        private int selectedLine = 0;
        private double Period = 1.0;
        private BitmapImage loadedImg;
        private Bitmap openImg;

        private List<Complex> ArrSpectr = new List<Complex>();

        private List<double> ArrSpectrAH = new List<double>();
        private List<double> ArrSpectrFH = new List<double>();
        private List<double> BrightnessValueOfLine = new List<double>();

        public LuminanceChannel()
        {
            InitializeComponent();
        }

        private void ShowPlot()
        {
            var tempSeries = new List<double>();

            tempSeries.Clear();

            for (int i = 0; i < imgWidth; i++)
            {
                tempSeries.Add(BrightnessValueOfLine[i]);
            }

            if(imgWidth == 0)
                tempSeries.Add(0.0);

            TimePlotWB.plt.Clear();
            TimePlotWB.plt.PlotSignal(tempSeries.ToArray(), (imgWidth/Period)==0 ? 1 : (imgWidth / Period), 0, 0, Color.Black, 2);
            TimePlotWB.plt.AxisAuto(0);
            TimePlotWB.Configure(false, false, false, false);
            TimePlotWB.Render();

            Canvas.SetTop(lineSelect, (selectedLine * loadedImage.ActualHeight) / imgHight);
        }

        private void OpenImg(string link)
        {
            loadedImg = new BitmapImage(new Uri(link));
            loadedImage.Source = loadedImg;

            openImg = new Bitmap(@link);

            imgWidth = openImg.Width;
            imgHight = openImg.Height - 1;

            allNbrLine.Content = "(0..." + Convert.ToString(imgHight) + ")";
            SliderWB.Minimum = -imgHight;
            SliderWB.Maximum = 0;

            textBoxnbrLineWB.Text = Convert.ToString(Math.Abs(SliderWB.Value));
            ButCalcDFT.IsEnabled = true;
            textBoxnbrLineWB.IsEnabled = true;

            BrightnessValueOfLine = Calculation.GetBrightnessOfLine(openImg, selectedLine);
            ShowPlot();

            ButWBLoadStand_Click(ButWBLoadStand, new RoutedEventArgs());
        }

        private void OpenImg(string link, Bitmap img)
        {
            loadedImg = new BitmapImage(new Uri("pack://Application:,,,/" + link));
            loadedImage.Source = loadedImg;

            openImg = img;

            imgWidth = openImg.Width;
            imgHight = openImg.Height - 1;

            allNbrLine.Content = "(0..." + Convert.ToString(imgHight) + ")";
            SliderWB.Minimum = -imgHight;
            SliderWB.Maximum = 0;

            textBoxnbrLineWB.Text = Convert.ToString(Math.Abs(SliderWB.Value));
            ButCalcDFT.IsEnabled = true;
            textBoxnbrLineWB.IsEnabled = true;

            BrightnessValueOfLine = Calculation.GetBrightnessOfLine(openImg, selectedLine);
            ShowPlot();

            ButWBLoadStand_Click(ButWBLoadStand, new RoutedEventArgs());
        }

        private void ButWBClear_Click(object sender, RoutedEventArgs e)
        {
            loadedImage.Source = null;
            allNbrLine.Content = "none";
            textBoxnbrLineWB.Text = "0";
            openImg = null;
            loadedImg = null;
            imgWidth = 0;
            imgHight = 0;
            selectedLine = 0;

            ButCalcDFT.IsEnabled = false;
            textBoxnbrLineWB.Text = null;
            textBoxnbrLineWB.IsEnabled = false;

            TimePlotWB.plt.Clear();
            PlotSpectrAH.plt.Clear();
            PlotSpectrFH.plt.Clear();
            TimePlotWB.Render();
            PlotSpectrAH.Render();
            PlotSpectrFH.Render();
        }

        private void ButWBLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openImageDialog = new OpenFileDialog();

            openImageDialog.Filter = "Image files|*.jpg;*.png;";
            openImageDialog.FilterIndex = 1;

            if (openImageDialog.ShowDialog() == true)
            {
                OpenImg(openImageDialog.FileName);
                BrightnessValueOfLine = Calculation.GetBrightnessOfLine(openImg, selectedLine);
                ShowPlot();
            }
        }

        private void ButCalcDFT_Click(object sender, RoutedEventArgs e)
        {
            var tempXvalue = new List<double>();
            double maxAH = 0;

            ArrSpectr.Clear();
            ArrSpectrAH.Clear();
            ArrSpectrFH.Clear();

            BrightnessValueOfLine = Calculation.GetBrightnessOfLine(openImg, selectedLine);
            ArrSpectr.AddRange(Calculation.DFT(BrightnessValueOfLine));

            double temp = imgWidth / (2 * Period);

            for (int i = 0; i < imgWidth; i++)
            {
                ArrSpectrAH.Add(Complex.Abs(ArrSpectr[i]));

                if (maxAH < ArrSpectrAH[i])
                    maxAH = ArrSpectrAH[i];
            }

            for (int i = 0; i < imgWidth; i++)
            {
                ArrSpectrFH.Add(ArrSpectr[i].Phase);
            }

            ArrSpectrAH = Calculation.ShiftCenter(ArrSpectrAH);

            for (int i = (-imgWidth/2), j = 0; i < (imgWidth/2); i++, j++)
            {
                tempXvalue.Add(i / Period);
            }

            PlotSpectrAH.plt.Clear();
            PlotSpectrFH.plt.Clear();
            PlotSpectrAH.plt.Title("АЧХ", true);
            PlotSpectrFH.plt.Title("ФЧХ", true);
            PlotSpectrAH.plt.PlotBar(tempXvalue.ToArray(), ArrSpectrAH.ToArray(), null, "АЧХ", 1);
            PlotSpectrFH.plt.PlotBar(tempXvalue.ToArray(), ArrSpectrFH.ToArray(), null, "ФЧХ", 1);
            PlotSpectrAH.plt.AxisBounds(-temp, temp, -0.01, 1.1 * maxAH);
            PlotSpectrFH.plt.AxisBounds(-temp, temp, -3.2, 3.2);
            PlotSpectrAH.Configure(null, null, null, null, null, null, false, true, false);
            PlotSpectrFH.Configure(null, null, null, null, null, null, false, true, false);
            PlotSpectrAH.Render();
            PlotSpectrFH.Render();

            TabItemSpectr.IsSelected = true;
        }

        private void ButWBLoadStand_Click(object sender, RoutedEventArgs e)
        {
            if (TimePlotWB.Visibility == Visibility.Visible)
            {
                TimePlotWB.Visibility = Visibility.Hidden;
                ScrollViewerStandartImg.Visibility = Visibility.Visible;
                ButWBLoadStand.Content = "Развёртка";
                ButWBLoadStand.ToolTip = "Отобразить временную развёртку";
            }
            else
            {
                TimePlotWB.Visibility = Visibility.Visible;
                ScrollViewerStandartImg.Visibility = Visibility.Hidden;
                ButWBLoadStand.Content = "Шаблоны";
                ButWBLoadStand.ToolTip = "Отобразить набор стандартных изображений";
            }
            TabItemScan.IsSelected = true;
        }

        private void SliderWB_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            selectedLine = Math.Abs((int)SliderWB.Value);
            textBoxnbrLineWB.Text = Convert.ToString(Math.Abs(SliderWB.Value));

            if (BrightnessValueOfLine != null)
                BrightnessValueOfLine.Clear();

            BrightnessValueOfLine = Calculation.GetBrightnessOfLine(openImg, selectedLine);

            ShowPlot();
        }

        private void PlotSpectrAH_AxisChanged(object sender, EventArgs e)
        {
            var temp = PlotSpectrAH.plt.Axis();
            PlotSpectrFH.plt.Axis(temp[0], temp[1]);
            PlotSpectrFH.Render();
        }

        private void PlotSpectrFH_AxisChanged(object sender, EventArgs e)
        {
            var temp = PlotSpectrFH.plt.Axis();
            PlotSpectrAH.plt.Axis(temp[0], temp[1]);
            PlotSpectrAH.Render();
        }

        private void textBoxnbrLineWB_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (InputChecker.CheckForEditableDigitField(e.Key))
                e.Handled = false;
            else 
                e.Handled = true;
        }

        private void textBoxnbrLineWB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxnbrLineWB.Text != "")
            {
                if (Convert.ToInt32(textBoxnbrLineWB.Text) <= imgHight)
                {
                    SliderWB.Value = -Convert.ToDouble(textBoxnbrLineWB.Text);
                }
                else
                {
                    textBoxnbrLineWB.Text = Convert.ToString(imgHight);
                    SliderWB.Value = -imgHight;
                }
                selectedLine = Math.Abs((int)SliderWB.Value);

                if (BrightnessValueOfLine != null)
                    BrightnessValueOfLine.Clear();

                BrightnessValueOfLine = Calculation.GetBrightnessOfLine(openImg, selectedLine);

                ShowPlot();
            }
        }

        private void textBoxPeriod_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (InputChecker.CheckForEditableDigitField(e.Key))
                e.Handled = false;
            else 
                e.Handled = true;
        }

        private void textBoxPeriod_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxPeriod.Text == "")
                textBoxPeriod.Text = "1";
            else if(Convert.ToInt32(textBoxPeriod.Text) > 999)
                textBoxPeriod.Text = "999";

            Period = (double)Convert.ToInt32(textBoxPeriod.Text) / 1000000.0;
            ShowPlot();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Действительно хотите выйти?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Application.Current.MainWindow.Show();
            else
                e.Cancel = true;
        }

        private void StdImg1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Загрузить изображение?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                OpenImg("StandartImages/1.png", Properties.Resources._1);
            }
        }

        private void StdImg2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Загрузить изображение?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                OpenImg("StandartImages/2.png", Properties.Resources._2);
            }
        }

        private void StdImg3_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Загрузить изображение?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                OpenImg("StandartImages/3.png", Properties.Resources._3);
            }
        }

        private void StdImg4_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Загрузить изображение?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                OpenImg("StandartImages/4.png", Properties.Resources._4);
            }
        }

        private void StdImg5_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Загрузить изображение?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                OpenImg("StandartImages/5.png", Properties.Resources._5);
            }
        }

        private void loadedImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point clk = Mouse.GetPosition(loadedImage);

            selectedLine = Convert.ToInt32((clk.Y * imgHight) / loadedImage.ActualHeight);

            SliderWB.Value = -selectedLine;
            textBoxnbrLineWB.Text = Convert.ToString(Math.Abs(SliderWB.Value));

            if (BrightnessValueOfLine != null)
                BrightnessValueOfLine.Clear();

            BrightnessValueOfLine = Calculation.GetBrightnessOfLine(openImg, selectedLine);

            ShowPlot();
        }
    }
}
