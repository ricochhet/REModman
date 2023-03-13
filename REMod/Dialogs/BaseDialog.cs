using System;
using System.Threading.Tasks;

namespace REMod.Dialogs
{
    public class BaseDialog
    {
        private BaseDialogWindow dialogWindow;
        public TaskCompletionSource<bool> Confirmed = new TaskCompletionSource<bool>();

        public BaseDialog(string title, string content)
        {
            dialogWindow = new BaseDialogWindow();
            dialogWindow.Title = title;
            dialogWindow.RootTitleBar.Title = title;
            dialogWindow.Content_TextBlock.Text = content;
            dialogWindow.Confirm_Button.Click += (s, e) => { OnClick(s, e); };
            dialogWindow.Cancel_Button.Click += (s, e) => { OnClick(s, e); };
        }

        public void Show()
        {
            dialogWindow.Show();
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

            dialogWindow.Close();
        }
    }
}
