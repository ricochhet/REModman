using REMod.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REModman.Utils;
using System.Windows;

namespace REMod.Helpers
{
    public class StatusNotifyHelper
    {
        public static void Assign(string String)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            string temp = String ?? string.Empty;

            if (temp.Length > Constants.MAX_NOTIFY_CHAR_LENGTH)
            {
                temp = StringHelper.Truncate(temp, Constants.MAX_NOTIFY_CHAR_LENGTH, "...");
            }

            mainWindow.StatusBarNotifier_TextBlock.Text = temp;
        }
    }
}
