using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using REMod.RisePakPatch.BinaryOperations;

namespace REMod.RisePakPatch
{
    internal class Program
    {
        private static string outputFile = "re_chunk_000.pak.patch_001.pak";

        private static void Main(string[] args)
        {
            Console.WriteLine("Rise Pak Patch Tool by MHVuze v1.1.1");
            Console.WriteLine("Based on RE7 Tools by Michalss & Kramla, Ekey");
            Console.WriteLine("==========");
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: \tRisePakPatch <path_to_folder> <pak_patch_name>\n");
            }
            else
            {
                string path = args[0];
                if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                {
                    Console.WriteLine("Input is directory.");
                    outputFile = args[1];
                    Console.WriteLine("Output file: " + outputFile);
                    ProcessDirectory(new DirectoryInfo(path).FullName);
                }
                else
                {
                    Console.WriteLine("Input is no directory.");
                    Console.WriteLine("Usage: \tRisePakPatch <path_to_folder> <pak_patch_name>\n");
                }
            }
            Console.WriteLine("Press Enter to exit...");
            Console.Read();
        }

        private static void ProcessDirectory(string directory)
        {
            if (File.Exists(outputFile))
            {
                Console.WriteLine("Deleting existing output file...");
                File.Delete(outputFile);
            }
            FileInfo[] files = new DirectoryInfo(directory).GetFiles("*.*", SearchOption.AllDirectories);
            Console.WriteLine($"Processing {files.Length} files...\n");
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
                    text = text.Substring(1);
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
                Console.WriteLine($"{item.filename}, {item.filenameLower}, {item.filenameUpper}");
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
            Console.WriteLine();
        }

        private static uint GetHash(string m_String, uint seed)
        {
            using MemoryStream stream = new(Encoding.Unicode.GetBytes(m_String));
            return MurMurHash3.Hash(stream, seed);
        }
    }
}
