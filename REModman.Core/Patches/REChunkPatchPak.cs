using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using REModman.Utils;
using REModman.Configuration.Enums;
using REModman.Configuration.Structs;

namespace REModman.Patches
{
    public class REChunkPatchPak
    {
        public static string CHUNK_PATCH_PAK_TEMPLATE = "re_chunk_000.pak.patch_<REPLACE>.pak";

        public static bool IsREChunkPatchPak(GameType type, string file)
        {
            bool isPak = false;

            if (type == GameType.MonsterHunterRise || type == GameType.MonsterHunterWorld)
            {
                if (file.Contains("re_chunk_") && file.Contains("pak.patch") && file.Contains(".pak"))
                {
                    isPak = true;
                }
            }

            return isPak;
        }

        public static List<ModData> Patch(string installPath, List<ModData> modData)
        {
            List<ModData> installList = new List<ModData>();

            for (int i = 0; i < modData.Count; i++)
            {
                ModData tempMod = modData[i];
                string replacer = CHUNK_PATCH_PAK_TEMPLATE.Replace("<REPLACE>", i.ToString("D3"));

                foreach (ModFile file in tempMod.ModFiles)
                {
                    string path = "." + file.LocalFilePath.Substring(StringHelper.IndexOfNth(file.LocalFilePath, "\\", 1));
                    file.LocalFilePath = file.LocalFilePath.Replace(tempMod.Path, replacer);
                    file.InstallAbsolutePath = PathHelper.GetAbsolutePath(Path.Combine(installPath, path)).Replace(tempMod.Path, replacer);
                    file.InstallRelativePath = Path.Combine(installPath, path).Replace(tempMod.Path, replacer);
                }

                tempMod.Path = replacer;
                installList.Add(tempMod);
            }

            return installList;
        }
    }
}