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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
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
            ResetPosition();

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
            Closing += (sender, args) => StorePosition();
        }


        private void StorePosition()
        {
            var reg = new RegistryStuf();
            int height = (int)this.Height;
            int width = (int)this.Width;
            int xPos = (int) this.Left;
            int yPos = (int) this.Top;
            reg.Write("height", height);
            reg.Write("width", width);
            reg.Write("xPos", xPos);
            reg.Write("yPos", yPos);
        }


        private void ResetPosition()
        {
            try
            {
                var reg = new RegistryStuf();
                int h = reg.Read("height");
                int w = reg.Read("width");
                int x = reg.Read("xPos");
                int y = reg.Read("yPos");
                this.Height = h;
                this.Width = w;
                this.Left = x;
                this.Top = y;
            }
            catch (NullReferenceException)
            {
                //using defaults
            }
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
            var index = GetIndexOfElementThatWasClicked(sender);
            Model.SeriesList[index].Play();
        }

        private void IncBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var index = GetIndexOfElementThatWasClicked(sender);
            Model.SeriesList[index].Increase();
        }

        private void IncBtn_OnRightClick(object sender, RoutedEventArgs e)
        {
            var index = GetIndexOfElementThatWasClicked(sender);
            Model.SeriesList[index].Increase(20);
        }

        private void DecBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var index = GetIndexOfElementThatWasClicked(sender);
            Model.SeriesList[index].Decrease();
        }

        private void DecBtn_OnRightClick(object sender, RoutedEventArgs e)
        {
            var index = GetIndexOfElementThatWasClicked(sender);
            Model.SeriesList[index].Decrease(20);
        }

        private int GetIndexOfElementThatWasClicked(object sender)
        {
            var item = (sender as FrameworkElement).DataContext;
            int index = ListView.Items.IndexOf(item);
            return index;
        }

        private void AddSeries(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = "Select the folder with your series";
                dialog.ShowNewFolderButton = false;
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result != System.Windows.Forms.DialogResult.OK)
                {
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
            var item = ListView.SelectedItem;
            int index = ListView.Items.IndexOf(item);
            string seriesToDelete = Model.SeriesList[index].SeriesNameAsString;

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Are you sure to remove {seriesToDelete} ?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Model.SeriesList.RemoveAt(index);
            }
            else
            {
                System.Windows.MessageBox.Show("Delete operation aborted");
            }
        }

        private void ScrollerinoHandler(object sender, MouseWheelEventArgs e)
        {
                ScrollViewer scv = (ScrollViewer)sender;
                scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
                e.Handled = true;
        }
    }
}
