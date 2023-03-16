using REMod.Core.Configuration.Enums;
using REMod.Core.Configuration.Structs;
using REMod.Core.Internal;
using REMod.Core.Patches;
using REMod.Core.Utils;
using System.Collections.Generic;
using System.IO;

namespace REMod.Core.Tools
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
