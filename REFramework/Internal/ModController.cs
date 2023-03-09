using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using REFramework.Utils;
using REFramework.Data;
using REFramework.Configuration;

namespace REFramework.Internal
{
    public class ModController
    {
        public static List<ModData> UninstallMod(List<ModData> modData, Dictionary<string, string> selectedMods)
        {
            List<ModData> installList = new List<ModData>();

            foreach (ModData mod in modData)
            {
                installList.Add(mod);
                
                if (selectedMods.ContainsKey(mod.Name))
                {
                    if (selectedMods[mod.Name] == mod.Guid)
                    {
                        foreach (ModFile file in mod.ModFiles)
                        {
                            File.Delete(file.InstallAbsolutePath);
                        }

                        installList.Remove(mod);
                    }
                }
            }

            return installList;
        }

        public static List<ModData> SelectMods(List<ModData> modData, Dictionary<string, string> selectedMods)
        {
            List<ModData> installList = new List<ModData>();

            foreach (ModData mod in modData)
            {
                if (selectedMods.ContainsKey(mod.Name))
                {
                    if (selectedMods[mod.Name] == mod.Guid)
                    {
                        installList.Add(mod);
                    }
                }
            }

            return installList;
        }

        public static void InstallMods(string installPath, List<ModData> modData)
        {
            if (Directory.Exists(installPath))
            {
                foreach (ModData mod in modData)
                {
                    foreach (ModFile file in mod.ModFiles)
                    {
                        string path = "." + file.LocalFilePath.Substring(StringHelper.IndexOfNth(file.LocalFilePath, "\\", 1));
                        file.InstallAbsolutePath = PathHelper.GetAbsolutePath(Path.Combine(installPath, path));
                        file.InstallRelativePath = Path.Combine(installPath, path);
                        FileStreamHelper.CopyFile(file.SourceRelativePath, Path.Combine(installPath, path), false);
                    }
                }
            }
        }

        public static List<ModData> DeserializeModList()
        {
            List<ModData> modData = new List<ModData>();

            if (Directory.Exists(Constants.DATA_FOLDER))
            {
                if (File.Exists(Path.Combine(Constants.DATA_FOLDER, Constants.MOD_LIST_FILE)))
                {
                    byte[] bytes = FileStreamHelper.ReadFile(Path.Combine(Constants.DATA_FOLDER, Constants.MOD_LIST_FILE));
                    string file = FileStreamHelper.UnkBytesToStr(bytes);
                    modData = JsonSerializer.Deserialize<List<ModData>>(file);
                }
            }

            return modData;
        }

        public static void SaveModList(List<ModData> modList)
        {
            FileStreamHelper.WriteFile(Constants.DATA_FOLDER, Constants.MOD_LIST_FILE, JsonSerializer.Serialize(modList, new JsonSerializerOptions { WriteIndented = true }), false);
        }
    }
}