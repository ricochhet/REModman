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
        public static bool IsPatchable(GameType type, ModData mod)
        {
            bool isPak = false;

            if (type == GameType.MonsterHunterRise || type == GameType.MonsterHunterWorld)
            {
                foreach (ModFile file in mod.Files)
                {
                    if (file.SourcePath.Contains("re_chunk_") && file.SourcePath.Contains("pak.patch") && file.SourcePath.Contains(".pak"))
                    {
                        isPak = true;
                        break;
                    }
                }
            }

            return isPak;
        }

        public static bool IsPatchable(GameType type, string file)
        {
            if (type == GameType.MonsterHunterRise || type == GameType.MonsterHunterWorld)
            {
                if (file.Contains("re_chunk_") && file.Contains("pak.patch") && file.Contains(".pak"))
                {
                    return true;
                }
            }

            return false;
        }

        public static string Patch(int value)
        {
            return "re_chunk_000.pak.patch_<REPLACE>.pak".Replace("<REPLACE>", value.ToString("D3"));
        }
    }
}