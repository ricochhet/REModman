using REMod.Views.Pages;
using System.Windows;
using System;
using Windows.Globalization;
using Wpf.Ui.Appearance;

namespace REMod
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = this;

            Watcher.Watch(this);

            InitializeComponent();

            Loaded += (_, _) => RootNavigation.Navigate(typeof(SetupPage));
        }
    }

}