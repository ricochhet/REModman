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
using Wpf.Ui.Contracts;
using Wpf.Ui.Controls.Navigation;
using System.Drawing.Printing;
using REMod.Dialogs;

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

        private async void InstallMod_Button_Click(object sender, RoutedEventArgs e)
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
                            BaseDialog confirmDialog = new BaseDialog("Mod Manager", $"Do you want to install mod {item.Name} for {SettingsManager.GetLastSelectedGame()}?");
                            confirmDialog.Show();

                            if (await confirmDialog.Confirmed.Task)
                            {
                                installedModList.Add(selectedMod);
                                ModManager.Install(SettingsManager.GetLastSelectedGame(), selectedMod);
                                ModManager.SaveData(SettingsManager.GetLastSelectedGame(), installedModList);
                            }
                        }
                        else
                        {
                            BaseDialog dialog = new BaseDialog("Mod Manager", $"{SettingsManager.GetLastSelectedGame()} already has a mod called {item.Name}.");
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
