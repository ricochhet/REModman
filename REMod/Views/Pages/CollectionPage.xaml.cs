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
        private GameType selectedGameType = GameType.None;
        public ObservableCollection<ModItem> ModCollection = new();

        public CollectionPage()
        {
            if (!DataManager.SettingsFileExists())
            {
                DataManager.CreateSettings();
            }

            InitializeComponent();
            InitializeModCollection();
        }

        private void InitializeModCollection()
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
                            Guid = mod.Hash,
                            Version = mod.Version,
                            Description = mod.Description,
                            IsEnabled = mod.IsEnabled,
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

        private void InitializeSetupChecks()
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

        private void SetOpenModFolderVisibility()
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

        private void SetSetupButtonVisiblity()
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

            InitializeSetupChecks();
        }

        private void GameSelector_ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (GameSelector_ComboBox.SelectedItem != null)
            {
                selectedGameType = (GameType)Enum.Parse(typeof(GameType), GameSelector_ComboBox.SelectedItem.ToString() ?? GameType.None.ToString());
                SettingsManager.SaveLastSelectedGame(selectedGameType);

                GameSelector_ComboBox.SelectedIndex = ((int)SettingsManager.GetLastSelectedGame());

                SetOpenModFolderVisibility();
                SetSetupButtonVisiblity();
                InitializeSetupChecks();
                InitializeModCollection();
            }
        }

        private void OpenModFolder_CardAction_Initialized(object sender, EventArgs e)
        {
            SetOpenModFolderVisibility();
        }

        private void OpenModFolder_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsManager.GetLastSelectedGame() != GameType.None)
            {
                if (Directory.Exists(DataManager.GetModFolderPath(SettingsManager.GetLastSelectedGame())))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        Arguments = DataManager.GetModFolderPath(SettingsManager.GetLastSelectedGame()),
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
                SetOpenModFolderVisibility();
                SetSetupButtonVisiblity();
                InitializeSetupChecks();
                InitializeModCollection();
            }
        }

        private void SetupGame_CardAction_Initialized(object sender, EventArgs e)
        {
            SetSetupButtonVisiblity();
        }

        private void SetupGame_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsManager.GetLastSelectedGame() != GameType.None)
            {
                if (!SettingsManager.IsGameRunning(SettingsManager.GetLastSelectedGame()))
                {
                    BaseDialog dialog = new BaseDialog("Mod Manager", $"{SettingsManager.GetLastSelectedGame()} must be running to start the setup process.");
                    dialog.Show();

                    SetOpenModFolderVisibility();
                    SetSetupButtonVisiblity();
                    InitializeSetupChecks();
                    InitializeModCollection();
                }
                else
                {
                    SettingsManager.SaveGamePath(SettingsManager.GetLastSelectedGame());
                    BaseDialog dialog = new BaseDialog("Mod Manager", $"Setup has been completed for {SettingsManager.GetLastSelectedGame()}.");
                    dialog.Show();

                    SetOpenModFolderVisibility();
                    SetSetupButtonVisiblity();
                    InitializeSetupChecks();
                    InitializeModCollection();
                }
            }
        }

        private void EnableMod_ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch? toggle = sender as ToggleSwitch;
            ModItem? item = toggle?.Tag as ModItem;

            if (item != null && SettingsManager.GetLastSelectedGame() != GameType.None)
            {
                if (Directory.Exists(SettingsManager.GetGamePath(SettingsManager.GetLastSelectedGame())))
                {
                    ModManager.Enable(SettingsManager.GetLastSelectedGame(), item.Guid, true);
                }
                else
                {
                    BaseDialog dialog = new BaseDialog("Mod Manager", $"{SettingsManager.GetLastSelectedGame()} has not been correctly configured.");
                    dialog.Show();
                }
            }
        }

        private void EnableMod_ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch? toggle = sender as ToggleSwitch;
            ModItem? item = toggle?.Tag as ModItem;

            if (item != null && SettingsManager.GetLastSelectedGame() != GameType.None)
            {
                if (Directory.Exists(SettingsManager.GetGamePath(SettingsManager.GetLastSelectedGame())))
                {
                    ModManager.Enable(SettingsManager.GetLastSelectedGame(), item.Guid, false);
                }
                else
                {
                    BaseDialog dialog = new BaseDialog("Mod Manager", $"{SettingsManager.GetLastSelectedGame()} has not been correctly configured.");
                    dialog.Show();
                }
            }
        }

        private async void DeleteMod_Button_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            ModItem? item = button?.Tag as ModItem;

            if (item != null && SettingsManager.GetLastSelectedGame() != GameType.None)
            {
                if (Directory.Exists(SettingsManager.GetGamePath(SettingsManager.GetLastSelectedGame())))
                {
                    BaseDialog confirmDialog = new BaseDialog("Mod Manager", $"Do you want to delete mod {item.Name} for {SettingsManager.GetLastSelectedGame()}?");
                    confirmDialog.SetConfirmAppearance(ControlAppearance.Danger);
                    confirmDialog.Show();

                    if (await confirmDialog.Confirmed.Task)
                    {
                        ModManager.Delete(SettingsManager.GetLastSelectedGame(), item.Guid);

                        SetOpenModFolderVisibility();
                        SetSetupButtonVisiblity();
                        InitializeSetupChecks();
                        InitializeModCollection();
                    }
                }
                else
                {
                    BaseDialog dialog = new BaseDialog("Mod Manager", $"{SettingsManager.GetLastSelectedGame()} has not been correctly configured.");
                    dialog.Show();
                }
            }
        }
    }
}
