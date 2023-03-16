using REMod.Core.Logger;
using REMod.Core.Murmur3;
using REMod.Core.Utils.BinaryOperations;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace REMod.Core.Tools
{
    public class RisePakPatch
    {
        public static void ProcessDirectory(string path, string outputFile = "re_chunk_000.pak.patch_001.pak")
        {
            string directory = new DirectoryInfo(path).FullName;

            if (!File.GetAttributes(directory).HasFlag(FileAttributes.Directory))
            {
                return;
            }

            if (File.Exists(outputFile))
            {
                LogBase.Info("Deleting existing output file...");
                File.Delete(outputFile);
            }

            FileInfo[] files = new DirectoryInfo(Path.Combine(directory, "natives")).GetFiles("*.*", SearchOption.AllDirectories);
            LogBase.Info($"Processing {files.Length} files...\n");

            List<FileEntry> list = new();
            File.Create(outputFile).Dispose();

            Writer writer = new(outputFile);
            writer.WriteUInt32(1095454795u);
            writer.WriteUInt32(4u);
            writer.WriteUInt32((uint)files.Length);
            writer.WriteUInt32(0u);
            writer.Seek(48 * files.Length + writer.Position);

            FileInfo[] array = files;
            foreach (FileInfo obj in array)
            {
                FileEntry fileEntry2 = new();
                string text = obj.FullName.Replace(directory, "").Replace("\\", "/");

                if (text.StartsWith("/"))
                {
                    text = text[1..];

                }

                uint hash = GetHash(text.ToLower(), uint.MaxValue);
                uint hash2 = GetHash(text.ToUpper(), uint.MaxValue);
                byte[] array2 = File.ReadAllBytes(obj.FullName);

                fileEntry2.filename = text;
                fileEntry2.offset = (ulong)writer.Position;
                fileEntry2.uncompSize = (ulong)array2.Length;
                fileEntry2.filenameLower = hash;
                fileEntry2.filenameUpper = hash2;

                list.Add(fileEntry2);
                writer.Write(array2);
            }

            writer.Seek(16L);

            foreach (FileEntry item in list)
            {
                LogBase.Info($"{item.filename}, {item.filenameLower}, {item.filenameUpper}");

                writer.WriteUInt32(item.filenameLower);
                writer.WriteUInt32(item.filenameUpper);
                writer.WriteUInt64(item.offset);
                writer.WriteUInt64(item.uncompSize);
                writer.WriteUInt64(item.uncompSize);
                writer.WriteUInt64(0uL);
                writer.WriteUInt32(0u);
                writer.WriteUInt32(0u);
            }

            writer.Close();
        }

        private static uint GetHash(string m_String, uint seed)
        {
            using MemoryStream stream = new(Encoding.Unicode.GetBytes(m_String));
            return MurMurHash3.Hash(stream, seed);
        }
    }
}
