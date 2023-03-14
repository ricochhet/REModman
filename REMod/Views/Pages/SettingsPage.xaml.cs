using REMod.Dialogs;
using REModman.Internal;
using System.Windows;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace REMod.Views.Pages
{
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            if (Theme.GetAppTheme() == ThemeType.Dark)
                DarkThemeRadioButton.IsChecked = true;
            else
                LightThemeRadioButton.IsChecked = true;
        }

        private void OnLightThemeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            Theme.Apply(ThemeType.Light);
        }

        private void OnDarkThemeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            Theme.Apply(ThemeType.Dark);
        }

        private async void CreateIndex_CardAction_Click(object sender, RoutedEventArgs e)
        {
            BaseDialog confirmDialog = new("Mod Manager", $"This action may have unintended consequences, are you sure?");
            confirmDialog.SetConfirmAppearance(ControlAppearance.Danger);
            confirmDialog.Show();

            if (await confirmDialog.Confirmed.Task)
            {
                DataManager.CreateIndex(SettingsManager.GetLastSelectedGame());
            }
        }

        private async void CreateSettings_CardAction_Click(object sender, RoutedEventArgs e)
        {
            BaseDialog confirmDialog = new("Mod Manager", $"This action may have unintended consequences, are you sure?");
            confirmDialog.SetConfirmAppearance(ControlAppearance.Danger);
            confirmDialog.Show();

            if (await confirmDialog.Confirmed.Task)
            {
                DataManager.CreateSettings();
            }
        }

        private async void CreateModsFolder_CardAction_Click(object sender, RoutedEventArgs e)
        {
            BaseDialog confirmDialog = new("Mod Manager", $"This action may have unintended consequences, are you sure?");
            confirmDialog.SetConfirmAppearance(ControlAppearance.Danger);
            confirmDialog.Show();

            if (await confirmDialog.Confirmed.Task)
            {
                DataManager.CreateModsFolder(SettingsManager.GetLastSelectedGame());
            }
        }

        private async void SaveGamePath_CardAction_Click(object sender, RoutedEventArgs e)
        {
            BaseDialog confirmDialog = new("Mod Manager", $"This action may have unintended consequences, are you sure?");
            confirmDialog.SetConfirmAppearance(ControlAppearance.Danger);
            confirmDialog.Show();

            if (await confirmDialog.Confirmed.Task)
            {
                SettingsManager.SaveGamePath(SettingsManager.GetLastSelectedGame());
            }
        }

        private async void DeleteIndex_CardAction_Click(object sender, RoutedEventArgs e)
        {
            BaseDialog confirmDialog = new("Mod Manager", $"This action may have unintended consequences, are you sure?");
            confirmDialog.SetConfirmAppearance(ControlAppearance.Danger);
            confirmDialog.Show();

            if (await confirmDialog.Confirmed.Task)
            {
                DataManager.DeleteIndex(SettingsManager.GetLastSelectedGame());
            }
        }

        private async void DeleteSettings_CardAction_Click(object sender, RoutedEventArgs e)
        {
            BaseDialog confirmDialog = new("Mod Manager", $"This action may have unintended consequences, are you sure?");
            confirmDialog.SetConfirmAppearance(ControlAppearance.Danger);
            confirmDialog.Show();

            if (await confirmDialog.Confirmed.Task)
            {
                DataManager.DeleteSettings();
            }
        }

        private async void DeleteData_CardAction_Click(object sender, RoutedEventArgs e)
        {
            BaseDialog confirmDialog = new("Mod Manager", $"This action may have unintended consequences, are you sure?");
            confirmDialog.SetConfirmAppearance(ControlAppearance.Danger);
            confirmDialog.Show();

            if (await confirmDialog.Confirmed.Task)
            {
                DataManager.DeleteDataFolder(SettingsManager.GetLastSelectedGame());
            }
        }
    }
}