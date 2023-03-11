using System.Windows;
using System;

namespace REMod
{
    public partial class App
    {
        public App()
        {
            ResourceDictionary languageDictionary = new ResourceDictionary();
            languageDictionary.Source = new Uri(@".\Languages\en-us.xaml", UriKind.Relative);
            this.Resources.MergedDictionaries.Add(languageDictionary);
        }
    }
}