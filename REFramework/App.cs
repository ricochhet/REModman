using System;
using System.IO;
using REFramework.Data;
using REFramework.Internal;
using REFramework.Configuration.Enums;
using System.Collections.Generic;

namespace REFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            // index mods
            // List<ModData> index = ModIndexer.IndexModDirectory(GameType.MonsterHunterRise);
            // ModIndexer.SaveModIndex(GameType.MonsterHunterRise, index);

            // install mods
            List<ModData> install = ModController.SelectMods(ModIndexer.DeserializeModIndex(GameType.MonsterHunterRise), new Dictionary<string, string>
            {
                {"My Other Other Mod", "d5c2a13e-1e3d-46dd-b73c-6c3df5cc1c60"},
                {"My Other Other Other Mod", "29d0b78d-0dfa-4c11-a8e9-06470c04b35d"},
            });
            ModController.InstallMods(GameType.MonsterHunterRise, install);
            ModController.SaveModList(GameType.MonsterHunterRise, install);

            // uninstall mosd
            // List<ModData> install = ModController.UninstallMod(ModController.DeserializeModList(GameType.MonsterHunterRise), new Dictionary<string, string>
            // {
            //     {"My Other Other Mod", "6b3f3ca0-deed-4f54-be1b-5b17d1b054f0"},
            // });
            // ModController.SaveModList(GameType.MonsterHunterRise, install);
        }
    }
}