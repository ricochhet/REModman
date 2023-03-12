using REMod.Models;
using REModman.Configuration.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace REMod.Helpers
{
    public class GameTypeParser
    {
        public static GameType Parse(ComboBox comboBox)
        {
            return (GameType)Enum.Parse(typeof(GameType), comboBox.SelectedItem.ToString() ?? GameType.None.ToString());
        }

        public static GameType Parse(ModInfoBridge data)
        {
            return (GameType)Enum.Parse(typeof(GameType), data.GameType ?? GameType.None.ToString());
        }
    }
}
