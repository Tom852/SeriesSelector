using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SeriesSelector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            LabelTop.Content = new Core().GetPrintableName(Series.TNG);
            LabelBot.Content = new Core().GetPrintableName(Series.Men);
        }

        private void TopLaunch(object sender, RoutedEventArgs e)
        {
            new Core().PlayNext(Series.TNG);
            UpdateLabels();
        }

        private void TopInc(object sender, RoutedEventArgs e)
        {
            new Core().Increase(Series.TNG);
            UpdateLabels();

        }
        private void TopInc2(object sender, RoutedEventArgs e)
        {
            new Core().Increase(Series.TNG, 20);
            UpdateLabels();
        }

        private void TopDec(object sender, RoutedEventArgs e)
        {
            new Core().Decrease(Series.TNG);
            UpdateLabels();
        }

        private void TopDec2(object sender, RoutedEventArgs e)
        {
            new Core().Decrease(Series.TNG, 20);
            UpdateLabels();
        }

        private void BotLaunch(object sender, RoutedEventArgs e)
        {
            new Core().PlayNext(Series.Men);
            UpdateLabels();

        }

        private void BotInc(object sender, RoutedEventArgs e)
        {
            new Core().Increase(Series.Men);
            UpdateLabels();

        }
        private void BotInc2(object sender, RoutedEventArgs e)
        {
            new Core().Increase(Series.Men, 20);
            UpdateLabels();

        }
        private void BotDec(object sender, RoutedEventArgs e)
        {
            new Core().Decrease(Series.Men);
            UpdateLabels();

        }

        private void BotDec2(object sender, RoutedEventArgs e)
        {
            new Core().Decrease(Series.Men, 20);
            UpdateLabels();

        }
    }
}
