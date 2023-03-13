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
using REModman.Configuration.Structs;
using Wpf.Ui.Controls;
using System.IO;
using REMod.Dialogs;

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
                        });
                    }

                    ModsItemsControl.ItemsSource = ModCollection;
                }
                else
                {
                    BaseDialog dialog = new BaseDialog("Configuration Error", $"{SettingsManager.GetLastSelectedGame()} has not been correctly configured.");
                    dialog.Show();
                }
            }
        }

        private async void UninstallMod_Button_Click(object sender, RoutedEventArgs e)
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
                            BaseDialog confirmDialog = new BaseDialog("Mod Manager", $"Do you want to uninstall mod {item.Name} for {SettingsManager.GetLastSelectedGame()}?");
                            confirmDialog.Show();

                            if (await confirmDialog.Confirmed.Task == true)
                            {
                                ModData? temp = installedModList.Find(i => i.Name == selectedMod.Name && i.Guid == selectedMod.Guid);
                                if (temp != null)
                                {
                                    installedModList.Remove(temp);
                                    ModManager.Uninstall(SettingsManager.GetLastSelectedGame(), selectedMod);
                                    ModManager.SaveData(SettingsManager.GetLastSelectedGame(), installedModList);
                                    ModCollection.Remove(item);
                                }
                            }
                        }
                        else
                        {
                            BaseDialog dialog = new BaseDialog("Mod Manager", $"{SettingsManager.GetLastSelectedGame()} does not have an installed mod called {item.Name}.");
                            dialog.Show();
                        }
                    }
                    else
                    {
                        BaseDialog dialog = new BaseDialog("Configuration Error", $"{SettingsManager.GetLastSelectedGame()} has not been correctly configured.");
                        dialog.Show();
                    }
                }
            }
            else
            {
                throw new NullReferenceException();
            }
        }
    }
}
