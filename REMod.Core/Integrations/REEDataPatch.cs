using REMod.Core.Configuration.Structs;
using REMod.Core.Logger;
using REMod.Core.Providers;
using System;
using System.Collections.Generic;
using System.IO;

namespace REMod.Core.Integrations
{
    public class REEDataPatch
    {
        private static bool IsFilePatchPak(string String) => String.Contains("re_chunk_") && String.Contains("pak.patch") && String.Contains(".pak");
        private static string FixPatchPakFileName(int value) => "re_chunk_000.pak.patch_<REPLACE>.pak".Replace("<REPLACE>", value.ToString("D3"));

        public static bool IsNatives(string directory) => directory == "natives";
        public static bool HasNatives(string directory) => Directory.Exists(Path.Combine(directory, "natives"));
        public static string GetRelativeFromNatives(FileInfo path) => "natives" + path.FullName.Split("natives")[1];

        public static bool IsREF(string directory) => directory == "reframework";
        public static string GetRelativeFromREF(FileInfo path) => "reframework" + path.FullName.Split("reframework")[1];

        public static bool IsValidPatchPak(string directory)
        {
            if (Path.GetExtension(directory) == ".pak")
            {
                if (IsFilePatchPak(directory) || directory.Contains("re_chunk_000"))
                {
                    LogBase.Error($"Invalid path \"{directory}\" was found.");
                    return false;
                }

                return true;
            }

            return false;
        }

        public static bool HasValidPatchPaks(ModData mod)
        {
            bool hasValid = false;

            foreach (ModFile file in mod.Files)
            {
                if (IsValidPatchPak(file.SourcePath))
                {
                    hasValid = true;
                    break;
                }
            }

            return hasValid;
        }

        public static string InstallPath(FileInfo fileInfo)
        {
            if (IsNatives(fileInfo.Name))
            {
                return GetRelativeFromNatives(fileInfo);
            }
            else if (IsREF(fileInfo.Name))
            {
                return GetRelativeFromREF(fileInfo);
            }

            return fileInfo.FullName;
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
                    if (Path.GetExtension(file.SourcePath) == ".pak" && !IsFilePatchPak(file.SourcePath) && !file.SourcePath.Contains("re_chunk_000") && mod.IsEnabled)
                    {
                        string path = file.InstallPath.Replace(Path.GetFileName(file.InstallPath), FixPatchPakFileName(startIndex));
                        FsProvider.CopyFile(file.SourcePath, path);
                        file.InstallPath = path;
                        startIndex++;
                    }
                }
            }

            return list;
        }
    }
}