using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Xml.Serialization;
using Path = System.IO.Path;

namespace SeriesSelector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SeriesViewModel Model { get; set; }
        private readonly string persistenceFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TomsSeriesShit/SeriesPicker.data");
        public MainWindow()
        {

            InitializeComponent();

            if (File.Exists(persistenceFile))
            {
                string xml = File.ReadAllText(persistenceFile);
                TextReader tr = new StringReader(xml);

                var s = new XmlSerializer(typeof(SeriesViewModel));
                var data = (SeriesViewModel) s.Deserialize(tr);
                Model = data;
            }
            else
            {
                Model = new SeriesViewModel();
            }
            DataContext = Model;

            Closing += (sender, args) => PersistData();
        }

        private void PersistData()
        {
            var s = new XmlSerializer(typeof(SeriesViewModel));
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            s.Serialize(sw, Model);
            string xmlResult = sw.GetStringBuilder().ToString();
            Directory.CreateDirectory(Path.GetDirectoryName(persistenceFile));
            File.WriteAllText(persistenceFile, xmlResult);
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

        private void AddSeries(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result != System.Windows.Forms.DialogResult.OK)
                {
                    MessageBox.Show("No path was provided", "Alert", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!Directory.GetFiles(dialog.SelectedPath).Any())
                {
                    MessageBox.Show("This folder is empty", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Model.SeriesList.Add(new Series(dialog.SelectedPath));
            }
        }

        private void ListView_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete)
            {
                e.Handled = true;
                return;
            }
            var item = (ListView.SelectedItem as FrameworkElement).DataContext;
            int index = ListView.Items.IndexOf(item);
        }
    }
}
