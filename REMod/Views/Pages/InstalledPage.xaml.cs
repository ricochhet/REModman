using System;
using System.Windows;
using REModman.Internal;
using REModman.Configuration.Enums;
using System.Diagnostics;
using WinRT;
using System.Windows.Media;
using REMod.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using REMod;
using REMod.Helpers;
using System.Windows.Controls;
using REModman.Configuration.Structs;

namespace REMod.Views.Pages
{
    public partial class InstalledPage
    {
        private GameType selectedGameType = GameType.None;
        public ObservableCollection<ModInfoBridge> ModCollection = new();

        public InstalledPage()
        {
            InitializeComponent();
        }

        private void GameSelector_ComboBox_Initialize(object sender, EventArgs e)
        {
            GameSelector_ComboBox.Items.Clear();
            GameSelector_ComboBox.ItemsSource = Enum.GetValues(typeof(GameType));
            GameSelector_ComboBox.SelectedIndex = 0;
            StatusNotifyHelper.Assign("Important information will show up here.");
        }

        private void GameSelector_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GameSelector_ComboBox.SelectedItem != null)
            {
                selectedGameType = (GameType)Enum.Parse(typeof(GameType), GameSelector_ComboBox.SelectedItem.ToString() ?? "None");
                StatusNotifyHelper.Assign($"Selected game: {selectedGameType}");

                ModCollection.Clear();
                StatusNotifyHelper.Assign($"Selected game: {selectedGameType}");

                if (selectedGameType != GameType.None)
                {
                    if (Collection.CheckForDataFolder(selectedGameType) && Collection.CheckForModFolder(selectedGameType))
                    {
                        List<ModData> index = Installer.DeserializeModList(selectedGameType);

                        foreach (ModData mod in index)
                        {
                            ModCollection.Add(new ModInfoBridge
                            {
                                Name = mod.Name,
                                Guid = mod.Guid,
                                Version = mod.Version,
                                Description = mod.Description,
                                GameType = selectedGameType.ToString(),
                            });
                        }

                        ModsItemsControl.ItemsSource = ModCollection;
                        StatusNotifyHelper.Assign($"Indexed installed mods for {selectedGameType}.");
                    }
                    else
                    {
                        StatusNotifyHelper.Assign($"{selectedGameType} has not been fully setup.");
                    }
                }
                else
                {
                    StatusNotifyHelper.Assign($"Please select a valid game.");
                }
            }
        }
    }
}
