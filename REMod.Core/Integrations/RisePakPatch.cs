using REMod.Core.Configuration;
using REMod.Core.Configuration.Enums;
using REMod.Core.Configuration.Structs;
using REMod.Core.Integrations;
using REMod.Core.Internal;
using REMod.Core.Logger;
using REMod.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace REMod.Core.Tools
{
    public class RisePakPatch
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

                if (File.GetAttributes(mod.BasePath).HasFlag(FileAttributes.Directory))
                {
                    try
                    {
                        ProcessStartInfo startInfo = new()
                        {
                            Arguments = $"\"{new DirectoryInfo(mod.BasePath).FullName}\" \"{Path.Combine(directory, PathHelper.MakeValid(mod.Name) + ".pak")}\"",
                            FileName = $"{Constants.TOOLS_FOLDER}RisePakPatch.exe",
                            CreateNoWindow = true,
                        };

                        Process process = new()
                        {
                            StartInfo = startInfo
                        };

                        Process.Start(startInfo);
                    }
                    catch (Exception err)
                    {
                        LogBase.Error(err.ToString());
                    }
                }
            }
        }
    }
}
