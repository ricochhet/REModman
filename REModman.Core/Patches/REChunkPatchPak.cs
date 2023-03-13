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
        public static string CHUNK_PATCH_PAK_DEFAULT = "re_chunk_000.pak.patch_000.pak";
        public static string CHUNK_PATCH_PAK_TEMPLATE = "re_chunk_000.pak.patch_<REPLACE>.pak";

        public static bool IsPatchable(GameType type, string file)
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

        public static string Patch(int value)
        {
            return CHUNK_PATCH_PAK_TEMPLATE.Replace("<REPLACE>", value.ToString("D3"));
        }
    }
}