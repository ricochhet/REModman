using REMod.Core.Configuration.Enums;
using REMod.Core.Configuration.Structs;
using REMod.Core.Logger;
using REMod.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace REMod.Core.Patches
{
    public class PakDataPatch
    {
        public static readonly GameType[] ValidGameTypes = new GameType[]
        {
            GameType.MonsterHunterRise
        };

        private static bool IsPatchPak(string String) => String.Contains("re_chunk_") && String.Contains("pak.patch") && String.Contains(".pak");
        private static string PatchString(int value) => "re_chunk_000.pak.patch_<REPLACE>.pak".Replace("<REPLACE>", value.ToString("D3"));

        public static bool NativesExists(string directory)
        {
            if (Directory.Exists(Path.Combine(directory, "natives")))
            {
                return true;
            }

            return false;
        }

        public static bool IsNatives(string directory)
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

        public static bool IsValidPak(string directory)
        {
            if (Path.GetExtension(directory) == ".pak")
            {
                if (IsPatchPak(directory) || directory.Contains("re_chunk_000"))
                {
                    LogBase.Error($"Invalid path \"{directory}\" was found.");
                    return false;
                }

                return true;
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
                    if (Path.GetExtension(file.SourcePath) == ".pak" && !IsPatchPak(file.SourcePath) && !file.SourcePath.Contains("re_chunk_000") && mod.IsEnabled)
                    {
                        string path = file.InstallPath.Replace(Path.GetFileName(file.InstallPath), PatchString(startIndex));
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