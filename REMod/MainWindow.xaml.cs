using REMod.Views.Pages;
using System.Windows;
using System;
using Windows.Globalization;
using Wpf.Ui.Appearance;
using System.Windows.Controls;
using REModman.Configuration.Enums;
using REMod.Helpers;

namespace REMod
{
    public partial class MainWindow
    {
        public GameType SelectedGameType = GameType.None;

        public MainWindow()
        {
            DataContext = this;

            Watcher.Watch(this);

            InitializeComponent();

            Loaded += (_, _) => RootNavigation.Navigate(typeof(SetupPage));
        }
    }
}