﻿using System;
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
        public MainWindow()
        {
            InitializeComponent();
            new PositionMaster(this).LoadPosition();

            Model = new PersistenceMaster().Load();
            DataContext = Model;

            Closing += (sender, args) => new PersistenceMaster().Persist(Model);
            Closing += (sender, args) => new PositionMaster(this).StorePosition();
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
            var item = ListView.SelectedItem;
            int index = ListView.Items.IndexOf(item);
            Series targetSeries = Model.SeriesList[index];

            if (e.Key == Key.Delete)
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(
                    $"Are you sure to remove {targetSeries} ?", "Delete Confirmation",
                    System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    Model.SeriesList.RemoveAt(index);
                }
            }

            if (e.Key == Key.Enter)
            {
                targetSeries.Play();
            }

            e.Handled = true;
        }

        private void ScrollerinoHandler(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
