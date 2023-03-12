using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using REModman.Utils;
using REModman.Patches;
using REModman.Configuration;
using REModman.Configuration.Enums;
using REModman.Internal;
using REModman.Configuration.Structs;
using System.Security.AccessControl;

namespace REModman.Internal
{
    public class ModManager
    {
        public static List<ModData> Select(List<ModData> modData, Dictionary<string, string> selectedMods)
        {
            List<ModData> installList = new List<ModData>();

            if (modData.Count != 0)
            {
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
            }

            return installList;
        }

        public static void Install(GameType type, List<ModData> selectedMods)
        {
            string installPath = SettingsManager.GetGamePath(type);
            List<ModData> intercepted = new List<ModData>();

            if (Directory.Exists(installPath))
            {
                foreach (ModData mod in selectedMods)
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
                            FileStreamHelper.CopyFile(file.SourceRelativePath, Path.Combine(installPath, path), false);
                        }
                    }
                }

                if (REChunkPatchPak.IsValidGameType(type))
                {
                    if (intercepted.Count != 0)
                    {
                        List<ModData> temp = REChunkPatchPak.InterceptModInstaller(intercepted);

                        foreach (ModData mod in selectedMods)
                        {
                            if (REChunkPatchPak.IsREChunkPatchPak(mod.Path))
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
                }
            }
        }

        public static void Uninstall(GameType type, List<ModData> selectedMods)
        {
            string installPath = SettingsManager.GetGamePath(type);

            if (Directory.Exists(installPath))
            {
                foreach (ModData mod in selectedMods)
                {
                    foreach (ModFile file in mod.ModFiles)
                    {
                        File.Delete(file.InstallAbsolutePath);
                    }
                }
            }
        }

        public static bool IsInstalled(GameType type, ModData mod)
        {
            bool isInstalled = false;
            List<ModData> installedModList = DeserializeData(type);

            foreach (ModData installedMod in installedModList)
            {
                if (installedMod.Name == mod.Name && installedMod.Guid == mod.Guid)
                {
                    isInstalled = true;
                    break;
                }
            }

            return isInstalled;
        }

        public static List<ModData> DeserializeData(GameType type)
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

        public static void SaveData(GameType type, List<ModData> modList)
        {
            string gameModFolder = EnumSwitch.GetModFolder(type);
            string gameDataFolder = Path.Combine(Constants.DATA_FOLDER, gameModFolder);
            FileStreamHelper.WriteFile(gameDataFolder, Constants.MOD_LIST_FILE, JsonSerializer.Serialize(modList, new JsonSerializerOptions { WriteIndented = true }), false);
        }
    }
}