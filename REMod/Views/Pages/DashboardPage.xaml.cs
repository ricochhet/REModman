using System;
using System.Windows;
using REModman.Internal;
using REModman.Configuration.Enums;
using System.Diagnostics;
using WinRT;
using REModman.Configuration;
using REModman.Utils;
using REMod.Helpers;
using System.Windows.Controls;

namespace REMod.Views.Pages
{
    public partial class DashboardPage
    {
        public DashboardPage()
        {
            InitializeComponent();

            ResourceDictionary languageDictionary = new ResourceDictionary();
            languageDictionary.Source = new Uri(@".\Languages\en-us.xaml", UriKind.Relative);
            this.Resources.MergedDictionaries.Add(languageDictionary);
            StatusNotifyHelper.Assign("Important information will show up here.");
        }
    }
}