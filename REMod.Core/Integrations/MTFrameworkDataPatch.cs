using System.IO;

namespace REMod.Core.Integrations
{
    public class MTFrameworkDataPatch
    {
        public static bool IsNatives(string directory) => directory == "nativePC";
        public static bool HasNatives(string directory) => Directory.Exists(Path.Combine(directory, "nativePC"));
        public static string GetRelativeFromNatives(FileInfo path) => "nativePC" + path.FullName.Split("nativePC")[1];
    }
}