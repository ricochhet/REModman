using IniParser;
using IniParser.Model;
using REModman.Configuration;
using REModman.Configuration.Enums;
using REModman.Configuration.Structs;
using REModman.Patches;
using REModman.Utils;
using System;
using System.Collections.Generic;
using System.IO;
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
                    byte[] bytes = FileStreamHelper.ReadFile(Path.Combine(dataFolder, Constants.MOD_INDEX_FILE));
                    string file = FileStreamHelper.UnkBytesToStr(bytes);
                    list = JsonSerializer.Deserialize<List<ModData>>(file);
                }
            }

            return list;
        }

        private static ModData Find(List<ModData> list, string identifier) => list.Find(i => i.Hash == identifier);

        public static void Save(GameType type, List<ModData> list)
        {
            string modFolder = Path.Combine(Constants.DATA_FOLDER, EnumSwitch.GetModFolder(type));
            FileStreamHelper.WriteFile(modFolder, Constants.MOD_INDEX_FILE, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }), false);
        }

        public static List<ModData> Index(GameType type)
        {
            List<ModData> list = new();
            IniDataParser parser = new();

            string gamePath = SettingsManager.GetGamePath(type);
            string modFolder = Path.Combine(Constants.MODS_FOLDER, EnumSwitch.GetModFolder(type));

            if (Directory.Exists(modFolder))
            {
                foreach (string infoFile in FileStreamHelper.GetFiles(modFolder, Constants.MOD_INFO_FILE, false))
                {
                    byte[] infoBytes = FileStreamHelper.ReadFile(infoFile);
                    string infoData = FileStreamHelper.UnkBytesToStr(infoBytes);

                    IniData modIni = parser.Parse(infoData);
                    PropertyCollection modInfo = modIni["modinfo"];
                    
                    string modPath = Path.GetDirectoryName(infoFile);
                    List<ModFile> modFiles = new();
                    bool containsInvalidFiles = false;
                    string modHash = string.Empty;

                    foreach (string modFilePath in FileStreamHelper.GetFiles(modPath, "*.*", false))
                    {
                        string fileHash = CryptoHelper.FileHash.Sha256(modFilePath);
                        modHash += fileHash;

                        string sourcePath = PathHelper.GetAbsolutePath(modFilePath);
                        string installPath = PathHelper.GetAbsolutePath(
                            Path.Combine(gamePath, string.Concat(".", modFilePath.AsSpan(StringHelper.IndexOfNth(modFilePath, "\\", 3)))));

                        if (!Path.GetFileName(sourcePath).Contains(Constants.MOD_INFO_FILE))
                        {
                            if (REEnginePatcher.IsValid(type, modFilePath))
                            {
                                modFiles.Add(new ModFile
                                {
                                    SourcePath = PathHelper.UnixPath(sourcePath),
                                    InstallPath = PathHelper.UnixPath(installPath),
                                    Hash = fileHash,
                                });
                            }
                            else
                            {
                                containsInvalidFiles = true;
                                break;
                            }
                        }
                    }

                    string identifier = CryptoHelper.StringHash.Sha256(modHash);

                    if (!containsInvalidFiles)
                    {
                        list.Add(new ModData
                        {
                            Name = modInfo["name"],
                            Description = modInfo["description"],
                            Author = modInfo["author"],
                            Version = modInfo["version"],
                            Hash = identifier,
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
                Install(type, enabledMod);
                list = Patch(type, list);
            }
            else
            {
                Uninstall(type, enabledMod);
                list = Patch(type, list);
            }

            Save(type, list);
        }

        private static List<ModData> Patch(GameType type, List<ModData> list)
        {
            int i = 0;
            foreach (ModData mod in list) 
            {
                if (mod.IsEnabled)
                {
                    foreach (ModFile file in mod.Files)
                    {
                        if (!Path.GetFileName(file.SourcePath).Contains(Constants.MOD_INFO_FILE) && REEnginePatcher.IsPatchable(type, file.SourcePath))
                        {
                            string path = file.InstallPath.Replace(Path.GetFileName(file.InstallPath), REEnginePatcher.Patch(i));

                            if (File.Exists(file.InstallPath))
                            {
                                File.Move(file.InstallPath, path);
                            }

                            file.InstallPath = path;
                            i++;
                        }
                    }
                }
                else
                {
                    foreach (ModFile file in mod.Files)
                    {
                        if (!Path.GetFileName(file.SourcePath).Contains(Constants.MOD_INFO_FILE) && REEnginePatcher.IsPatchable(type, file.InstallPath))
                        {
                            file.InstallPath = file.SourcePath;
                        }
                    }
                }
            }

            return list;
        }

        private static void Install(GameType type, ModData mod)
        {
            if (Directory.Exists(SettingsManager.GetGamePath(type)))
            {
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
                foreach (ModFile file in mod.Files)
                {
                    if (File.Exists(file.InstallPath))
                    {
                        File.Delete(file.InstallPath);
                    }
                }
            }
        }

        public static void Delete(GameType type, string identifier)
        {
            List<ModData> list = Deserialize(type);
            ModData mod = Find(list, identifier);

            foreach (ModFile file in mod.Files)
            {
                if (File.Exists(file.InstallPath))
                {
                    File.Delete(file.InstallPath);
                }

                if (File.Exists(file.SourcePath))
                {
                    File.Delete(file.SourcePath);
                }
            }

            list.Remove(Find(list, identifier));
            Save(type, list);
        }
    }
}
