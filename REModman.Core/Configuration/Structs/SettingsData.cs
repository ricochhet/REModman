using REModman.Configuration.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REModman.Configuration.Structs
{
    public class SettingsData
    {
        public GameType LastSelectedGame { get; set; }
        public Dictionary<string, string> GamePaths { get; set; }
    }
}
