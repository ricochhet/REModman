using REMod.Dialogs;
using REMod.Core.Internal;
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

        private async void DeleteData_CardAction_Click(object sender, RoutedEventArgs e)
        {
            BaseDialog confirmDialog = new("Mod Manager", $"This action is irreversible, are you sure?");
            confirmDialog.SetConfirmAppearance(ControlAppearance.Danger);
            confirmDialog.Show();

            if (await confirmDialog.Confirmed.Task)
            {
                DataManager.DeleteGameDataFolder(SettingsManager.GetLastSelectedGame());
            }
        }
    }
}