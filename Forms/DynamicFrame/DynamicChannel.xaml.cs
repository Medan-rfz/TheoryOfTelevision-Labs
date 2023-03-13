using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Numerics;
using System.Collections.ObjectModel;
using OxyPlot;
using TheoryOfTelevision.Common;

namespace TheoryOfTelevision
{


    public partial class DynamicChannel : Window
    {
        public int imgWidth = 800;
        public double Period = 1;

        public DynamicChannel()
        {
            InitializeComponent();

            LineZeroPos.ItemsSource = getLineSeries(10);
            LineShiftPos.ItemsSource = getLineSeries(10);
        }

        public Collection<DataPoint> getLineSeries(int nbrStart)
        {
            Collection<DataPoint> res = new Collection<DataPoint>();

            res.Clear();

            int k = 0;

            for (int i = 0; i < nbrStart; i++, k++)
            {
                res.Add(new DataPoint(((k * Period) / 800), 0));
            }

            for (int i = 0; i < 100; i++, k++)
            {
                res.Add(new DataPoint(((k * Period) / 800), 1));
            }

            for (int i = 0; i < (700 - nbrStart); i++, k++)
            {
                res.Add(new DataPoint(((k * Period) / 800), 0));
            }

            return res;
        }

        public List<double> getDoubleLineSeries(int nbrStart)
        {
            List<double> res = new List<double>();

            res.Clear();

            int k = 0;

            for (int i = 0; i < nbrStart; i++, k++)
            {
                res.Add(0);
            }

            for (int i = 0; i < 100; i++, k++)
            {
                res.Add(1);
            }

            for (int i = 0; i < (700 - nbrStart); i++, k++)
            {
                res.Add(0);
            }

            return res;
        }

        public void allPlotRefresh()
        {
            if (XAHZeroPos == null) return;

            Collection<DataPoint> seriesAHZeroPos = new Collection<DataPoint>();
            Collection<DataPoint> seriesAHShiftPos = new Collection<DataPoint>();
            Collection<DataPoint> seriesAHDoublePos = new Collection<DataPoint>();

            Collection<DataPoint> seriesFHZeroPos = new Collection<DataPoint>();
            Collection<DataPoint> seriesFHShiftPos = new Collection<DataPoint>();
            Collection<DataPoint> seriesFHDoublePos = new Collection<DataPoint>();

            List<Complex> spectrZeroPos = new List<Complex>();
            List<Complex> spectrShiftPos = new List<Complex>();
            List<Complex> spectrDoublePos = new List<Complex>();

            spectrZeroPos.Clear();
            spectrShiftPos.Clear();
            spectrDoublePos.Clear();

            List<double> AHZeroPos = new List<double>();
            List<double> AHShiftPos = new List<double>();
            List<double> AHDoublePos = new List<double>();

            List<double> FHZeroPos = new List<double>();
            List<double> FHShiftPos = new List<double>();
            List<double> FHDoublePos = new List<double>();

            AHZeroPos.Clear(); AHShiftPos.Clear(); AHDoublePos.Clear();
            FHZeroPos.Clear(); FHShiftPos.Clear(); FHDoublePos.Clear();

            List<double> tmpDoublePos = new List<double>();

            tmpDoublePos.Clear();

            tmpDoublePos.AddRange(getDoubleLineSeries(10));
            tmpDoublePos.AddRange(getDoubleLineSeries((int)(sliderShift.Value - 70)));

            spectrZeroPos.AddRange(Calculation.DFT(getDoubleLineSeries(10)));
            spectrShiftPos.AddRange(Calculation.DFT(getDoubleLineSeries((int)(sliderShift.Value - 70))));

            for(int i = 0; i < imgWidth; i++)
            {
                spectrDoublePos.Add(Complex.Add(spectrZeroPos[i], spectrShiftPos[i]));
            }


            for (int i = 0; i < imgWidth; i++)
            {
                AHZeroPos.Add(Complex.Abs(spectrZeroPos[i]));
                AHShiftPos.Add(Complex.Abs(spectrShiftPos[i]));
            }

            for (int i = 0; i < (imgWidth); i++)
            {
                AHDoublePos.Add(Complex.Abs(spectrDoublePos[i]));
            }

            for (int i = 0; i < imgWidth; i++)
            {
                FHZeroPos.Add(spectrZeroPos[i].Phase);
                FHShiftPos.Add(spectrShiftPos[i].Phase);
            }

            for (int i = 0; i < (imgWidth); i++)
            {
                FHDoublePos.Add(spectrDoublePos[i].Phase);
            }

            AHZeroPos = Calculation.ShiftCenter(AHZeroPos);
            AHShiftPos = Calculation.ShiftCenter(AHShiftPos);
            AHDoublePos = Calculation.ShiftCenter(AHDoublePos);

            FHZeroPos = Calculation.ShiftCenter(FHZeroPos);
            FHShiftPos = Calculation.ShiftCenter(FHShiftPos);
            FHDoublePos = Calculation.ShiftCenter(FHDoublePos);

            seriesAHZeroPos.Clear(); seriesAHShiftPos.Clear(); seriesAHDoublePos.Clear();
            seriesFHZeroPos.Clear(); seriesFHShiftPos.Clear(); seriesFHDoublePos.Clear();

            for (int i = (-imgWidth / 2), j = 0; i < (imgWidth / 2); i++, j++)
            {
                seriesAHZeroPos.Add(new DataPoint((i / Period), AHZeroPos[j]));
                seriesAHShiftPos.Add(new DataPoint((i / Period), AHShiftPos[j]));

                seriesFHZeroPos.Add(new DataPoint((i / Period), FHZeroPos[j]));
                seriesFHShiftPos.Add(new DataPoint((i / Period), FHShiftPos[j]));
            }

            for (int i = (-imgWidth / 2), j = 0; i < (imgWidth / 2); i++, j++)
            {
                seriesAHDoublePos.Add(new DataPoint((i / Period), AHDoublePos[j]));

                seriesFHDoublePos.Add(new DataPoint((i / Period), FHDoublePos[j]));
            }

            LineAHZeroPos.ItemsSource = seriesAHZeroPos;
            LineAHShiftPos.ItemsSource = seriesAHShiftPos;
            LineAHTwoPos.ItemsSource = seriesAHDoublePos;

            LineFHZeroPos.ItemsSource = seriesFHZeroPos;
            LineFHShiftPos.ItemsSource = seriesFHShiftPos;
            LineFHTwoPos.ItemsSource = seriesFHDoublePos;


            double tempRange = (double)imgWidth / (2 * Period);

            XAHZeroPos.AbsoluteMinimum = XAHZeroPos.Minimum = XFHZeroPos.AbsoluteMinimum = XFHZeroPos.Minimum = -tempRange;
            XAHZeroPos.AbsoluteMaximum = XAHZeroPos.Maximum = XFHZeroPos.AbsoluteMaximum = XFHZeroPos.Maximum = tempRange;

            XAHShiftPos.AbsoluteMinimum = XAHShiftPos.Minimum = XFHShiftPos.AbsoluteMinimum = XFHShiftPos.Minimum = -tempRange;
            XAHShiftPos.AbsoluteMaximum = XAHShiftPos.Maximum = XFHShiftPos.AbsoluteMaximum = XFHShiftPos.Maximum = tempRange;

            XAHTwoPos.AbsoluteMinimum = XAHTwoPos.Minimum = XFHTwoPos.AbsoluteMinimum = XFHTwoPos.Minimum = -(tempRange);
            XAHTwoPos.AbsoluteMaximum = XAHTwoPos.Maximum = XFHTwoPos.AbsoluteMaximum = XFHTwoPos.Maximum = tempRange;

            YAHZeroPos.Minimum = YAHShiftPos.Minimum = YAHTwoPos.Minimum = 0;
            YAHZeroPos.Maximum = YAHShiftPos.Maximum = AHZeroPos.Max() + (0.1 * AHZeroPos.Max());
            YAHTwoPos.Maximum = AHDoublePos.Max() + (0.1 * AHDoublePos.Max());

            YFHZeroPos.Minimum = YFHShiftPos.Minimum = YFHTwoPos.Minimum = -3.141592;
            YFHZeroPos.Maximum = YFHShiftPos.Maximum = YFHTwoPos.Maximum = 3.141592;
        }

        public void shiftValueRefresh()
        {
            double steps = sliderShift.Value - 80;
            double costStep = (Period * 1000000) / Width;

            string res = Convert.ToString(Math.Round(steps * costStep, 3)) + " мкс"; ;

            labelShift.Content = res;
        }

        private void sliderShift_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (indTrueSpectr == null) return;

            LineShiftPos.ItemsSource = getLineSeries((int)(sliderShift.Value - 70));

            shiftValueRefresh();

            indTrueSpectr.Fill = System.Windows.Media.Brushes.Red;
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

            LineZeroPos.ItemsSource = getLineSeries(10);
            LineShiftPos.ItemsSource = getLineSeries((int)(sliderShift.Value - 70));

            shiftValueRefresh();
        }

        private void ButCalcDFT_Click(object sender, RoutedEventArgs e)
        {
            if (indTrueSpectr == null) return;

            allPlotRefresh();
            indTrueSpectr.Fill = System.Windows.Media.Brushes.ForestGreen;
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

    }
}
