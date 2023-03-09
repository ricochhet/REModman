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
            ModIndexer.CreateDataFolder(GameType.MonsterHunterRise);
            /*List<ModData> index = ModIndexer.IndexModDirectory(GameType.MonsterHunterRise);
            ModIndexer.SaveModIndex(index);*/

           /* List<ModData> installList = ModController.DeserializeModList();
            List<ModData> install = ModController.UninstallMod(installList, new Dictionary<string, string>
            {
                {"My Mod", "7262ba6e-9992-4f17-b13a-6b3ca036917a"},
            });

            ModController.SaveModList(install);*/
            /*List<ModData> install = ModController.SelectMods(ModIndexer.DeserializeModIndex(), new Dictionary<string, string>
            {
                {"My Other Other Mod", "1f072867-0e71-44f8-9ab6-5b034da733e4"},
            });

            ModController.InstallMods(GamePath.GetSavedGamePath(GameType.MonsterHunterRise), install);

            ModController.SaveModList(install);*/
            /*List<ModData> install = ModController.SelectMods(ModIndexer.DeserializeModIndex(), new Dictionary<string, string>
            {
                {"My Mod", "7262ba6e-9992-4f17-b13a-6b3ca036917a"},
                {"My Other Mod", "af770814-8283-40cc-bd75-43ca92b3dbc0"},
            });

            ModController.InstallMods(GamePath.GetSavedGamePath(GameType.MonsterHunterRise), install);*/
            /*;*/
            // List<ModData> index = ModIndexer.IndexModDirectory(GameType.MonsterHunterRise);
            // ModIndexer.SaveModIndex(index);

            /*

            ModController.SaveModList(install);*/

            // ModIndexer.CreateModFolder(GameType.MonsterHunterWorld);
            // ModIndexer.CreateDataFolder();
            // GamePath.SaveGamePath(GameType.MonsterHunterRise);
            // ModIndexer.CreateDataFolder();
            // ModIndexer.DeleteDataFolder();
            /*List<ModData> index = ModIndexer.IndexModDirectory("./Mods/");
            // ModIndexer.SaveModIndex("./Mods/", index);

            List<ModData> install = ModController.SelectMods(ModIndexer.DeserializeModIndex("./Mods/"), new Dictionary<string, string>
            {
                {"My Mod", "ba94ebaa-c809-44b6-bb46-f1f86516b1c8"},
                {"My Other Mod", "11bd33de-4ddb-40a4-ad1d-25a23afa0965"},
            });

            ModController.InstallMods(".\\InstallPath\\", install);

            ModController.SaveModList("./Mods/", install);*/
        }
    }
}