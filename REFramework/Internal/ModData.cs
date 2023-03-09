using System.Collections.Generic;

namespace REFramework.Data
{
    public class ModData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string LoadAfter { get; set; }
        public string Path { get; set; }
        public List<ModFile> ModFiles { get; set; }
    }

    public class ModFile
    {
        public string SourceRelativePath { get; set; }
        public string SourceAbsolutePath { get; set; }
        public string SHA256 { get; set; }
        public string MD5 { get; set; }
    }
}