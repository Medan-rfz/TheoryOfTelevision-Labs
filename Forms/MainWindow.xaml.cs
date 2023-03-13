using System.Windows;

namespace TheoryOfTelevision
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrightnessChannel_Click(object sender, RoutedEventArgs e)
        {
            LuminanceChannel LuminanceChannelWin = new LuminanceChannel();
            LuminanceChannelWin.Show();
            Application.Current.MainWindow.Hide();
        }

        private void ColorChannel_Click(object sender, RoutedEventArgs e)
        {
            ColorChannel ColorChannelWin = new ColorChannel();
            ColorChannelWin.Show();
            Application.Current.MainWindow.Hide();
        }

        private void HelpWinBut_Click(object sender, RoutedEventArgs e)
        {
            Help HelpWin = new Help();
            HelpWin.Show();
            Application.Current.MainWindow.Hide();
        }

        private void DinamicResearchBut_Click(object sender, RoutedEventArgs e)
        {
            DynamicChannel DynamicChannelWin = new DynamicChannel();
            DynamicChannelWin.Show();
            Application.Current.MainWindow.Hide();
        }

        private void ColorMixWinBut_Click(object sender, RoutedEventArgs e)
        {
            ColorMix ColorMixWin = new ColorMix();
            ColorMixWin.Show();
            Application.Current.MainWindow.Hide();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.Close();
        }

    }
}
