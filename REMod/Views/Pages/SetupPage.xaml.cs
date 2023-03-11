using System;
using System.Windows;
using REModman.Internal;
using REModman.Configuration.Enums;
using System.Diagnostics;
using WinRT;
using REModman.Configuration;
using REModman.Utils;
using REMod.Helpers;

namespace REMod.Views.Pages
{
    public partial class SetupPage
    {
        private GameType selectedGameType = GameType.None;

        public SetupPage()
        {
            InitializeComponent();

            ResourceDictionary languageDictionary = new ResourceDictionary();
            languageDictionary.Source = new Uri(@".\Languages\en-us.xaml", UriKind.Relative);
            this.Resources.MergedDictionaries.Add(languageDictionary);
        }

        private void GameSelector_ComboBox_Initialize(object sender, System.EventArgs e)
        {
            GameSelector_ComboBox.Items.Clear();
            GameSelector_ComboBox.ItemsSource = Enum.GetValues(typeof(GameType));
            GameSelector_ComboBox.SelectedIndex = 0;
            StatusNotifier_TextBlock.Text = StatusNotifyHelper.Assign("Important information will show up here.");
        }

        private void GameSelector_ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (GameSelector_ComboBox.SelectedItem != null)
            {
                selectedGameType = (GameType)Enum.Parse(typeof(GameType), GameSelector_ComboBox.SelectedItem.ToString() ?? "None");
                StatusNotifier_TextBlock.Text = $"Selected game: {selectedGameType}";
            }
        }

        private void CreateDataFolder_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                if (!ModIndexer.CheckForDataFolder(selectedGameType))
                {
                    ModIndexer.CreateDataFolder(selectedGameType);
                    StatusNotifier_TextBlock.Text = $"Data folder created for {selectedGameType}.";
                }
                else
                {
                    StatusNotifier_TextBlock.Text = $"{selectedGameType} already has a data folder.";
                }
            }
            else
            {
                StatusNotifier_TextBlock.Text = $"Please select a valid game.";
            }
        }

        private void CreateModsFolder_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                if (!ModIndexer.CheckForModFolder(selectedGameType))
                {
                    ModIndexer.CreateModFolder(selectedGameType);
                    StatusNotifier_TextBlock.Text = $"Mod folder created for {selectedGameType}.";
                }
                else
                {
                    StatusNotifier_TextBlock.Text = $"{selectedGameType} already has a mod folder.";
                }
            }
            else
            {
                StatusNotifier_TextBlock.Text = $"Please select a valid game.";
            }
        }

        private void SetupGameData_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                if (ModIndexer.CheckForDataFolder(selectedGameType))
                {
                    if (ProcessHelper.GetProcIdFromName(EnumSwitch.GetProcName(selectedGameType)) != 0)
                    {
                        GamePath.SaveGamePath(selectedGameType);
                        StatusNotifier_TextBlock.Text = $"Game data created for {selectedGameType}.";
                    }
                    else
                    {
                        StatusNotifier_TextBlock.Text = $"{selectedGameType} must be running.";
                    }
                }
                else
                {
                    StatusNotifier_TextBlock.Text = $"{selectedGameType} does not have any data.";
                }
            }
            else
            {
                StatusNotifier_TextBlock.Text = $"Please select a valid game.";
            }
        }

        private void ClearData_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                if (ModIndexer.CheckForDataFolder(selectedGameType))
                {
                    StatusNotifier_TextBlock.Text = $"Deleting data for game: {selectedGameType}.";
                    ModIndexer.DeleteDataFolder(selectedGameType);
                }
                else
                {
                    StatusNotifier_TextBlock.Text = $"{selectedGameType} does not have any data.";
                }
            }
            else
            {
                StatusNotifier_TextBlock.Text = $"Please select a valid game.";
            }
        }
    }
}