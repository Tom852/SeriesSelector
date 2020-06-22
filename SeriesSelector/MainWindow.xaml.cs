using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public SeriesViewModel Model = new SeriesViewModel();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Model;
            var tng = new Series(@"F:\New Files\_series\Star Trek xD\TNG");
            var twomen = new Series(@"E:\Serien\Two And A Half Men");
            Model.SeriesList.Add(tng);
            Model.SeriesList.Add(twomen);
        }

        private void PlayBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var index = ButtonClickToListIndex(sender);
            Model.SeriesList[index].Play();
        }

        private void IncBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var index = ButtonClickToListIndex(sender);
            Model.SeriesList[index].Increase();
        }

        private void IncBtn_OnRightClick(object sender, RoutedEventArgs e)
        {
            var index = ButtonClickToListIndex(sender);
            Model.SeriesList[index].Increase(20);
        }

        private void DecBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var index = ButtonClickToListIndex(sender);
            Model.SeriesList[index].Decrease();
        }

        private void DecBtn_OnRightClick(object sender, RoutedEventArgs e)
        {
            var index = ButtonClickToListIndex(sender);
            Model.SeriesList[index].Decrease(20);
        }

        private int ButtonClickToListIndex(object sender)
        {
            var item = (sender as FrameworkElement).DataContext;
            int index = ListView.Items.IndexOf(item);
            return index;
        }
    }
}
