using REMod.Dialogs;
using REMod.Models;
using REMod.Core.Configuration.Enums;
using REMod.Core.Configuration.Structs;
using REMod.Core.Internal;
using REMod.Core.Tools;
using REMod.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Wpf.Ui.Controls;
using REMod.Core.Logger;

namespace REMod.Views.Pages
{
    public partial class CollectionPage
    {
        private GameType selectedGameType = GameType.None;
        private string selectedGamePath = string.Empty;

        public ObservableCollection<ModItem> ModCollection = new();

        public CollectionPage()
        {
            if (!DataManager.SettingsFileExists())
            {
                DataManager.CreateSettings();
            }

            InitializeComponent();
        }

        private void PopulateItemsControl()
        {
            ModCollection.Clear();

            if (selectedGameType != GameType.None)
            {
                if (DataManager.DataFolderExists(selectedGameType) && DataManager.ModsFolderExists(selectedGameType))
                {
                    List<ModData> index = ModManager.Index(selectedGameType);
                    ModManager.SafeSave(selectedGameType, index);

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
                    BaseDialog dialog = new("Configuration Error", $"{SettingsManager.GetLastSelectedGame()} has not been correctly configured.");
                    dialog.Show();
                }
            }
        }

        private void CheckSelectedGameState()
        {
            if (selectedGameType != GameType.None)
            {
                if (!DataManager.ModsFolderExists(selectedGameType))
                {
                    DataManager.CreateModsFolder(selectedGameType);
                }

                if (!DataManager.IndexFileExists(selectedGameType))
                {
                    DataManager.CreateIndex(selectedGameType);
                }
            }
        }

        private void OpenModFolder_Grid_Visibility()
        {
            if (selectedGameType != GameType.None)
            {
                if (selectedGamePath != "")
                {
                    OpenModFolder_Grid.IsEnabled = true;
                    OpenModFolder_Grid.Visibility = Visibility.Visible;
                }
                else
                {
                    OpenModFolder_Grid.IsEnabled = false;
                    OpenModFolder_Grid.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                OpenModFolder_Grid.IsEnabled = false;
                OpenModFolder_Grid.Visibility = Visibility.Collapsed;
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
            GameSelector_ComboBox.SelectedIndex = (int)SettingsManager.GetLastSelectedGame();
            selectedGameType = SettingsManager.GetLastSelectedGame();
            selectedGamePath = SettingsManager.GetGamePath(selectedGameType);

            CheckSelectedGameState();
        }

        private void GameSelector_ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (GameSelector_ComboBox.SelectedItem != null)
            {
                selectedGameType = (GameType)Enum.Parse(typeof(GameType), GameSelector_ComboBox.SelectedItem.ToString() ?? GameType.None.ToString());
                SettingsManager.SaveLastSelectedGame(selectedGameType);

                GameSelector_ComboBox.SelectedIndex = (int)SettingsManager.GetLastSelectedGame();
                selectedGamePath = SettingsManager.GetGamePath(selectedGameType);

                OpenModFolder_Grid_Visibility();
                SetupGame_CardAction_Visibility();
                CheckSelectedGameState();
                PopulateItemsControl();
            }
        }

        private void OpenModFolder_CardAction_Initialized(object sender, EventArgs e)
        {
            OpenModFolder_Grid_Visibility();
        }

        private void OpenModFolder_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                if (Directory.Exists(DataManager.GetModFolderPath(selectedGameType)))
                {
                    ProcessStartInfo startInfo = new()
                    {
                        Arguments = PathHelper.GetAbsolutePath(DataManager.GetModFolderPath(selectedGameType)),
                        FileName = "explorer.exe",
                    };

                    Process.Start(startInfo);
                }
            }
        }

        private void OpenGameFolder_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                if (Directory.Exists(selectedGamePath))
                {
                    ProcessStartInfo startInfo = new()
                    {
                        Arguments = selectedGamePath,
                        FileName = "explorer.exe",
                    };

                    Process.Start(startInfo);
                }
            }
        }

        private void Refresh_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                OpenModFolder_Grid_Visibility();
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
                if (!SettingsManager.IsGameRunning(selectedGameType))
                {
                    BaseDialog dialog = new("Mod Manager", $"{selectedGameType} must be running to start the setup process.");
                    dialog.Show();

                    OpenModFolder_Grid_Visibility();
                    SetupGame_CardAction_Visibility();
                    CheckSelectedGameState();
                    PopulateItemsControl();
                }
                else
                {
                    SettingsManager.SaveGamePath(selectedGameType);
                    BaseDialog dialog = new("Mod Manager", $"Setup has been completed for {selectedGameType}.");
                    dialog.Show();

                    OpenModFolder_Grid_Visibility();
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
                    ModManager.Enable(selectedGameType, item.Hash, true);
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
                    ModManager.Enable(selectedGameType, item.Hash, false);
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
                numberBox.Value = ModManager.GetLoadOrder(selectedGameType, item.Hash);
            }
        }

        private void LoadOrder_NumberBox_ValueChanged(object sender, RoutedEventArgs e)
        {
            NumberBox? numberBox = sender as NumberBox;

            if (numberBox?.Tag is ModItem item && selectedGameType != GameType.None)
            {
                if (numberBox.Value != null)
                {
                    ModManager.SetLoadOrder(selectedGameType, item.Hash, (int)numberBox.Value);
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
                    BaseDialog confirmDialog = new("Mod Manager", $"Do you want to delete mod {StringHelper.Truncate(item.Name, 38)} for {SettingsManager.GetLastSelectedGame()}?");
                    confirmDialog.SetConfirmAppearance(ControlAppearance.Danger);
                    confirmDialog.Show();

                    if (await confirmDialog.Confirmed.Task)
                    {
                        ModManager.Delete(selectedGameType, item.Hash);

                        OpenModFolder_Grid_Visibility();
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
