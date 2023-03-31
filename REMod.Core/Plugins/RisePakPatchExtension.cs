using REMod.Core.Configuration.Enums;
using REMod.Core.Configuration.Structs;
using REMod.Core.Integrations;
using REMod.Core.Manager;
using REMod.Core.Utils;
using System.Collections.Generic;
using System.IO;

namespace REMod.Core.Plugins
{
    public class RisePakPatchExtension
    {
        public static bool IsPatchable(GameType type, string identifier)
        {
            List<ModData> list = ManagerCache.Load(type);
            ModData mod = ManagerCache.Find(list, identifier);

            if (mod == null)
            {
                return false;
            }

            if (REEDataPatch.HasNatives(mod.BasePath))
            {
                return true;
            }

            return false;
        }

        public static void Patch(GameType type, string identifier)
        {
            List<ModData> list = ManagerCache.Load(type);
            ModData mod = ManagerCache.Find(list, identifier);

            if (mod == null)
            {
                return;
            }

            if (Directory.Exists(mod.BasePath))
            {
                string directory = mod.BasePath + " Pak Version";
                if (Directory.Exists(directory))
                    Directory.Delete(directory, true);

                Directory.CreateDirectory(directory);

                if (File.GetAttributes(mod.BasePath).HasFlag(FileAttributes.Directory))
                {
                    RisePakPatch.ProcessDirectory(new DirectoryInfo(mod.BasePath).FullName, Path.Combine(directory, PathHelper.MakeValid(mod.Name) + ".pak"));
                }
            }
        }
    }
}
