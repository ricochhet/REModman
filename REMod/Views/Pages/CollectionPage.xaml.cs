using REMod.Core.Configuration.Enums;
using REMod.Core.Configuration.Structs;
using REMod.Core.Manager;
using REMod.Core.Plugins;
using REMod.Core.Providers;
using REMod.Core.Resolvers.Enums;
using REMod.Core.Utils;
using REMod.Dialogs;
using REMod.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Wpf.Ui.Controls;

namespace REMod.Views.Pages
{
    public partial class CollectionPage
    {
        private GameType selectedGameType = GameType.None;
        private string selectedGamePath = string.Empty;

        public ObservableCollection<ModItem> ModCollection = new();

        public CollectionPage()
        {
            if (!DataProvider.Exists(FileType.Settings))
            {
                DataProvider.Create(FileType.Settings);
            }

            InitializeComponent();
        }

        private void PopulateItemsControl()
        {
            ModCollection.Clear();

            if (selectedGameType != GameType.None)
            {
                if (DataProvider.Exists(FolderType.Data, selectedGameType) && DataProvider.Exists(FolderType.Mods, selectedGameType))
                {
                    List<ModData> index = ManagerCache.Build(selectedGameType);
                    ManagerCache.SaveHashChanges(selectedGameType, index);

                    foreach (ModData mod in index)
                    {
                        ModCollection.Add(new ModItem
                        {
                            Name = mod.Name,
                            Hash = mod.Hash,
                            IsEnabled = mod.IsEnabled,
                        });
                    }

                    ModsItemsControl.ItemsSource = ModCollection;
                }
                else
                {
                    BaseDialog dialog = new("Configuration Error", $"{ManagerSettings.GetLastSelectedGame()} has not been correctly configured.");
                    dialog.Show();
                }
            }
        }

        private void CheckSelectedGameState()
        {
            if (selectedGameType != GameType.None)
            {
                if (!DataProvider.Exists(FolderType.Mods, selectedGameType))
                {
                    DataProvider.Create(FolderType.Mods, selectedGameType);
                }

                if (!DataProvider.Exists(FolderType.Downloads, selectedGameType))
                {
                    DataProvider.Create(FolderType.Downloads, selectedGameType);
                }

                if (!DataProvider.Exists(FileType.Cache, selectedGameType))
                {
                    DataProvider.Create(FileType.Cache, selectedGameType);
                }
            }
        }

        private void ToolBar_Grid_Visibility()
        {
            if (selectedGameType != GameType.None)
            {
                if (selectedGamePath != "")
                {
                    ToolBar_Grid.IsEnabled = true;
                    ToolBar_Grid.Visibility = Visibility.Visible;
                }
                else
                {
                    ToolBar_Grid.IsEnabled = false;
                    ToolBar_Grid.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                ToolBar_Grid.IsEnabled = false;
                ToolBar_Grid.Visibility = Visibility.Collapsed;
            }
        }

        private void SetupGame_CardAction_Visibility()
        {
            if (selectedGameType != GameType.None)
            {
                if (selectedGamePath != "")
                {
                    SetupGame_CardAction.IsEnabled = false;
                    SetupGame_CardAction.Visibility = Visibility.Collapsed;
                }
                else
                {
                    SetupGame_CardAction.IsEnabled = true;
                    SetupGame_CardAction.Visibility = Visibility.Visible;
                }
            }
            else
            {
                SetupGame_CardAction.IsEnabled = false;
                SetupGame_CardAction.Visibility = Visibility.Collapsed;
            }
        }

        private void ModsItemsControl_Initialized(object sender, EventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                PopulateItemsControl();
            }
        }

        private void GameSelector_ComboBox_Initialize(object sender, EventArgs e)
        {
            GameSelector_ComboBox.Items.Clear();
            GameSelector_ComboBox.ItemsSource = Enum.GetValues(typeof(GameType));
            GameSelector_ComboBox.SelectedIndex = (int)ManagerSettings.GetLastSelectedGame();
            selectedGameType = ManagerSettings.GetLastSelectedGame();
            selectedGamePath = ManagerSettings.GetGamePath(selectedGameType);

            CheckSelectedGameState();
        }

        private void GameSelector_ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (GameSelector_ComboBox.SelectedItem != null)
            {
                selectedGameType = (GameType)Enum.Parse(typeof(GameType), GameSelector_ComboBox.SelectedItem.ToString() ?? GameType.None.ToString());
                ManagerSettings.SaveLastSelectedGame(selectedGameType);

                GameSelector_ComboBox.SelectedIndex = (int)ManagerSettings.GetLastSelectedGame();
                selectedGamePath = ManagerSettings.GetGamePath(selectedGameType);

                ToolBar_Grid_Visibility();
                SetupGame_CardAction_Visibility();
                CheckSelectedGameState();
                PopulateItemsControl();
            }
        }

        private void OpenFolder_CardAction_Initialized(object sender, EventArgs e)
        {
            ToolBar_Grid_Visibility();
        }

        private void OpenFolder_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                OpenFolder confirmDialog = new("Open Folder", selectedGameType, selectedGamePath);
                confirmDialog.Show();
            }
        }

        private void Refresh_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                ToolBar_Grid_Visibility();
                SetupGame_CardAction_Visibility();
                CheckSelectedGameState();
                PopulateItemsControl();
            }
        }

        private void SetupGame_CardAction_Initialized(object sender, EventArgs e)
        {
            SetupGame_CardAction_Visibility();
        }

        private void SetupGame_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                if (!ProcessHelper.IsProcRunning(selectedGameType))
                {
                    BaseDialog dialog = new("Mod Manager", $"{selectedGameType} must be running to start the setup process.");
                    dialog.Show();

                    ToolBar_Grid_Visibility();
                    SetupGame_CardAction_Visibility();
                    CheckSelectedGameState();
                    PopulateItemsControl();
                }
                else
                {
                    ManagerSettings.SaveGamePath(selectedGameType);
                    BaseDialog dialog = new("Mod Manager", $"Setup has been completed for {selectedGameType}.");
                    dialog.Show();

                    ToolBar_Grid_Visibility();
                    SetupGame_CardAction_Visibility();
                    CheckSelectedGameState();
                    PopulateItemsControl();
                }
            }
        }

        private void EnableMod_ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch? toggle = sender as ToggleSwitch;

            if (toggle?.Tag is ModItem item && selectedGameType != GameType.None)
            {
                if (Directory.Exists(selectedGamePath))
                {
                    ModInstaller.Enable(selectedGameType, item.Hash, true);
                }
                else
                {
                    BaseDialog dialog = new("Mod Manager", $"{selectedGameType} has not been correctly configured.");
                    dialog.Show();
                }
            }
        }

        private void EnableMod_ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch? toggle = sender as ToggleSwitch;

            if (toggle?.Tag is ModItem item && selectedGameType != GameType.None)
            {
                if (Directory.Exists(selectedGamePath))
                {
                    ModInstaller.Enable(selectedGameType, item.Hash, false);
                }
                else
                {
                    BaseDialog dialog = new("Mod Manager", $"{selectedGameType} has not been correctly configured.");
                    dialog.Show();
                }
            }
        }

        private void PatchMod_Button_Initialized(object sender, EventArgs e)
        {
            Button? button = sender as Button;

            if (button?.Tag is ModItem item && selectedGameType != GameType.None)
            {
                if (!RisePakPatchExtension.IsPatchable(selectedGameType, item.Hash))
                {
                    button.IsEnabled = false;
                }
                else
                {
                    button.IsEnabled = true;
                }
            }
        }

        private async void PatchMod_Button_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            if (button?.Tag is ModItem item && selectedGameType != GameType.None)
            {
                if (RisePakPatchExtension.IsPatchable(selectedGameType, item.Hash))
                {
                    BaseDialog confirmDialog = new("Mod Manager", $"{StringHelper.Truncate(item.Name, 38)} can be converted to a PAK mod, proceed?");
                    confirmDialog.Show();

                    if (await confirmDialog.Confirmed.Task)
                    {
                        RisePakPatchExtension.Patch(selectedGameType, item.Hash);
                    }
                }
            }
        }

        private void LoadOrder_NumberBox_Initialized(object sender, EventArgs e)
        {
            NumberBox? numberBox = sender as NumberBox;

            if (numberBox?.Tag is ModItem item && selectedGameType != GameType.None)
            {
                numberBox.Value = ModInstaller.GetLoadOrder(selectedGameType, item.Hash);
            }
        }

        private void LoadOrder_NumberBox_ValueChanged(object sender, RoutedEventArgs e)
        {
            NumberBox? numberBox = sender as NumberBox;

            if (numberBox?.Tag is ModItem item && selectedGameType != GameType.None)
            {
                if (numberBox.Value != null)
                {
                    ModInstaller.SetLoadOrder(selectedGameType, item.Hash, (int)numberBox.Value);
                }
            }
        }

        private async void DeleteMod_Button_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            if (button?.Tag is ModItem item && selectedGameType != GameType.None)
            {
                if (Directory.Exists(selectedGamePath))
                {
                    BaseDialog confirmDialog = new("Mod Manager", $"Do you want to delete mod {StringHelper.Truncate(item.Name, 38)} for {ManagerSettings.GetLastSelectedGame()}?");
                    confirmDialog.SetConfirmAppearance(ControlAppearance.Danger);
                    confirmDialog.Show();

                    if (await confirmDialog.Confirmed.Task)
                    {
                        ModInstaller.Delete(selectedGameType, item.Hash);

                        ToolBar_Grid_Visibility();
                        SetupGame_CardAction_Visibility();
                        CheckSelectedGameState();
                        PopulateItemsControl();
                    }
                }
                else
                {
                    BaseDialog dialog = new("Mod Manager", $"{selectedGameType} has not been correctly configured.");
                    dialog.Show();
                }
            }
        }
    }
}
