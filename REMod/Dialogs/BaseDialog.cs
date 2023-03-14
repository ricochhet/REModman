using REModman.Logger;
using System;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Controls;

namespace REMod.Dialogs
{
    public class BaseDialog
    {
        private readonly BaseDialogWindow dialogWindow;
        public TaskCompletionSource<bool> Confirmed = new();

        public BaseDialog(string title, string content)
        {
            dialogWindow = new BaseDialogWindow
            {
                Title = title
            };
            dialogWindow.RootTitleBar.Title = title;
            dialogWindow.Content_TextBlock.Text = content;
            dialogWindow.Confirm_Button.Click += (s, e) => { OnClick(s, e); };
            dialogWindow.Cancel_Button.Click += (s, e) => { OnClick(s, e); };
            dialogWindow.Owner = Application.Current.MainWindow;
        }

        public void Show()
        {
            LogBase.Info($"[DIALOG] Opening dialog box: {dialogWindow.RootTitleBar.Title} - {dialogWindow.Content_TextBlock.Text}");
            dialogWindow.Show();
            Application.Current.MainWindow.IsEnabled = false;
        }

        public void SetConfirmAppearance(ControlAppearance appearance)
        {
            dialogWindow.Confirm_Button.Appearance = appearance;
        }

        private void OnClick(object sender, EventArgs e)
        {
            if (sender == dialogWindow.Confirm_Button)
            {
                Confirmed.SetResult(true);
            }
            else
            {
                Confirmed.SetResult(false);
            }

            LogBase.Info($"[DIALOG] Closing dialog box: {dialogWindow.RootTitleBar.Title} - {dialogWindow.Content_TextBlock.Text}");
            dialogWindow.Close();
            Application.Current.MainWindow.IsEnabled = true;
        }
    }
}
