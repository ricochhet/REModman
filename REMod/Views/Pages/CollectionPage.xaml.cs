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
    public partial class CollectionPage
    {
        private ObservableCollection<ModInfoBridge> modCollection = new();

        public CollectionPage()
        {
            InitializeComponent();

            if (SettingsManager.GetLastSelectedGame() != GameType.None)
            {
                ModCollection.Clear();
                StatusNotifyHelper.Assign($"Selected game: {SettingsManager.GetLastSelectedGame()}");

                if (SettingsManager.GetLastSelectedGame() != GameType.None)
                {
                    if (SetupManager.DataFolderExists(SettingsManager.GetLastSelectedGame()) && SetupManager.ModsFolderExists(SettingsManager.GetLastSelectedGame()))
                    {
                        List<ModData> index = ModIndexer.Index(SettingsManager.GetLastSelectedGame());
                        ModIndexer.SaveData(SettingsManager.GetLastSelectedGame(), index);

                        foreach (ModData mod in index)
                        {
                            ModCollection.Add(new ModInfoBridge
                            {
                                Name = mod.Name,
                                Guid = mod.Guid,
                                Version = mod.Version,
                                Description = mod.Description,
                                VirtualizingItemsControl = ModsItemsControl,
                                GameType = SettingsManager.GetLastSelectedGame().ToString(),
                            });
                        }

                        ModsItemsControl.ItemsSource = ModCollection;
                        StatusNotifyHelper.Assign($"Indexed mod collection for {SettingsManager.GetLastSelectedGame()}.");
                    }
                    else
                    {
                        StatusNotifyHelper.Assign($"{SettingsManager.GetLastSelectedGame()} has not been fully setup.");
                    }
                }
                else
                {
                    StatusNotifyHelper.Assign($"Please select a valid game.");
                }
            }
        }

        public ObservableCollection<ModInfoBridge> ModCollection { get => modCollection; set => modCollection = value; }
    }
}
