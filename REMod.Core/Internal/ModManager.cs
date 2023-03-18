using REMod.Core.Configuration;
using REMod.Core.Configuration.Enums;
using REMod.Core.Configuration.Structs;
using REMod.Core.Integrations;
using REMod.Core.Logger;
using REMod.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace REMod.Core.Internal
{
    public class ModManager
    {
        private static void Save(GameType type, List<ModData> list)
        {
            List<ModData> sorted = list.OrderBy(i => i.LoadOrder).ToList();
            FileStreamHelper.WriteFile(Path.Combine(Constants.DATA_FOLDER, EnumSwitch.GetModFolder(type)), Constants.MOD_INDEX_FILE, JsonSerializer.Serialize(sorted, new JsonSerializerOptions { WriteIndented = true }), false);
        }

        public static List<ModData> DeserializeIndex(GameType type)
        {
            List<ModData> list = new();
            string dataFolder = Path.Combine(Constants.DATA_FOLDER, EnumSwitch.GetModFolder(type));

            if (Directory.Exists(dataFolder))
            {
                if (File.Exists(Path.Combine(dataFolder, Constants.MOD_INDEX_FILE)))
                {
                    byte[] bytes = FileStreamHelper.ReadFile(Path.Combine(dataFolder, Constants.MOD_INDEX_FILE), true);
                    string file = FileStreamHelper.UnkBytesToStr(bytes);
                    list = JsonSerializer.Deserialize<List<ModData>>(file);
                }
            }

            return list;
        }

        public static ModData Find(List<ModData> list, string identifier) => list.Find(i => i.Hash == identifier);


        public static void SaveByHashes(GameType type, List<ModData> list)
        {
            List<ModData> deserializedList = DeserializeIndex(type);
            List<ModData> listDiff = list.Where(p => !deserializedList.Any(l => p.Hash == l.Hash)).ToList();

            if (listDiff.Count != 0)
            {
                Save(type, list);
            }
        }


        public static void SaveByModified(GameType type, List<ModData> list)
        {
            List<ModData> deserializedList = DeserializeIndex(type);
            List<ModData> listDiff = list.Where(p => 
                !deserializedList.Any(l => p.Hash == l.Hash) || 
                deserializedList.Any(l => p.IsEnabled != l.IsEnabled) || 
                deserializedList.Any(l => p.LoadOrder != l.LoadOrder)
            ).ToList();

            if (listDiff.Count != 0)
            {
                Save(type, list);
            }
        }

        public static List<ModData> Index(GameType type)
        {
            List<ModData> list = DeserializeIndex(type);
            string gamePath = SettingsManager.GetGamePath(type);
            string baseModDir = Path.Combine(Constants.MODS_FOLDER, EnumSwitch.GetModFolder(type));

            if (Directory.Exists(baseModDir))
            {
                DirectoryInfo[] modDirs = new DirectoryInfo(baseModDir).GetDirectories("*.*", SearchOption.TopDirectoryOnly);

                foreach (DirectoryInfo obj in modDirs)
                {
                    string modHash = string.Empty;
                    List<ModFile> modFiles = new();

                    DirectoryInfo[] directories = new DirectoryInfo(obj.FullName).GetDirectories("*.*", SearchOption.TopDirectoryOnly);
                    foreach (DirectoryInfo directory in directories)
                    {
                        if (REEDataPatch.IsNatives(directory.Name))
                        {
                            FileInfo[] nativeFiles = new DirectoryInfo(directory.FullName).GetFiles("*.*", SearchOption.AllDirectories);

                            foreach (FileInfo file in nativeFiles)
                            {
                                if (FileCheck.IsSafe(file.Name))
                                {
                                    string fileHash = CryptoHelper.FileHash.Sha256(file.FullName);
                                    modHash += fileHash;

                                    modFiles.Add(new ModFile
                                    {
                                        InstallPath = Path.Combine(gamePath, REEDataPatch.GetNativesFile(file)),
                                        SourcePath = file.FullName,
                                        Hash = fileHash,
                                    });
                                }
                            }
                        }
                        else if (REEDataPatch.IsREFMod(directory.Name))
                        {
                            FileInfo[] refFiles = new DirectoryInfo(directory.FullName).GetFiles("*.*", SearchOption.AllDirectories);

                            foreach (FileInfo file in refFiles)
                            {
                                if (FileCheck.IsSafe(file.Name))
                                {
                                    string fileHash = CryptoHelper.FileHash.Sha256(file.FullName);
                                    modHash += fileHash;

                                    modFiles.Add(new ModFile
                                    {
                                        InstallPath = Path.Combine(gamePath, REEDataPatch.GetREFFile(file)),
                                        SourcePath = file.FullName,
                                        Hash = fileHash,
                                    });
                                }
                            }
                        }
                    }

                    FileInfo[] files = new DirectoryInfo(obj.FullName).GetFiles("*.*", SearchOption.TopDirectoryOnly);
                    foreach (FileInfo file in files)
                    {
                        if (REEDataPatch.IsValidPak(file.FullName) && FileCheck.IsSafe(file.Name))
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
                        string basePath = PathHelper.UnixPath(Path.Combine(baseModDir, 
                            PathHelper.UnixPath(obj.FullName)
                            .Split(baseModDir.TrimStart('.'))[1]
                            .TrimStart(Path.AltDirectorySeparatorChar)));

                        list.Add(new ModData
                        {
                            Name = Path.GetFileName(basePath),
                            Hash = identifier,
                            LoadOrder = 0,
                            BasePath = obj.FullName,
                            IsEnabled = false,
                            Files = modFiles
                        });
                    }
                }
            }

            return list.OrderBy(i => i.LoadOrder).ToList();
        }

        public static void SetLoadOrder(GameType type, string identifier, int value)
        {
            List<ModData> list = DeserializeIndex(type);
            ModData mod = Find(list, identifier);

            if (mod == null)
            {
                return;
            }

            if (mod.LoadOrder == value)
            {
                return;
            }

            mod.LoadOrder = value;
            SaveByModified(type, list);
        }

        public static int GetLoadOrder(GameType type, string identifier)
        {
            List<ModData> list = DeserializeIndex(type);
            ModData mod = Find(list, identifier);

            if (mod == null)
            {
                return 0;
            }

            return mod.LoadOrder;
        }

        public static void Enable(GameType type, string identifier, bool isEnabled)
        {
            List<ModData> list = DeserializeIndex(type);
            ModData mod = Find(list, identifier);

            if (mod == null)
            {
                return;
            }

            if (mod.IsEnabled == isEnabled)
            {
                return;
            }

            mod.IsEnabled = isEnabled;

            if (isEnabled)
            {
                list = REEDataPatch.Patch(list);
                Install(type, mod);
            }
            else
            {
                Uninstall(type, mod);
                list = REEDataPatch.Patch(list);
            }

            SaveByModified(type, list);
        }

        private static void Install(GameType type, ModData mod)
        {
            if (Directory.Exists(SettingsManager.GetGamePath(type)))
            {
                LogBase.Info($"Attempting to install mod: {mod.Name}.");
                foreach (ModFile file in mod.Files)
                {
                    FileStreamHelper.CopyFile(file.SourcePath, file.InstallPath, false);
                }
            }
        }

        private static void Uninstall(GameType type, ModData mod)
        {
            if (Directory.Exists(SettingsManager.GetGamePath(type)))
            {
                LogBase.Info($"Attempting to uninstall mod: {mod.Name}.");
                foreach (ModFile file in mod.Files)
                {
                    if (File.Exists(file.InstallPath))
                    {
                        LogBase.Info($"Removing file: {file.InstallPath}.");

                        try
                        {
                            File.Delete(file.InstallPath);
                        }
                        catch (Exception e)
                        {
                            LogBase.Error($"Failed to remove file: {file.InstallPath}.");
                            LogBase.Error(e.ToString());
                        }
                    }
                }

                LogBase.Info($"Cleaning folder: {SettingsManager.GetGamePath(type)}.");
                FileStreamHelper.DeleteEmptyDirectories(SettingsManager.GetGamePath(type));
            }
        }

        public static void Delete(GameType type, string identifier)
        {
            List<ModData> list = DeserializeIndex(type);
            ModData mod = Find(list, identifier);

            if (mod == null)
            {
                return;
            }

            Enable(type, mod.Hash, false);
            LogBase.Info($"Attempting to delete mod: {mod.Name}.");

            if (Directory.Exists(mod.BasePath))
            {
                try
                {
                    Directory.Delete(mod.BasePath, true);
                }
                catch (Exception e)
                {
                    LogBase.Error($"Failed to remove directory: {mod.BasePath}.");
                    LogBase.Error(e.ToString());
                }
            }

            list.Remove(Find(list, mod.Hash));
            Save(type, list);

            FileStreamHelper.DeleteEmptyDirectories(Constants.MODS_FOLDER);
        }
    }
}
