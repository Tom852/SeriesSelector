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
        public MainWindow()
        {
            InitializeComponent();
            var model = new SeriesViewModel();
            model.SeriesList = new ObservableCollection<Series>();
            DataContext = model;
            var tng = new Series(@"F:\New Files\_series\Star Trek xD\TNG");
            var twomen = new Series(@"E:\Serien\Two And A Half Men");
            model.SeriesList.Add(tng);
            model.SeriesList.Add(twomen);
        }

    }
}
