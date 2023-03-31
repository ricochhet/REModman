using REMod.Core.Configuration.Enums;
using REMod.Core.Providers;
using REMod.Core.Resolvers;
using REMod.Core.Resolvers.Enums;
using REMod.Core.Utils;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Wpf.Ui.Controls.Window;

namespace REMod.Dialogs
{
    public partial class OpenFolderDialog : FluentWindow
    {
        private static GameType selectedGameType = GameType.None;
        private static string selectedGamePath = string.Empty;

        public OpenFolderDialog(GameType type, string gamePath)
        {
            selectedGameType = type;
            selectedGamePath = gamePath;

            InitializeComponent();
        }

        private void OpenModFolder_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                if (DataProvider.Exists(FolderType.Mods, selectedGameType))
                {
                    ProcessStartInfo startInfo = new()
                    {
                        Arguments = PathHelper.GetAbsolutePath(PathResolver.ModPath(selectedGameType)),
                        FileName = "explorer.exe",
                    };

                    Process.Start(startInfo);
                }
            }
        }

        private void OpenDownloadFolder_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                if (DataProvider.Exists(FolderType.Mods, selectedGameType))
                {
                    ProcessStartInfo startInfo = new()
                    {
                        Arguments = PathHelper.GetAbsolutePath(PathResolver.DownloadPath(selectedGameType)),
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
    }
}
