using REModman.Configuration;
using REModman.Configuration.Enums;
using REModman.Configuration.Structs;
using REModman.Logger;
using REModman.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace REModman.Patches
{
    public class REEnginePatcher
    {
        private static readonly string[] InvalidFiles = new string[]
        {
            "re_chunk_000.pak",
            "re_chunk_000.pak.patch_000.pak",
            "re_chunk_000.pak.patch_001.pak",
            "MonsterHunterRise.exe"
        };

        private static bool IsPakFormat(string String) => String.Contains("re_chunk_") && String.Contains("pak.patch") && String.Contains(".pak");
        private static bool IsSafe(string value)
        {
            if (InvalidFiles.Contains(value))
            {
                LogBase.Error($"[REENGINEPATCHER] DANGEROUS FILE FOUND: {value}.");
                return false;
            }

            return true;
        }

        public static string Patch(int value) => "re_chunk_000.pak.patch_<REPLACE>.pak".Replace("<REPLACE>", value.ToString("D3"));

        public static bool IsValid(GameType type, string String)
        {
            if (type == GameType.MonsterHunterRise || type == GameType.MonsterHunterWorld)
            {
                string path = PathHelper.GetFirstDirectory(String)[3];
                if (!IsSafe(path)) return false;

                if (path == "natives")
                {
                    LogBase.Info($"[REENGINEPATCHER] Valid path \"{path}\" was found.");
                    return true;
                }
                else if (Path.GetExtension(path) == ".pak")
                {
                    if (IsPakFormat(path) || String.Contains("re_chunk_000"))
                    {
                        LogBase.Warn($"[REENGINEPATCHER] Invalid path \"{path}\" was found.");
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }

        public static bool IsPatchable(GameType type, string String)
        {
            if (type == GameType.MonsterHunterRise || type == GameType.MonsterHunterWorld)
            {
                if ((String.Contains("re_chunk_000") && String.Contains("patch_") && String.Contains(".pak")) || String.Contains("re_chunk_000"))
                {
                    LogBase.Info($"[REENGINEPATCHER] {String} is patchable.");
                    return true;
                }
            }

            return false;
        }

        public static List<ModData> Patch(GameType type, List<ModData> list)
        {
            int i = 2;
            foreach (ModData mod in list)
            {
                if (mod.IsEnabled)
                {
                    LogBase.Info($"[REENGINEPATCHER] Patch found enabled mod {mod.Name}.");
                    foreach (ModFile file in mod.Files)
                    {
                        if (!Path.GetFileName(file.SourcePath).Contains(Constants.MOD_INFO_FILE) && Path.GetExtension(file.SourcePath) == ".pak" && !IsPakFormat(file.SourcePath) && !file.SourcePath.Contains("re_chunk_000"))
                        {
                            string path = file.InstallPath.Replace(Path.GetFileName(file.InstallPath), Patch(i));

                            if (IsSafe(path))
                            {
                                if (File.Exists(file.InstallPath))
                                {
                                    LogBase.Info($"[REENGINEPATCHER] Patching file for {mod.Name} with patch {path}.");
                                    File.Move(file.InstallPath, path);
                                }

                                LogBase.Info($"[REENGINEPATCHER] Patching index for {mod.Name} with patch {path}.");
                                file.InstallPath = path;
                                i++;
                            }
                        }
                        else if (!Path.GetFileName(file.SourcePath).Contains(Constants.MOD_INFO_FILE) && IsPatchable(type, file.SourcePath))
                        {
                            string path = file.InstallPath.Replace(Path.GetFileName(file.InstallPath), Patch(i));

                            if (IsSafe(path))
                            {
                                if (File.Exists(file.InstallPath))
                                {
                                    LogBase.Info($"[REENGINEPATCHER] Patching file for {mod.Name} with patch {path}.");
                                    File.Move(file.InstallPath, path);
                                }

                                LogBase.Info($"[REENGINEPATCHER] Patching index for {mod.Name} with patch {path}.");
                                file.InstallPath = path;
                                i++;
                            }
                        }
                    }
                }
                else
                {
                    LogBase.Info($"[REENGINEPATCHER] Patch found disabled mod {mod.Name}.");
                    foreach (ModFile file in mod.Files)
                    {
                        if (!Path.GetFileName(file.SourcePath).Contains(Constants.MOD_INFO_FILE) && IsPatchable(type, file.InstallPath))
                        {
                            LogBase.Info($"[REENGINEPATCHER] Unpatching index for {mod.Name} with patch {file.SourcePath}.");
                            file.InstallPath = file.DefaultInstallPath;
                        }
                    }
                }
            }

            return list;
        }

        public static List<ModData> DebugPatch(GameType type, List<ModData> list)
        {
            int startIndex = 2;

            foreach (ModData mod in list)
            {
                foreach (ModFile file in mod.Files)
                {
                    if (!Path.GetFileName(file.SourcePath).Contains(Constants.MOD_INFO_FILE) && Path.GetExtension(file.SourcePath) == ".pak" && !IsPakFormat(file.SourcePath) && !file.SourcePath.Contains("re_chunk_000") && mod.IsEnabled)
                    {
                        string path = file.InstallPath.Replace(Path.GetFileName(file.InstallPath), Patch(startIndex));

                        if (IsSafe(path))
                        {
                            File.Delete(file.InstallPath);
                            file.InstallPath = path;
                            FileStreamHelper.CopyFile(file.SourcePath, path, false);
                            startIndex++;
                        }
                    }
                }
            }

            return list;
        }
    }
}