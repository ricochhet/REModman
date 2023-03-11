using REModman.Configuration.Enums;
using REModman.Data;
using REModman.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Common;

namespace Wpf.Ui.Demo.Simple.Models
{
    public static class CollectionPageViewModel
    {
        public static ICommand InstallModCommand => new RelayCommand<ModInfoBridge>(InstallMod);

        private static void InstallMod(ModInfoBridge? data)
        {
            if (data != null)
            {
                GameType gameType = (GameType)Enum.Parse(typeof(GameType), data.GameType ?? "None");

                if (data.LogBox != null)
                {
                    if (gameType != GameType.None)
                    {
                        data.LogBox.Text = "Hello";
                    }
                    else
                    {
                        data.LogBox.Text = "Hello"
                    }
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
            else
            {
                throw new NullReferenceException();
            }
        }
    }
}
