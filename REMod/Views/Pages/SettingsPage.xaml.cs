﻿using REModman.Configuration.Enums;
using REModman.Internal;
using System;
using System.Windows;
using Wpf.Ui.Appearance;

namespace REMod.Views.Pages
{
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            if (Theme.GetAppTheme() == ThemeType.Dark)
                DarkThemeRadioButton.IsChecked = true;
            else
                LightThemeRadioButton.IsChecked = true;
        }

        private void OnLightThemeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            Theme.Apply(ThemeType.Light);
        }

        private void OnDarkThemeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            Theme.Apply(ThemeType.Dark);
        }

        private void CreateIndex_CardAction_Click(object sender, RoutedEventArgs e)
        {
            InitializerManager.CreateIndex(SettingsManager.GetLastSelectedGame());
        }

        private void CreateSettings_CardAction_Click(object sender, RoutedEventArgs e)
        {
            InitializerManager.CreateSettings();
        }

        private void CreateModsFolder_CardAction_Click(object sender, RoutedEventArgs e)
        {
            InitializerManager.CreateModsFolder(SettingsManager.GetLastSelectedGame());
        }

        private void SaveGamePath_CardAction_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.SaveGamePath(SettingsManager.GetLastSelectedGame());
        }

        private void DeleteIndex_CardAction_Click(object sender, RoutedEventArgs e)
        {
            InitializerManager.DeleteIndex(SettingsManager.GetLastSelectedGame());
        }

        private void DeleteSettings_CardAction_Click(object sender, RoutedEventArgs e)
        {
            InitializerManager.DeleteSettings();
        }

        private void DeleteData_CardAction_Click(object sender, RoutedEventArgs e)
        {
            InitializerManager.DeleteDataFolder(SettingsManager.GetLastSelectedGame());
        }
    }
}