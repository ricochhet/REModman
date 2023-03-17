using REMod.Core.Configuration;
using REMod.Core.Configuration.Enums;
using REMod.Core.Configuration.Structs;
using REMod.Core.Integrations;
using REMod.Core.Logger;
using REMod.Core.Utils;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace REMod.Core.Internal
{
    public class ModManager
    {
        public static List<ModData> Deserialize(GameType type)
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

        public static ModData Find(List<ModData> list, string identifier) => list.Find(i => i.Hash == identifier);

        public static void Save(GameType type, List<ModData> list)
        {
            FileStreamHelper.WriteFile(Path.Combine(Constants.DATA_FOLDER, EnumSwitch.GetModFolder(type)), Constants.MOD_INDEX_FILE, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }), false);
        }

        public static List<ModData> Index(GameType type)
        {
            List<ModData> list = Deserialize(type);
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
                        if (PakDataPatch.IsNatives(directory.Name))
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
                        if (PakDataPatch.IsValidPak(file.FullName))
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
                            BasePath = obj.FullName,
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
                        File.Delete(file.InstallPath);
                    }
                }

                LogBase.Info($"Cleaning folder: {SettingsManager.GetGamePath(type)}.");
                FileStreamHelper.DeleteEmptyDirectories(SettingsManager.GetGamePath(type));
            }
        }

        public static void Delete(GameType type, string identifier)
        {
            List<ModData> list = Deserialize(type);
            ModData mod = Find(list, identifier);

            Uninstall(type, mod);
            list = PakDataPatch.Patch(list);
            LogBase.Info($"Attempting to delete mod: {mod.Name}.");

            foreach (ModFile file in mod.Files)
            {
                if (File.Exists(file.SourcePath))
                {
                    LogBase.Info($"Removing file: {file.SourcePath}.");
                    File.Delete(file.SourcePath);
                }
            }

            list.Remove(Find(list, mod.Hash));
            FileStreamHelper.DeleteEmptyDirectories(Constants.MODS_FOLDER);
            Save(type, list);
        }
    }
}
