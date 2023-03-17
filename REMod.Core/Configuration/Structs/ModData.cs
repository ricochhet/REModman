using System.Collections.Generic;

namespace REMod.Core.Configuration.Structs
{
    public class ModData
    {
        public string Name { get; set; }
        public string Hash { get; set; }
        public int LoadOrder { get; set; }
        public string BasePath { get; set; }
        public bool IsEnabled { get; set; }
        public List<ModFile> Files { get; set; }
    }

    public class ModFile
    {
        public string InstallPath { get; set; }
        public string SourcePath { get; set; }
        public string Hash { get; set; }
    }
}