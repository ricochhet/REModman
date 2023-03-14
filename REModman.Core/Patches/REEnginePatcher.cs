using REModman.Configuration.Enums;
using REModman.Utils;
using System;
using System.IO;

namespace REModman.Patches
{
    public class REEnginePatcher
    {
        private static bool IsPakFormat(string String) => String.Contains("re_chunk_") && String.Contains("pak.patch") && String.Contains(".pak");
        public static string Patch(int value) => "re_chunk_000.pak.patch_<REPLACE>.pak".Replace("<REPLACE>", value.ToString("D3"));

        public static bool IsValid(GameType type, string String)
        {
            if (type == GameType.MonsterHunterRise || type == GameType.MonsterHunterWorld)
            {
                string path = PathHelper.GetFirstDirectory(String)[3];

                if (path == "natives")
                {
                    return true;
                }
                else if (Path.GetExtension(path) == ".pak")
                {
                    if (IsPakFormat(path) || String.Contains("re_chunk_000"))
                        return false;
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
                    return true;
                }
            }

            return false;
        }
    }
}