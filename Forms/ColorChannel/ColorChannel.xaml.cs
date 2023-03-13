using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using System.Numerics;
using System.Drawing;
using System.Collections.ObjectModel;
using Microsoft.Win32;

using TheoryOfTelevision.Common;

namespace TheoryOfTelevision
{
    public partial class ColorChannel : Window
    {
        private int imgWidth;
        private int imgHight;
        private int selectedLine = 0;
        private double Period = 1;
        private BitmapImage loadedImg;
        private Bitmap openImg;

        private Color color_plotWB = Color.Black;

        private List<Complex> ArrSpectr = new List<Complex>();

        private List<double> ArrSpectrAH = new List<double>();
        private List<double> ArrSpectrFH = new List<double>();
        private List<double> ColorValueOfLine = new List<double>();

        public ColorChannel()
        {
            InitializeComponent();
        }

        private void ShowPlot()
        {
            List<double> ColorValueOfLineR = new List<double>();
            List<double> ColorValueOfLineG = new List<double>();
            List<double> ColorValueOfLineB = new List<double>();

            ColorValueOfLineR = Calculation.GetREDofLine(openImg, selectedLine);
            ColorValueOfLineG = Calculation.GetGREENofLine(openImg, selectedLine);
            ColorValueOfLineB = Calculation.GetBLUEofLine(openImg, selectedLine);

            SelectColor();

            var tempSeries = new List<double>();
            var tempSeriesR = new List<double>();
            var tempSeriesG = new List<double>();
            var tempSeriesB = new List<double>();

            tempSeries.Clear();
            tempSeriesR.Clear();
            tempSeriesG.Clear();
            tempSeriesB.Clear();

            for (int i = 0; i < imgWidth; i++)
            {
                tempSeries.Add(ColorValueOfLine[i]);
            }
            if (imgWidth == 0)
                tempSeries.Add(0.0);

            PlotWB.plt.Clear();
            PlotWB.plt.PlotSignal(tempSeries.ToArray(), (imgWidth / Period) == 0 ? 1 : (imgWidth / Period), 0, 0, color_plotWB, 2);
            PlotWB.plt.AxisAuto(0);
            PlotWB.Configure(false, false, false, false);
            PlotWB.Render();


            for (int i = 0; i < imgWidth; i++)
            {
                tempSeriesR.Add(ColorValueOfLineR[i]);
            }
            if (imgWidth == 0)
                tempSeriesR.Add(0.0);

            PlotRED.plt.Clear();
            PlotRED.plt.PlotSignal(tempSeriesR.ToArray(), (imgWidth / Period) == 0 ? 1 : (imgWidth / Period), 0, 0, Color.Red, 2, label: "Красный");
            PlotRED.plt.AxisAuto(0);
            PlotRED.Configure(false, false, false, false);
            PlotRED.Render();


            for (int i = 0; i < imgWidth; i++)
            {
                tempSeriesG.Add(ColorValueOfLineG[i]);
            }
            if (imgWidth == 0) tempSeriesG.Add(0.0);

            PlotGREEN.plt.Clear();
            PlotGREEN.plt.PlotSignal(tempSeriesG.ToArray(), (imgWidth / Period) == 0 ? 1 : (imgWidth / Period), 0, 0, Color.Green, 2);
            PlotGREEN.plt.AxisAuto(0);
            PlotGREEN.Configure(false, false, false, false);
            PlotGREEN.Render();


            for (int i = 0; i < imgWidth; i++)
            {
                tempSeriesB.Add(ColorValueOfLineB[i]);
            }
            if (imgWidth == 0) tempSeriesB.Add(0.0);

            PlotBLUE.plt.Clear();
            PlotBLUE.plt.PlotSignal(tempSeriesB.ToArray(), (imgWidth / Period) == 0 ? 1 : (imgWidth / Period), 0, 0, Color.Blue, 2);
            PlotBLUE.plt.AxisAuto(0);
            PlotBLUE.Configure(false, false, false, false);
            PlotBLUE.Render();

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

            //SelectColor();
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

            //SelectColor();
            ShowPlot();

            ButWBLoadStand_Click(ButWBLoadStand, new RoutedEventArgs());
        }

        private void SelectColor()
        {
            if (Colorbox != null)
            {
                if (Colorbox.SelectedIndex == 0) ColorValueOfLine = Calculation.GetREDofLine(openImg, selectedLine);
                if (Colorbox.SelectedIndex == 1) ColorValueOfLine = Calculation.GetGREENofLine(openImg, selectedLine);
                if (Colorbox.SelectedIndex == 2) ColorValueOfLine = Calculation.GetBLUEofLine(openImg, selectedLine);
            }
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

            PlotWB.plt.Clear();
            PlotSpectrAH.plt.Clear();
            PlotSpectrFH.plt.Clear();
            PlotRED.plt.Clear();
            PlotGREEN.plt.Clear();
            PlotBLUE.plt.Clear();

            PlotWB.Render();
            PlotSpectrAH.Render();
            PlotSpectrFH.Render();
            PlotRED.Render();
            PlotGREEN.Render();
            PlotBLUE.Render();
        }

        private void ButWBLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openImageDialog = new OpenFileDialog();

            openImageDialog.Filter = "Image files|*.jpg;*.png;";
            openImageDialog.FilterIndex = 1;

            if (openImageDialog.ShowDialog() == true)
            {
                OpenImg(openImageDialog.FileName);
                //SelectColor();
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

            SelectColor();
            ArrSpectr.AddRange(Calculation.DFT(ColorValueOfLine));

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

            for (int i = (-imgWidth / 2), j = 0; i < (imgWidth / 2); i++, j++)
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
            if (PlotWB.Visibility == Visibility.Visible)
            {
                PlotWB.Visibility = Visibility.Hidden;
                ScrollViewerStandartImg.Visibility = Visibility.Visible;
                ButWBLoadStand.Content = "Развёртка";
                ButWBLoadStand.ToolTip = "Отобразить временную развёртку";
            }
            else
            {
                PlotWB.Visibility = Visibility.Visible;
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

            if (ColorValueOfLine != null)
                ColorValueOfLine.Clear();

            //SelectColor();
            ShowPlot();
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

                if (ColorValueOfLine != null)
                    ColorValueOfLine.Clear();
                //SelectColor();
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
            else if (Convert.ToInt32(textBoxPeriod.Text) > 999)
                textBoxPeriod.Text = "999";

            Period = (double)Convert.ToInt32(textBoxPeriod.Text) / 1000000;
            ShowPlot();
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

        private void Colorbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (Colorbox.SelectedIndex)
            {
                case 0: color_plotWB = Color.Red; break;
                case 1: color_plotWB = Color.Green; break;
                case 2: color_plotWB = Color.Blue; break;
                default: break;
            }
            ShowPlot();
        }

        private void loadedImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point clk = Mouse.GetPosition(loadedImage);

            selectedLine = Convert.ToInt32((clk.Y * imgHight) / loadedImage.ActualHeight);

            SliderWB.Value = -selectedLine;
            textBoxnbrLineWB.Text = Convert.ToString(Math.Abs(SliderWB.Value));

            if (ColorValueOfLine != null)
                ColorValueOfLine.Clear();

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
    }
}
