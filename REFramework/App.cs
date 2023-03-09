using System;
using REFramework.Data;
using REFramework.Internal;
using System.Collections.Generic;

namespace REFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            List<ModData> modData = ModManager.IndexModDirectory("./Mods/");
            ModManager.WriteModIndex("./Mods/", modData);
            // Console.WriteLine(EFileStream.GetProcPath(EFileStream.GetProcIdFromName("Discord")));
            // List<ModData> mods = ModManager.FindAllMods("./Mods/", Constants.RE_ENGINE);
            // ModManager.CreateModIndex("./Mods/", mods);
            // ModManager.ListMods(mods);
            //mm.CreateModList();
            // mm.ListMods();

            // ModDataFormat.Read("../../../Mods/MyMod01.remm");
            // ArgParser cl = new ArgParser(args);
            // FileFormat.Read("../../../Mods/mod_list.remm");
        }
    }
}