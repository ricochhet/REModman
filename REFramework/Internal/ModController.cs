using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using REFramework.Utils;
using REFramework.Data;
using REFramework.Patches;
using REFramework.Configuration;
using REFramework.Configuration.Enums;

namespace REFramework.Internal
{
    public class ModController
    {
        public static List<ModData> UninstallMods(List<ModData> modData, Dictionary<string, string> selectedMods)
        {
            List<ModData> installList = new List<ModData>();

            if (modData.Count != 0)
            {
                foreach (ModData mod in modData)
                {
                    installList.Add(mod);
                    Logger.Log(LogType.Info, new string[]
                    {
                        $"Added mod to List<ModData> with name \"{mod.Name}\" in function UninstallMod()"
                    });

                    if (selectedMods.ContainsKey(mod.Name))
                    {
                        if (selectedMods[mod.Name] == mod.Guid)
                        {
                            foreach (ModFile file in mod.ModFiles)
                            {
                                File.Delete(file.InstallAbsolutePath);
                                Logger.Log(LogOpType.Delete, new string[]
                                {
                                    $"{file.LocalFilePath}, {file.InstallAbsolutePath}"
                                });
                            }

                            installList.Remove(mod);
                            Logger.Log(LogType.Info, new string[]
                            {
                                $"\"{mod.Name}\" was remove from List<ModData> in function UninstallMod()"
                            });
                        }
                        else
                        {
                            Logger.Log(LogType.Info, new string[]
                            {
                                $"Matching Guid for \"{mod.Name}\" was not found in function UninstallMod()"
                            });
                        }
                    }
                    else
                    {
                        Logger.Log(LogType.Info, new string[]
                        {
                            $"Dictionary did not contain \"{mod.Name}\" in function UninstallMod()"
                        });
                    }
                }
            }
            else
            {
                Logger.Log(LogType.Error, new string[]
                {
                    $"List<ModData> was empty in function UninstallMod()"
                });
            }

            return installList;
        }

        public static List<ModData> SelectMods(List<ModData> modData, Dictionary<string, string> selectedMods)
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
                            Logger.Log(LogType.Info, new string[]
                            {
                                $"\"{mod.Name}\" was added to List<ModData> in function SelectMods()"
                            });
                        }
                        else
                        {
                            Logger.Log(LogType.Info, new string[]
                            {
                                $"Matching Guid for \"{mod.Name}\" was not found in function SelectMods()"
                            });
                        }
                    }
                    else
                    {
                        Logger.Log(LogType.Info, new string[]
                        {
                            $"Dictionary did not contain \"{mod.Name}\" in function UninstallMod()"
                        });
                    }
                }
            }
            else
            {
                Logger.Log(LogType.Error, new string[]
                {
                    $"List<ModData> was empty in function SelectMods()"
                });
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
                            FileStreamHelper.CopyFile(file.SourceRelativePath, Path.Combine(installPath, path), false);
                        }
                    }
                }

                if (REChunkPatchPak.IsValidGameType(type))
                {
                    if (intercepted.Count != 0)
                    {
                        List<ModData> temp = REChunkPatchPak.InterceptModInstaller(intercepted);

                        foreach (ModData mod in modData)
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