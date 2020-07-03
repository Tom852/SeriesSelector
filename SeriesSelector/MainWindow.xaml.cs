using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using MediaToolkit;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace SeriesSelector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SeriesViewModel Model { get; set; } = new SeriesViewModel();
        public readonly Clock c = new Clock();
        public ProgressBarHandler pbh = new ProgressBarHandler();

        public MainWindow()
        {
            InitializeComponent();
            new PositionMaster(this).LoadPosition();

            Model.SeriesList = new PersistenceMaster().Load();

            Model.Time = c.Time;
            c.OnTimechange += (o, s) => Model.Time = s;

            DataContext = Model;

            Model.SeriesList.CollectionChanged += StoreData;

            Closing += StoreData;
            Closing += (o, a) => new PositionMaster(this).StorePosition();

            Application.Current.DispatcherUnhandledException += HandleException;
        }

        #region ButtonClicks
        private void PlayBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var index = GetIndexOfElementThatWasClicked(sender);
            Series s = Model.SeriesList[index];
            Play(s);
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

        private void PlusBtn_OnClick(object sender, RoutedEventArgs e)
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

        private void XBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var index = GetIndexOfElementThatWasClicked(sender);
            Series s = Model.SeriesList[index];
            RemoveSeries(s, index);
        }

        private void EBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var index = GetIndexOfElementThatWasClicked(sender);
            Series s = Model.SeriesList[index];
            var path = s.GetFullFilePathOfCurrentEpisode();
            string argument = $"/e, /select, \"{path}\"";
            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        private void HelpBtn_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Right click the arrows to skip 20 episodes at once.\nTo remove a series, mark the text an press DEL.", "Hints", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DollarBtn_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Feel free to donate with Paypal to thomaskistler (at) bluewin (dot) ch", "Donate", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
            System.Diagnostics.Process.Start("https://www.paypal.me/tomk453");
        }

        #endregion ButtonClicks


        #region SCrollAndKeyEvents
        private void ListView_OnKeyDown(object sender, KeyEventArgs e)
        {
            var item = ListView.SelectedItem;
            if (item == null) return;
            int index = ListView.Items.IndexOf(item);
            Series targetSeries = Model.SeriesList[index];

            switch (e.Key)
            {
                case Key.Delete:
                    RemoveSeries(targetSeries, index);
                    break;
                case Key.Enter:
                    Play(targetSeries);
                    break;
                case Key.Left:
                    targetSeries.Decrease();
                    break;
                case Key.Right:
                    targetSeries.Increase();
                    break;
                case Key.Up:
                    targetSeries.Increase(20);
                    break;
                case Key.Down:
                    targetSeries.Decrease(20);
                    break;
                case Key.E:
                    //todo
                    break;

            }

            ListView.SelectedIndex = index;
            e.Handled = true;
        }

        private void ScrollerinoHandler(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        #endregion

        private void StoreData(object sender, EventArgs e)
        {
            new PersistenceMaster().Persist(Model.SeriesList);
        }


        private void HandleException(object s, DispatcherUnhandledExceptionEventArgs a)
        {
            string who = s.ToString();
            string why = a.Exception.Message;
            string trace = a.Exception.StackTrace;
            MessageBox.Show($"{who} caused an error:\n{why}\n{trace}", "An Ooopsie happened", MessageBoxButton.OK, MessageBoxImage.Error);
            a.Handled = true;
        }


        private void Play(Series s)
        {
            InitializeProgressBar(s);
            s.Play();
        }
    

        private void InitializeProgressBar(Series series)
        {
            pbh?.Stop();
            var inputFile = new MediaToolkit.Model.MediaFile { Filename = series.GetFullFilePathOfCurrentEpisode() };
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
            }

            var duration = inputFile.Metadata.Duration.TotalSeconds;

            pbh.Start(duration);
            pbh.PercentageChanged += (s, p) => Model.Progress = p;
        }

        

        private int GetIndexOfElementThatWasClicked(object sender)
        {
            var item = (sender as FrameworkElement).DataContext;
            int index = ListView.Items.IndexOf(item);
            return index;
        }

        



        private void RemoveSeries(Series targetSeries, int index)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(
                $"Are you sure to remove {targetSeries} ?", "Remove Confirmation",
                System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Model.SeriesList.RemoveAt(index);
            }
        }





    }
}