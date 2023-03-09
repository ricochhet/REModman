using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using REFramework.Utils;
using REFramework.Data;
using REFramework.Addons;
using REFramework.Configuration;
using REFramework.Configuration.Enums;

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

        public static void InstallMods(GameType type, List<ModData> modData)
        {
            string installPath = GamePath.GetSavedGamePath(type);
            List<ModData> intercepted = new List<ModData>();

            if (Directory.Exists(installPath))
            {
                foreach (ModData mod in modData)
                {
                    if (REChunkPatchPak.IsREChunkPatchPak(mod.Path))
                    {
                        if (REChunkPatchPak.IsValidGameType(type))
                        {
                            intercepted.Add(mod);
                        }
                    }
                    else
                    {
                        foreach (ModFile file in mod.ModFiles)
                        {
                            string path = "." + file.LocalFilePath.Substring(StringHelper.IndexOfNth(file.LocalFilePath, "\\", 1));
                            file.InstallAbsolutePath = PathHelper.GetAbsolutePath(Path.Combine(installPath, path));
                            file.InstallRelativePath = Path.Combine(installPath, path);
                            // FileStreamHelper.CopyFile(file.SourceRelativePath, Path.Combine(installPath, path), false);
                        }
                    }
                }

                if (REChunkPatchPak.IsValidGameType(type))
                {
                    List<ModData> temp = REChunkPatchPak.InterceptModInstaller(intercepted);
                    foreach (ModData mod in modData)
                    {
                        foreach (ModFile file in mod.ModFiles)
                        {
                            string path = "." + file.LocalFilePath.Substring(StringHelper.IndexOfNth(file.LocalFilePath, "\\", 1));
                            file.InstallAbsolutePath = PathHelper.GetAbsolutePath(Path.Combine(installPath, path));
                            file.InstallRelativePath = Path.Combine(installPath, path);
                            // FileStreamHelper.CopyFile(file.SourceRelativePath, Path.Combine(installPath, path), false);
                        }
                    }
                }
            }
        }

        public static List<ModData> DeserializeModList(GameType type)
        {
            List<ModData> modData = new List<ModData>();
            string gameModFolder = EnumSwitch.GetModFolder(type);
            string gameDataFolder = Path.Combine(Constants.DATA_FOLDER, gameModFolder);

            if (Directory.Exists(gameDataFolder))
            {
                if (File.Exists(Path.Combine(gameDataFolder, Constants.MOD_LIST_FILE)))
                {
                    byte[] bytes = FileStreamHelper.ReadFile(Path.Combine(gameDataFolder, Constants.MOD_LIST_FILE));
                    string file = FileStreamHelper.UnkBytesToStr(bytes);
                    modData = JsonSerializer.Deserialize<List<ModData>>(file);
                }
            }

            return modData;
        }

        public static void SaveModList(GameType type, List<ModData> modList)
        {
            string gameModFolder = EnumSwitch.GetModFolder(type);
            string gameDataFolder = Path.Combine(Constants.DATA_FOLDER, gameModFolder);
            FileStreamHelper.WriteFile(gameDataFolder, Constants.MOD_LIST_FILE, JsonSerializer.Serialize(modList, new JsonSerializerOptions { WriteIndented = true }), false);
        }
    }
}