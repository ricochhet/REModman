using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Cryptography;
using IniParser;
using IniParser.Model;
using REFramework.Extensions;

namespace REFramework.Internal
{
    public class Constants
    {
        public static string MOD_LIST = "modlist.json";
        public static string MOD_INFO = "modinfo.ini";
        public static string RE_ENGINE = "re_chunk_000.pak.patch_000.pak";
    }
}