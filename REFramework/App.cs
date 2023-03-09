using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REFramework.Internal;
using REFramework.Extensions;

namespace REFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(EFileStream.GetProcPath(EFileStream.GetProcIdFromName("Discord")));
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