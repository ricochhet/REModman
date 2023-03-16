using REModman.Configuration.Enums;
using REModman.Configuration.Structs;
using REModman.Internal;
using REModman.Patches;
using REModman.Utils;
using System.Collections.Generic;
using System.IO;

namespace REModman.Tools
{
    public class RisePakPatchExtensions
    {
        public static bool IsPatchable(GameType type, string identifier)
        {
            List<ModData> list = ModManager.Deserialize(type);
            ModData mod = ModManager.Find(list, identifier);

            if (PakDataPatch.NativesExists(mod.BasePath))
            {
                return true;
            }

            return false;
        }

        public static void Patch(GameType type, string identifier)
        {
            List<ModData> list = ModManager.Deserialize(type);
            ModData mod = ModManager.Find(list, identifier);

            if (Directory.Exists(mod.BasePath))
            {
                string directory = mod.BasePath + "_PAK";
                if (Directory.Exists(directory))
                    Directory.Delete(directory, true);

                Directory.CreateDirectory(directory);
                RisePakPatch.ProcessDirectory(mod.BasePath, Path.Combine(directory, PathHelper.MakeValid(mod.Name) + ".pak"));
            }
        }
    }
}
