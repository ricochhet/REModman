using REModman.Configuration.Enums;
using System.Collections.Generic;

namespace REModman.Configuration.Structs
{
    public class SettingsData
    {
        public GameType LastSelectedGame { get; set; }
        public Dictionary<string, string> GamePaths { get; set; }
    }
}
