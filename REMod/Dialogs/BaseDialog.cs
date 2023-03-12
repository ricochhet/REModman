using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Wpf.Ui.Controls;
using REMod.Views;
using System.Windows.Input;
using Wpf.Ui.Common;

namespace REMod.Dialogs
{
    public class BaseDialog
    {
        private BaseDialogWindow modalWindow;

        public BaseDialog(string title, string content)
        {
            modalWindow = new BaseDialogWindow();
            modalWindow.Title = title;
            modalWindow.RootTitleBar.Title = title;
            modalWindow.Content_TextBlock.Text = content;

            modalWindow.Confirm_Button.Click += (s, e) => { Confirm(); };
            modalWindow.Cancel_Button.Click += (s, e) => { Close(); };
        }

        public void Show()
        {
            modalWindow.Show();
        }

        public void Close()
        {
            modalWindow.Close();
        }

        public bool Confirm()
        {
            modalWindow.Close();
            return true;
        }
    }
}
