using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls.Window;

namespace REMod.Dialogs
{
    public partial class BaseDialogWindow : FluentWindow
    {
        public BaseDialogWindow()
        {
            InitializeComponent();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement? frameworkElement = sender as FrameworkElement;
            if (frameworkElement != null) 
            {
                ((BaseDialog)frameworkElement.DataContext).Close();
            }
        }
    }
}
