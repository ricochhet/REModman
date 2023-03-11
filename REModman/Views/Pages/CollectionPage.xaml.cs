using System;
using System.Windows;
using REModman.Internal;
using REModman.Configuration.Enums;
using System.Diagnostics;
using WinRT;
using System.Windows.Media;
using Wpf.Ui.Demo.Simple.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using REModman.Data;

namespace Wpf.Ui.Demo.Simple.Views.Pages;

public partial class CollectionPage
{
    private GameType selectedGameType = GameType.None;
    public ObservableCollection<ModInfoBridge> ModCollection = new();

    public CollectionPage()
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

    private void IndexModDirectory_CardAction_Click(object sender, RoutedEventArgs e)
    {
        if (selectedGameType != GameType.None)
        {
            if (ModIndexer.CheckForDataFolder(selectedGameType) && ModIndexer.CheckForModFolder(selectedGameType))
            {
                List<ModData> index = ModIndexer.IndexModDirectory(selectedGameType);
                ModIndexer.SaveModIndex(selectedGameType, index);

                foreach (ModData mod in index)
                {
                    ModCollection.Add(new ModInfoBridge
                    {
                        Name = mod.Name,
                        Guid = mod.Guid,
                        Version = mod.Version,
                        Description = mod.Description,
                        LogBox = StatusBarLog_TextBlock,
                        GameType = selectedGameType.ToString(),
                    });
                }
                
                ModsItemsControl.ItemsSource = ModCollection;
                StatusBarLog_TextBlock.Text = $"Indexed mod collection for {selectedGameType}.";
            }
            else
            {
                StatusBarLog_TextBlock.Text = $"{selectedGameType} has not been fully setup.";
            }
        }
        else
        {
            StatusBarLog_TextBlock.Text = $"Please select a valid game.";
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Hi");
    }
}
