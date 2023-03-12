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
    public partial class InstalledPage
    {
        public ObservableCollection<ModItem> ModCollection = new();

        public InstalledPage()
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
                        List<ModData> index = ModManager.DeserializeData(SettingsManager.GetLastSelectedGame());

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
                        StatusNotifyHelper.Assign($"Indexed installed mods for {SettingsManager.GetLastSelectedGame()}.");
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

        private void UninstallMod_Button_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            ModItem? item = button?.Tag as ModItem;

            if (item != null)
            {
                if (SettingsManager.GetLastSelectedGame() != GameType.None)
                {
                    if (Directory.Exists(SettingsManager.GetGamePath(SettingsManager.GetLastSelectedGame())))
                    {
                        ModData selectedMod = ModManager.Select(ModManager.DeserializeData(SettingsManager.GetLastSelectedGame()), item.Name, item.Guid);
                        bool isModInstalled = ModManager.IsInstalled(SettingsManager.GetLastSelectedGame(), selectedMod);
                        List<ModData> installedModList = ModManager.DeserializeData(SettingsManager.GetLastSelectedGame());

                        if (isModInstalled == true)
                        {
                            ModData? temp = installedModList.Find(i => i.Name == selectedMod.Name && i.Guid == selectedMod.Guid);
                            if (temp != null)
                            {
                                installedModList.Remove(temp);
                                ModManager.Uninstall(SettingsManager.GetLastSelectedGame(), selectedMod);
                                ModManager.SaveData(SettingsManager.GetLastSelectedGame(), installedModList);
                            }
                        }
                        else
                        {
                            StatusNotifyHelper.Assign($"Mod: {item.Name} is not installed for {SettingsManager.GetLastSelectedGame()}.");
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
