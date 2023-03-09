using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using REFramework.Utils;
using REFramework.Data;
using REFramework.Configuration.Enums;

namespace REFramework.Addons
{
    public class REChunkPatchPak
    {
        public static string CHUNK_PATCH_PAK_TEMPLATE = "re_chunk_000.pak.patch_<REPLACE>.pak";

        public static bool IsREChunkPatchPak(string file)
        {
            bool isPak = false;

            if (
                file.Contains("re_chunk_") &&
                file.Contains("pak.patch") &&
                file.Contains(".pak")
                )
            {
                isPak = true;
            }

            return isPak;
        }

        public static List<ModData> InterceptModInstaller(List<ModData> modData)
        {
            List<ModData> installList = new List<ModData>();

            for (int i = 0; i < modData.Count; i++)
            {
                ModData tempMod = modData[i];
                string replacer = CHUNK_PATCH_PAK_TEMPLATE.Replace("<REPLACE>", i.ToString("D3"));

                foreach (ModFile file in tempMod.ModFiles)
                {
                    file.LocalFilePath = file.LocalFilePath.Replace(tempMod.Path, replacer);
                    file.SourceAbsolutePath = file.SourceAbsolutePath.Replace(tempMod.Path, replacer);
                    file.SourceRelativePath = file.SourceRelativePath.Replace(tempMod.Path, replacer);
                }

                tempMod.Path = replacer;
                installList.Add(tempMod);
            }

            return installList;
        }

        public static bool IsValidGameType(GameType type)
        {
            bool IsValidGameType = false;

            if (type == GameType.MonsterHunterRise || type == GameType.MonsterHunterWorld)
            {
                IsValidGameType = true;
            }

            return IsValidGameType;
        }
    }
}