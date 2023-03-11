using System;
using System.Windows;
using REModman.Internal;
using REModman.Configuration.Enums;
using System.Diagnostics;
using WinRT;
using REModman.Configuration;
using REModman.Utils;

namespace Wpf.Ui.Demo.Simple.Views.Pages;

public partial class SetupPage
{
    private GameType selectedGameType = GameType.None;

    public SetupPage()
    {
        InitializeComponent();
    }

    private void GameSelector_ComboBox_Initialize(object sender, System.EventArgs e)
    {
        GameSelector_ComboBox.Items.Clear();
        GameSelector_ComboBox.ItemsSource = Enum.GetValues(typeof(GameType));
        GameSelector_ComboBox.SelectedIndex = 0;
        StatusBarLog_TextBlock.Text = "Important information will show up here.";
    }

    private void GameSelector_ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (GameSelector_ComboBox.SelectedItem != null)
        {
            selectedGameType = (GameType)Enum.Parse(typeof(GameType), GameSelector_ComboBox.SelectedItem.ToString() ?? "None");
            StatusBarLog_TextBlock.Text = $"Selected game: {selectedGameType}";
        }
    }

    private void CreateDataFolder_CardAction_Click(object sender, RoutedEventArgs e)
    {
        if (selectedGameType != GameType.None)
        {
            if (!ModIndexer.CheckForDataFolder(selectedGameType))
            {
                ModIndexer.CreateDataFolder(selectedGameType);
                StatusBarLog_TextBlock.Text = $"Data folder created for {selectedGameType}.";
            }
            else
            {
                StatusBarLog_TextBlock.Text = $"{selectedGameType} already has a data folder.";
            }
        }
        else
        {
            StatusBarLog_TextBlock.Text = $"Please select a valid game.";
        }
    }

    private void CreateModsFolder_CardAction_Click(object sender, RoutedEventArgs e)
    {
        if (selectedGameType != GameType.None)
        {
            if (!ModIndexer.CheckForModFolder(selectedGameType))
            {
                ModIndexer.CreateModFolder(selectedGameType);
                StatusBarLog_TextBlock.Text = $"Mod folder created for {selectedGameType}.";
            }
            else
            {
                StatusBarLog_TextBlock.Text = $"{selectedGameType} already has a mod folder.";
            }
        }
        else
        {
            StatusBarLog_TextBlock.Text = $"Please select a valid game.";
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
                    StatusBarLog_TextBlock.Text = $"Game data created for {selectedGameType}.";
                }
                else
                {
                    StatusBarLog_TextBlock.Text = $"{selectedGameType} must be running.";
                }
            }
            else
            {
                StatusBarLog_TextBlock.Text = $"{selectedGameType} does not have any data.";
            }
        }
        else
        {
            StatusBarLog_TextBlock.Text = $"Please select a valid game.";
        }
    }

    private void ClearData_CardAction_Click(object sender, RoutedEventArgs e)
    {
        if (selectedGameType != GameType.None)
        {
            if (ModIndexer.CheckForDataFolder(selectedGameType))
            {
                StatusBarLog_TextBlock.Text = $"Deleting data for game: {selectedGameType}.";
                ModIndexer.DeleteDataFolder(selectedGameType);
            }
            else
            {
                StatusBarLog_TextBlock.Text = $"{selectedGameType} does not have any data.";
            }
        }
        else
        {
            StatusBarLog_TextBlock.Text = $"Please select a valid game.";
        }
    }
}
