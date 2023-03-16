using REMod.Core.Configuration.Enums;
using System.Collections.Generic;

namespace REMod.Core.Configuration.Structs
{
    public class SettingsData
    {
        public GameType LastSelectedGame { get; set; }
        public Dictionary<string, string> GamePaths { get; set; }
    }
}
