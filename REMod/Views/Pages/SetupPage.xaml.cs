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
            GameSelector_ComboBox.SelectedIndex = ((int)SettingsManager.GetLastSelectedGame());
            StatusNotifyHelper.Assign("Important information will show up here.");
        }

        private void GameSelector_ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (GameSelector_ComboBox.SelectedItem != null)
            {
                selectedGameType = GameTypeParser.Parse(GameSelector_ComboBox);
                SettingsManager.SaveLastSelectedGame(selectedGameType);

                GameSelector_ComboBox.SelectedIndex = ((int)SettingsManager.GetLastSelectedGame());
                StatusNotifyHelper.Assign($"Selected game: {selectedGameType}");
            }
        }

        private void Setup_CardAction_Click(object sender, RoutedEventArgs e)
        {
            // SetupActionHelper.GetSetupAction(SetupType.CREATE_INDEX, selectedGameType);
        }

        private void CreateIndex_CardAction_Click(object sender, RoutedEventArgs e)
        {
            SetupActionHelper.GetAction(SetupType.CREATE_INDEX, selectedGameType);
        }

        private void CreateList_CardAction_Click(object sender, RoutedEventArgs e)
        {
            SetupActionHelper.GetAction(SetupType.CREATE_LIST, selectedGameType);
        }

        private void CreateSettings_CardAction_Click(object sender, RoutedEventArgs e)
        {
            SetupActionHelper.GetAction(SetupType.CREATE_SETTINGS, selectedGameType);
        }

        private void CreateModsFolder_CardAction_Click(object sender, RoutedEventArgs e)
        {
            SetupActionHelper.GetAction(SetupType.CREATE_MODS_FOLDER, selectedGameType);
        }

        private void SaveGamePath_CardAction_Click(object sender, RoutedEventArgs e)
        {
            SetupActionHelper.GetAction(SetupType.SAVE_GAME_PATH, selectedGameType);
        }

        private void DeleteIndex_CardAction_Click(object sender, RoutedEventArgs e)
        {
            SetupActionHelper.GetAction(SetupType.DELETE_INDEX, selectedGameType);
        }

        private void DeleteList_CardAction_Click(object sender, RoutedEventArgs e)
        {
            SetupActionHelper.GetAction(SetupType.DELETE_LIST, selectedGameType);
        }

        private void DeleteSettings_CardAction_Click(object sender, RoutedEventArgs e)
        {
            SetupActionHelper.GetAction(SetupType.DELETE_SETTINGS, selectedGameType);
        }

        private void DeleteData_CardAction_Click(object sender, RoutedEventArgs e)
        {
            SetupActionHelper.GetAction(SetupType.DELETE_DATA_FOLDER, selectedGameType);
        }
    }
}