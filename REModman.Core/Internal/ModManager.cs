using IniParser;
using IniParser.Model;
using REModman.Configuration;
using REModman.Configuration.Enums;
using REModman.Configuration.Structs;
using REModman.Logger;
using REModman.Patches;
using REModman.Tools;
using REModman.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace REModman.Internal
{
    public class ModManager
    {
        private static List<ModData> Deserialize(GameType type)
        {
            List<ModData> list = new();
            string dataFolder = Path.Combine(Constants.DATA_FOLDER, EnumSwitch.GetModFolder(type));

            if (Directory.Exists(dataFolder))
            {
                if (File.Exists(Path.Combine(dataFolder, Constants.MOD_INDEX_FILE)))
                {
                    byte[] bytes = FileStreamHelper.ReadFile(Path.Combine(dataFolder, Constants.MOD_INDEX_FILE), false);
                    string file = FileStreamHelper.UnkBytesToStr(bytes);
                    list = JsonSerializer.Deserialize<List<ModData>>(file);
                }
            }

            return list;
        }

        private static ModData Find(List<ModData> list, string identifier) => list.Find(i => i.Hash == identifier);

        public static void Save(GameType type, List<ModData> list)
        {
            FileStreamHelper.WriteFile(Path.Combine(Constants.DATA_FOLDER, EnumSwitch.GetModFolder(type)), Constants.MOD_INDEX_FILE, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }), false);
        }

        public static List<ModData> Index(GameType type)
        {
            List<ModData> list = Deserialize(type);
            IniDataParser parser = new();

            string gamePath = SettingsManager.GetGamePath(type);
            string modFolder = Path.Combine(Constants.MODS_FOLDER, EnumSwitch.GetModFolder(type));

            if (Directory.Exists(modFolder))
            {
                DirectoryInfo[] mods = new DirectoryInfo(modFolder).GetDirectories("*.*", SearchOption.TopDirectoryOnly);

                foreach (DirectoryInfo obj in mods)
                {
                    string modHash = string.Empty;
                    List<ModFile> modFiles = new();

                    DirectoryInfo[] directories = new DirectoryInfo(obj.FullName).GetDirectories("*.*", SearchOption.TopDirectoryOnly);
                    foreach (DirectoryInfo directory in directories)
                    {
                        if (PakDataPatch.HasNativesFolder(directory.Name))
                        {
                            FileInfo[] nativeFiles = new DirectoryInfo(directory.FullName).GetFiles("*.*", SearchOption.AllDirectories);

                            foreach (FileInfo file in nativeFiles)
                            {
                                string fileHash = CryptoHelper.FileHash.Sha256(file.FullName);
                                modHash += fileHash;

                                modFiles.Add(new ModFile
                                {
                                    InstallPath = Path.Combine(gamePath, PakDataPatch.GetNativesFile(file)),
                                    SourcePath = file.FullName,
                                    Hash = fileHash,
                                });
                            }
                        }
                    }

                    FileInfo[] files = new DirectoryInfo(obj.FullName).GetFiles("*.*", SearchOption.TopDirectoryOnly);
                    foreach (FileInfo file in files)
                    {
                        if (PakDataPatch.HasValidPak(file.FullName))
                        {
                            string fileHash = CryptoHelper.FileHash.Sha256(file.FullName);
                            modHash += fileHash;

                            modFiles.Add(new ModFile
                            {
                                InstallPath = Path.Combine(gamePath, file.Name),
                                SourcePath = file.FullName,
                                Hash = fileHash,
                            });
                        }
                    }

                    string identifier = CryptoHelper.StringHash.Sha256(modHash);

                    if (modFiles.Count != 0 && Find(list, identifier) == null)
                    {
                        string basePath = PathHelper.UnixPath(Path.Combine(modFolder, 
                            PathHelper.UnixPath(obj.FullName)
                            .Split(modFolder.TrimStart('.'))[1]
                            .TrimStart(Path.AltDirectorySeparatorChar)));

                        list.Add(new ModData
                        {
                            Name = Path.GetFileName(basePath),
                            Description = string.Empty,
                            Author = string.Empty,
                            Version = string.Empty,
                            Hash = identifier,
                            BasePath = basePath,
                            IsEnabled = false,
                            Files = modFiles
                        });
                    }
                }
            }

            return list;
        }

        public static void Enable(GameType type, string identifier, bool isEnabled)
        {
            List<ModData> list = Deserialize(type);

            ModData enabledMod = Find(list, identifier);
            enabledMod.IsEnabled = isEnabled;

            if (isEnabled)
            {
                list = PakDataPatch.Patch(list);
                Install(type, enabledMod);
            }
            else
            {
                Uninstall(type, enabledMod);
                list = PakDataPatch.Patch(list);
            }

            Save(type, list);
        }

        private static void Install(GameType type, ModData mod)
        {
            if (Directory.Exists(SettingsManager.GetGamePath(type)))
            {
                LogBase.Info($"[MODMANAGER] Attempting to install mod: {mod.Name}.");
                foreach (ModFile file in mod.Files)
                {
                    if (!Path.GetFileName(file.SourcePath).Contains(Constants.MOD_INFO_FILE))
                    {
                        FileStreamHelper.CopyFile(file.SourcePath, file.InstallPath, false);
                    }
                }
            }
        }

        private static void Uninstall(GameType type, ModData mod)
        {
            if (Directory.Exists(SettingsManager.GetGamePath(type)))
            {
                LogBase.Info($"[MODMANAGER] Attempting to uninstall mod: {mod.Name}.");
                foreach (ModFile file in mod.Files)
                {
                    if (File.Exists(file.InstallPath))
                    {
                        LogBase.Info($"[MODMANAGER] Removing file: {file.InstallPath}.");
                        File.Delete(file.InstallPath);
                    }
                }

                LogBase.Info($"[MODMANAGER] Cleaning folder: {SettingsManager.GetGamePath(type)}.");
                FileStreamHelper.DeleteEmptyDirectories(SettingsManager.GetGamePath(type));
            }
        }

        public static void Delete(GameType type, string identifier)
        {
            List<ModData> list = Deserialize(type);
            ModData mod = Find(list, identifier);
            LogBase.Info($"[MODMANAGER] Attempting to delete mod: {mod.Name}.");

            foreach (ModFile file in mod.Files)
            {
                if (File.Exists(file.InstallPath))
                {
                    LogBase.Info($"[MODMANAGER] Removing file: {file.InstallPath}.");
                    File.Delete(file.InstallPath);
                }

                if (File.Exists(file.SourcePath))
                {
                    LogBase.Info($"[MODMANAGER] Removing file: {file.SourcePath}.");
                    File.Delete(file.SourcePath);
                }
            }

            list.Remove(Find(list, identifier));
            Save(type, list);
        }

        public static bool IsPatchable(GameType type, string identifier)
        {
            List<ModData> list = Deserialize(type);
            ModData mod = Find(list, identifier);

            if (PakDataPatch.HasNativesFolder(mod.BasePath))
            {
                return true;
            }

            return false;
        }

        public static void Patch(GameType type, string identifier)
        {
            List<ModData> list = Deserialize(type);
            ModData mod = Find(list, identifier);
            IniDataParser parser = new();
            string modInfoName = string.Empty;

            if (Directory.Exists(mod.BasePath))
            {
                string patchedModDir = mod.BasePath + "PakPatch";
                if (Directory.Exists(patchedModDir))
                    Directory.Delete(patchedModDir, true);

                Directory.CreateDirectory(patchedModDir);
                if (File.Exists(Path.Combine(mod.BasePath, Constants.MOD_INFO_FILE)))
                {
                    byte[] infoBytes = FileStreamHelper.ReadFile(Path.Combine(mod.BasePath, Constants.MOD_INFO_FILE), false);
                    string infoData = FileStreamHelper.UnkBytesToStr(infoBytes);
                    IniData modIni = parser.Parse(infoData);
                    PropertyCollection modInfo = modIni["modinfo"];
                    modInfoName = modInfo["name"];

                    FileStreamHelper.CopyFile(Path.Combine(mod.BasePath, Constants.MOD_INFO_FILE), Path.Combine(patchedModDir, Constants.MOD_INFO_FILE), false);
                }

                if (File.Exists(Path.Combine(patchedModDir, Constants.MOD_INFO_FILE)))
                {
                    byte[] infoBytes = FileStreamHelper.ReadFile(Path.Combine(patchedModDir, Constants.MOD_INFO_FILE), false);
                    string infoData = FileStreamHelper.UnkBytesToStr(infoBytes).Replace(modInfoName, modInfoName + " (PakPatch)");
                    FileStreamHelper.WriteFile(patchedModDir, Constants.MOD_INFO_FILE, infoData, false);
                }

                RisePakPatch.ProcessDirectory(mod.BasePath, Path.Combine(patchedModDir, PathHelper.MakeValid(mod.Name).Replace(" ", "") + ".pak"));
            }
        }
    }
}
