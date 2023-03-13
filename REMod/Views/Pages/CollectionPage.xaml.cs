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
                    List<ModData> index = ModDeploy.Index(SettingsManager.GetLastSelectedGame());
                    ModDeploy.Save(SettingsManager.GetLastSelectedGame(), index);
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
                        BaseDialog confirmDialog = new BaseDialog("Mod Manager", $"Do you want to install mod {item.Name} for {SettingsManager.GetLastSelectedGame()}?");
                        confirmDialog.Show();

                        if (await confirmDialog.Confirmed.Task)
                        {
                            ModDeploy.Enable(SettingsManager.GetLastSelectedGame(), item.Guid, true);
                            // ModDeploy.Patch(SettingsManager.GetLastSelectedGame());
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
