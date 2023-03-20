using REMod.Core.Configuration.Enums;
using REMod.Core.Logger;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace REMod.Dialogs
{
    public class OpenFolder
    {
        private readonly OpenFolderDialog dialogWindow;

        public OpenFolder(string title, GameType selectedGameType, string selectedGamePath)
        {
            dialogWindow = new OpenFolderDialog(selectedGameType, selectedGamePath)
            {
                
                Title = title
            };

            dialogWindow.RootTitleBar.Title = title;
            dialogWindow.OpenModFolder_CardAction.Click += (s, e) => { OnClick(s, e); };
            dialogWindow.OpenDownloadFolder_CardAction.Click += (s, e) => { OnClick(s, e); };
            dialogWindow.OpenGameFolder_CardAction.Click += (s, e) => { OnClick(s, e); };
            dialogWindow.Cancel_Button.Click += (s, e) => { OnClick(s, e); };
            dialogWindow.Owner = Application.Current.MainWindow;
        }

        public void Show()
        {
            LogBase.Info($"Opening dialog box: {dialogWindow.RootTitleBar.Title}");
            dialogWindow.Show();
            Application.Current.MainWindow.IsEnabled = false;
        }

        private void OnClick(object sender, EventArgs e)
        {
            LogBase.Info($"Closing dialog box: {dialogWindow.RootTitleBar.Title}");
            dialogWindow.Close();
            Application.Current.MainWindow.IsEnabled = true;
        }
    }
}
