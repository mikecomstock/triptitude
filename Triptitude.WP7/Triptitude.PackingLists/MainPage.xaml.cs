﻿using System.Linq;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace Triptitude.PackingLists
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            TagsListBox.DataContext = App.TagsViewModel;
            Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!App.TagsViewModel.IsDataLoaded)
            {
                App.TagsViewModel.LoadData();
            }
        }

        private void TagsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mainPivot.SelectedItem = itemsPivotItem;
        }

        private void mainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainPivot.SelectedItem == itemsPivotItem)
            {
                if (TagsListBox.SelectedIndex != -1)
                {
                    ItemsListBox.ItemsSource = App.TagsViewModel.Tags[TagsListBox.SelectedIndex].Items;
                }
                else
                {
                    // Using GropuBy and Select instead of distinct (gets around having to deal with equality methods
                    ItemsListBox.ItemsSource = App.TagsViewModel.Tags
                        .SelectMany(t => t.Items)
                        .GroupBy(i => i.Id)
                        .Select(g => g.First())
                        .OrderBy(i => i.Name);
                }
            }
            else
            {
                TagsListBox.SelectedIndex = -1;
            }
        }
    }
}