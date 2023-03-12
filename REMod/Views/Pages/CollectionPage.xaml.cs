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
using REModman.Configuration.Structs;
using Wpf.Ui.Controls;
using System.IO;

namespace REMod.Views.Pages
{
    public partial class CollectionPage
    {
        public ObservableCollection<ModItem> ModCollection = new();

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
                            ModCollection.Add(new ModItem
                            {
                                Name = mod.Name,
                                Guid = mod.Guid,
                                Version = mod.Version,
                                Description = mod.Description,
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

        private void InstallMod_Button_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            ModItem? item = button?.Tag as ModItem;

            if (item != null)
            {
                if (SettingsManager.GetLastSelectedGame() != GameType.None)
                {
                    if (Directory.Exists(SettingsManager.GetGamePath(SettingsManager.GetLastSelectedGame())))
                    {
                        ModData selectedMod = ModManager.Select(ModIndexer.DeserializeData(SettingsManager.GetLastSelectedGame()), item.Name, item.Guid);
                        bool isModInstalled = ModManager.IsInstalled(SettingsManager.GetLastSelectedGame(), selectedMod);
                        List<ModData> installedModList = ModManager.DeserializeData(SettingsManager.GetLastSelectedGame());

                        if (isModInstalled == false)
                        {
                            installedModList.Add(selectedMod);
                            ModManager.Install(SettingsManager.GetLastSelectedGame(), selectedMod);
                            ModManager.SaveData(SettingsManager.GetLastSelectedGame(), installedModList);
                            StatusNotifyHelper.Assign($"Installed mod: {item.Name} for {SettingsManager.GetLastSelectedGame()}.");
                        }
                        else
                        {
                            StatusNotifyHelper.Assign($"Mod: {item.Name} is already installed for {SettingsManager.GetLastSelectedGame()}.");
                        }
                    }
                    else
                    {
                        StatusNotifyHelper.Assign($"Could not find the game path for game: {SettingsManager.GetLastSelectedGame()}.");
                    }
                }
                else
                {
                    StatusNotifyHelper.Assign($"Could not get the selected mod: {item.Name}.");
                }
            }
            else
            {
                throw new NullReferenceException();
            }
        }
    }
}
