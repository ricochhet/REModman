using System.Collections.Generic;

namespace REModman.Configuration.Structs
{
    public class ModData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Hash { get; set; }
        public bool IsEnabled { get; set; }
        public List<ModFile> Files { get; set; }
    }

    public class ModFile
    {
        public string DefaultInstallPath { get; set; }
        public string InstallPath { get; set; }
        public string SourcePath { get; set; }
        public string Hash { get; set; }
    }
}