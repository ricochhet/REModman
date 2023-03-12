using System;
using System.Windows;
using REModman.Internal;
using REModman.Configuration.Enums;
using System.Diagnostics;
using WinRT;
using REModman.Configuration;
using REModman.Utils;
using REMod.Helpers;
using System.Windows.Controls;
using REModman;

namespace REMod.Views.Pages
{
    public partial class SetupPage
    {
        private GameType selectedGameType = GameType.None;

        public SetupPage()
        {
            InitializeComponent();

            StatusNotifyHelper.Assign("Important information will show up here.");
        }

        private void GameSelector_ComboBox_Initialize(object sender, EventArgs e)
        {
            GameSelector_ComboBox.Items.Clear();
            GameSelector_ComboBox.ItemsSource = Enum.GetValues(typeof(GameType));
            GameSelector_ComboBox.SelectedIndex = ((int)Settings.GetLastSelectedGame());
            StatusNotifyHelper.Assign("Important information will show up here.");
        }

        private void GameSelector_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GameSelector_ComboBox.SelectedItem != null)
            {
                selectedGameType = (GameType)Enum.Parse(typeof(GameType), GameSelector_ComboBox.SelectedItem.ToString() ?? "None");
                if (Settings.GetLastSelectedGame() != GameType.None)
                {
                    selectedGameType = Settings.GetLastSelectedGame();
                }

                Settings.SaveLastSelectedGame(selectedGameType);
                StatusNotifyHelper.Assign($"Selected game: {selectedGameType}");
            }
        }

        private void CreateDataFolder_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                if (!Collection.CheckForDataFolder(selectedGameType))
                {
                    Collection.CreateDataFolder(selectedGameType);
                    StatusNotifyHelper.Assign($"Data folder created for {selectedGameType}.");
                }
                else
                {
                    StatusNotifyHelper.Assign($"{selectedGameType} already has a data folder.");
                }
            }
            else
            {
                StatusNotifyHelper.Assign($"Please select a valid game.");
            }
        }

        private void CreateModsFolder_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                if (!Collection.CheckForModFolder(selectedGameType))
                {
                    Collection.CreateModFolder(selectedGameType);
                    StatusNotifyHelper.Assign($"Mod folder created for {selectedGameType}.");
                }
                else
                {
                    StatusNotifyHelper.Assign($"{selectedGameType} already has a mod folder.");
                }
            }
            else
            {
                StatusNotifyHelper.Assign($"Please select a valid game.");
            }
        }

        private void SetupGameData_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                if (Collection.CheckForDataFolder(selectedGameType))
                {
                    if (ProcessHelper.GetProcIdFromName(EnumSwitch.GetProcName(selectedGameType)) != 0)
                    {
                        Settings.SaveGamePath(selectedGameType);
                        StatusNotifyHelper.Assign($"Game data created for {selectedGameType}.");
                    }
                    else
                    {
                        StatusNotifyHelper.Assign($"{selectedGameType} must be running.");
                    }
                }
                else
                {
                    StatusNotifyHelper.Assign($"{selectedGameType} does not have any data.");
                }
            }
            else
            {
                StatusNotifyHelper.Assign($"Please select a valid game.");
            }
        }

        private void ClearData_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                if (Collection.CheckForDataFolder(selectedGameType))
                {
                    StatusNotifyHelper.Assign($"Deleting data for game: {selectedGameType}.");
                    Collection.DeleteDataFolder(selectedGameType);
                }
                else
                {
                    StatusNotifyHelper.Assign($"{selectedGameType} does not have any data.");
                }
            }
            else
            {
                StatusNotifyHelper.Assign($"Please select a valid game.");
            }
        }
    }
}