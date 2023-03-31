namespace REMod.Core.Plugins
{
    public class FileEntry
    {
        public string id;

        public ulong offset;

        public ulong compSize;

        public ulong uncompSize;

        public string filename;

        public string unk1;

        public string unk2;

        public uint filenameLower;

        public uint filenameUpper;

        public FileEntry()
        {
            id = "";
            offset = 0uL;
            compSize = 0uL;
            uncompSize = 0uL;
            filename = "";
            unk1 = "";
            unk2 = "";
            filenameLower = 0u;
            filenameUpper = 0u;
        }
    }
}
