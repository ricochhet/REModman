using REModman.Configuration;
using REModman.Configuration.Enums;
using REModman.Configuration.Structs;
using REModman.Logger;
using REModman.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace REModman.Patches
{
    public class PakDataPatch
    {
        private static readonly string[] InvalidFiles = new string[]
        {
            "re_chunk_000.pak",
            "re_chunk_000.pak.patch_000.pak",
            "re_chunk_000.pak.patch_001.pak",
            "MonsterHunterRise.exe"
        };

        public static readonly GameType[] ValidGameTypes = new GameType[]
        {
            GameType.MonsterHunterRise
        };

        private static bool IsPakFormat(string String) => String.Contains("re_chunk_") && String.Contains("pak.patch") && String.Contains(".pak");
        private static string Patch(int value) => "re_chunk_000.pak.patch_<REPLACE>.pak".Replace("<REPLACE>", value.ToString("D3"));

        private static bool IsSafe(string value)
        {
            if (InvalidFiles.Contains(value))
            {
                LogBase.Error($"[REENGINEPATCHER] DANGEROUS FILE FOUND: {value}.");
                return false;
            }

            return true;
        }

        public static bool HasNativesFolder(string directory)
        {
            if (directory == "natives")
            {
                return true;
            }

            return false;
        }

        public static string GetNativesFile(FileInfo path)
        {
            return "natives" + path.FullName.Split("natives")[1];
        }

        public static bool HasValidPak(string directory)
        {
            if (Path.GetExtension(directory) == ".pak")
            {
                if (IsPakFormat(directory) || directory.Contains("re_chunk_000"))
                {
                    LogBase.Warn($"[REENGINEPATCHER] Invalid path \"{directory}\" was found.");
                    return false;
                }

                return true;
            }

            return false;
        }

        public static bool IsValid(GameType type, string directory)
        {
            if (ValidGameTypes.Contains(type))
            {
                string directoryName = PathHelper.GetFirstDirectory(directory)[3];
                if (!IsSafe(directoryName)) return false;

                if (directoryName == "natives")
                {
                    LogBase.Info($"[REENGINEPATCHER] Valid path \"{directoryName}\" was found.");
                    return true;
                }
                else if (Path.GetExtension(directoryName) == ".pak")
                {
                    if (IsPakFormat(directoryName) || directory.Contains("re_chunk_000"))
                    {
                        LogBase.Warn($"[REENGINEPATCHER] Invalid path \"{directoryName}\" was found.");
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }

        public static List<ModData> Patch(List<ModData> list)
        {
            int startIndex = 2;

            foreach (ModData mod in list)
            {
                foreach (ModFile file in mod.Files)
                {
                    if (Path.GetExtension(file.SourcePath) == ".pak")
                    {
                        if (File.Exists(file.InstallPath))
                        {
                            File.Delete(file.InstallPath);
                        }
                    }
                }
            }

            foreach (ModData mod in list)
            {
                foreach (ModFile file in mod.Files)
                {
                    if (!Path.GetFileName(file.SourcePath).Contains(Constants.MOD_INFO_FILE) && Path.GetExtension(file.SourcePath) == ".pak" && !IsPakFormat(file.SourcePath) && !file.SourcePath.Contains("re_chunk_000") && mod.IsEnabled)
                    {
                        string path = file.InstallPath.Replace(Path.GetFileName(file.InstallPath), Patch(startIndex));

                        FileStreamHelper.CopyFile(file.SourcePath, path, false);
                        file.InstallPath = path;
                        startIndex++;
                    }
                }
            }

            return list;
        }
    }
}