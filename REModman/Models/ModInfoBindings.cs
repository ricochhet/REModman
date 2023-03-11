using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;

namespace Wpf.Ui.Demo.Simple.Models;

public class ModInfoBindings : IMultiValueConverter
{
    public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
    {
        return new ModInfoBridge()
        {
            Name = Values[0].ToString(),
            Guid = Values[1].ToString(),
            GameType = Values[2].ToString(),
            LogBox = (TextBlock)Values[3],
        };
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}