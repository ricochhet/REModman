using System;
using System.Windows;
using Wpf.Ui.Appearance;

namespace REMod.Views.Pages
{
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            AppVersionTextBlock.Text = $"WPF UI - Simple Demo - {GetAssemblyVersion()}";

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

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? String.Empty;
        }
    }
}