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

namespace REMod.Views.Pages
{
    public partial class CollectionPage
    {
        private GameType selectedGameType = GameType.None;
        public ObservableCollection<ModItem> ModCollection = new();

        public CollectionPage()
        {
            if (!DataManager.SettingsFileExists())
            {
                DataManager.CreateSettings();
            }

            InitializeComponent();
            PopulateItemsControl();
        }

        private void PopulateItemsControl()
        {
            ModCollection.Clear();

            if (SettingsManager.GetLastSelectedGame() != GameType.None)
            {
                if (DataManager.DataFolderExists(SettingsManager.GetLastSelectedGame()) && DataManager.ModsFolderExists(SettingsManager.GetLastSelectedGame()))
                {
                    List<ModData> index = ModManager.Index(SettingsManager.GetLastSelectedGame());
                    ModManager.Save(SettingsManager.GetLastSelectedGame(), index);

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

        private static void CheckSelectedGameState()
        {
            if (SettingsManager.GetLastSelectedGame() != GameType.None)
            {
                if (!DataManager.ModsFolderExists(SettingsManager.GetLastSelectedGame()))
                {
                    DataManager.CreateModsFolder(SettingsManager.GetLastSelectedGame());
                }

                if (!DataManager.IndexFileExists(SettingsManager.GetLastSelectedGame()))
                {
                    DataManager.CreateIndex(SettingsManager.GetLastSelectedGame());
                }
            }
        }

        private void OpenModFolder_Grid_Visibility()
        {
            if (SettingsManager.GetLastSelectedGame() != GameType.None)
            {
                if (SettingsManager.GetGamePath(SettingsManager.GetLastSelectedGame()) != "")
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
            if (SettingsManager.GetLastSelectedGame() != GameType.None)
            {
                if (SettingsManager.GetGamePath(SettingsManager.GetLastSelectedGame()) != "")
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

        private void GameSelector_ComboBox_Initialize(object sender, EventArgs e)
        {
            GameSelector_ComboBox.Items.Clear();
            GameSelector_ComboBox.ItemsSource = Enum.GetValues(typeof(GameType));
            GameSelector_ComboBox.SelectedIndex = ((int)SettingsManager.GetLastSelectedGame());

            CheckSelectedGameState();
        }

        private void GameSelector_ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (GameSelector_ComboBox.SelectedItem != null)
            {
                selectedGameType = (GameType)Enum.Parse(typeof(GameType), GameSelector_ComboBox.SelectedItem.ToString() ?? GameType.None.ToString());
                SettingsManager.SaveLastSelectedGame(selectedGameType);

                GameSelector_ComboBox.SelectedIndex = ((int)SettingsManager.GetLastSelectedGame());

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
            if (SettingsManager.GetLastSelectedGame() != GameType.None)
            {
                if (Directory.Exists(DataManager.GetModFolderPath(SettingsManager.GetLastSelectedGame())))
                {
                    ProcessStartInfo startInfo = new()
                    {
                        Arguments = PathHelper.GetAbsolutePath(DataManager.GetModFolderPath(SettingsManager.GetLastSelectedGame())),
                        FileName = "explorer.exe",
                    };

                    Process.Start(startInfo);
                }
            }
        }

        private void OpenGameFolder_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsManager.GetLastSelectedGame() != GameType.None)
            {
                if (Directory.Exists(SettingsManager.GetGamePath(SettingsManager.GetLastSelectedGame())))
                {
                    ProcessStartInfo startInfo = new()
                    {
                        Arguments = SettingsManager.GetGamePath(SettingsManager.GetLastSelectedGame()),
                        FileName = "explorer.exe",
                    };

                    Process.Start(startInfo);
                }
            }
        }

        private void Refresh_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsManager.GetLastSelectedGame() != GameType.None)
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
            if (SettingsManager.GetLastSelectedGame() != GameType.None)
            {
                if (!SettingsManager.IsGameRunning(SettingsManager.GetLastSelectedGame()))
                {
                    BaseDialog dialog = new("Mod Manager", $"{SettingsManager.GetLastSelectedGame()} must be running to start the setup process.");
                    dialog.Show();

                    OpenModFolder_Grid_Visibility();
                    SetupGame_CardAction_Visibility();
                    CheckSelectedGameState();
                    PopulateItemsControl();
                }
                else
                {
                    SettingsManager.SaveGamePath(SettingsManager.GetLastSelectedGame());
                    BaseDialog dialog = new("Mod Manager", $"Setup has been completed for {SettingsManager.GetLastSelectedGame()}.");
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

            if (toggle?.Tag is ModItem item && SettingsManager.GetLastSelectedGame() != GameType.None)
            {
                if (Directory.Exists(SettingsManager.GetGamePath(SettingsManager.GetLastSelectedGame())))
                {
                    ModManager.Enable(SettingsManager.GetLastSelectedGame(), item.Hash, true);
                }
                else
                {
                    BaseDialog dialog = new("Mod Manager", $"{SettingsManager.GetLastSelectedGame()} has not been correctly configured.");
                    dialog.Show();
                }
            }
        }

        private void EnableMod_ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch? toggle = sender as ToggleSwitch;

            if (toggle?.Tag is ModItem item && SettingsManager.GetLastSelectedGame() != GameType.None)
            {
                if (Directory.Exists(SettingsManager.GetGamePath(SettingsManager.GetLastSelectedGame())))
                {
                    ModManager.Enable(SettingsManager.GetLastSelectedGame(), item.Hash, false);
                }
                else
                {
                    BaseDialog dialog = new("Mod Manager", $"{SettingsManager.GetLastSelectedGame()} has not been correctly configured.");
                    dialog.Show();
                }
            }
        }

        private void PatchMod_Button_Initialized(object sender, EventArgs e)
        {
            Button? button = sender as Button;

            if (button?.Tag is ModItem item && SettingsManager.GetLastSelectedGame() != GameType.None)
            {
                if (!RisePakPatch.IsPatchable(SettingsManager.GetLastSelectedGame(), item.Hash))
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

            if (button?.Tag is ModItem item && SettingsManager.GetLastSelectedGame() != GameType.None)
            {
                if (RisePakPatch.IsPatchable(SettingsManager.GetLastSelectedGame(), item.Hash))
                {
                    BaseDialog confirmDialog = new("Mod Manager", $"{StringHelper.Truncate(item.Name, 38)} can be converted to a PAK mod, proceed?");
                    confirmDialog.Show();

                    if (await confirmDialog.Confirmed.Task)
                    {
                        RisePakPatch.Patch(SettingsManager.GetLastSelectedGame(), item.Hash);
                    }
                }
            }
        }

        private async void DeleteMod_Button_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            if (button?.Tag is ModItem item && SettingsManager.GetLastSelectedGame() != GameType.None)
            {
                if (Directory.Exists(SettingsManager.GetGamePath(SettingsManager.GetLastSelectedGame())))
                {
                    BaseDialog confirmDialog = new("Mod Manager", $"Do you want to delete mod {StringHelper.Truncate(item.Name, 38)} for {SettingsManager.GetLastSelectedGame()}?");
                    confirmDialog.SetConfirmAppearance(ControlAppearance.Danger);
                    confirmDialog.Show();

                    if (await confirmDialog.Confirmed.Task)
                    {
                        ModManager.Delete(SettingsManager.GetLastSelectedGame(), item.Hash);

                        OpenModFolder_Grid_Visibility();
                        SetupGame_CardAction_Visibility();
                        CheckSelectedGameState();
                        PopulateItemsControl();
                    }
                }
                else
                {
                    BaseDialog dialog = new("Mod Manager", $"{SettingsManager.GetLastSelectedGame()} has not been correctly configured.");
                    dialog.Show();
                }
            }
        }
    }
}
