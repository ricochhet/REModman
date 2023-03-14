using REModman.Logger;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace REModman.Utils
{
    public class FileStreamHelper
    {
        private static readonly bool createDirectories = true;
        private static readonly bool overwrite = true;
        private static readonly bool logMd5 = true;
        private static readonly bool logSha256 = true;
        private static readonly bool debug = false;

        public static void LineWriter(string[] lines, bool suppressLogs)
        {
            if (!suppressLogs)
            {
                foreach (string line in lines)
                {
                    LogBase.Info(StringHelper.Truncate(line, 128, "..."));
                }
            }
        }

        public static void CopyFile(string folderPath, string destinationPath, bool suppressLogs)
        {
            if (createDirectories) Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

            LineWriter(new string[] {
                "[COPY] VARIABLE(folderPath)      : " + folderPath,
                "[COPY] VARIABLE(destinationPath) : " + destinationPath,
                "[COPY] OPERATION(srcPathToFile)  : " + folderPath,
                "[COPY] OPERATION(destPathToFile) : " + destinationPath
            }, suppressLogs);

            if (debug) return;

            if (File.Exists(folderPath))
            {
                File.Copy(folderPath, destinationPath, true);
                if (logMd5)
                {
                    LineWriter(new string[] {
                        "[COPY] MD5(srcPathToFile)        : " + CryptoHelper.FileHash.Md5(folderPath),
                        "[COPY] MD5(destPathToFile)       : " + CryptoHelper.FileHash.Md5(destinationPath),
                    }, suppressLogs);
                }

                if (logSha256)
                {
                    LineWriter(new string[] {
                        "[COPY] SHA256(srcPathToFile)        : " + CryptoHelper.FileHash.Sha256(folderPath),
                        "[COPY] SHA256(destPathToFile)       : " + CryptoHelper.FileHash.Sha256(destinationPath),
                    }, suppressLogs);
                }
            }
        }

        public static void CopyFile(string folderPath, string fileName, string destinationPath, bool suppressLogs)
        {
            if (createDirectories) Directory.CreateDirectory(destinationPath);

            string srcPathToFile = Path.Combine(folderPath, fileName);
            string destPathToFile = Path.Combine(destinationPath, fileName);

            LineWriter(new string[] {
                "[COPY] VARIABLE(folderPath)      : " + folderPath,
                "[COPY] VARIABLE(fileName)        : " + fileName,
                "[COPY] VARIABLE(destinationPath) : " + destinationPath,
                "[COPY] OPERATION(srcPathToFile)  : " + srcPathToFile,
                "[COPY] OPERATION(destPathToFile) : " + destPathToFile
            }, suppressLogs);

            if (debug) return;

            if (File.Exists(srcPathToFile))
            {
                File.Copy(srcPathToFile, destPathToFile, true);
                if (logMd5)
                {
                    LineWriter(new string[] {
                        "[COPY] MD5(srcPathToFile)        : " + CryptoHelper.FileHash.Md5(srcPathToFile),
                        "[COPY] MD5(destPathToFile)       : " + CryptoHelper.FileHash.Md5(destPathToFile),
                    }, suppressLogs);
                }

                if (logSha256)
                {
                    LineWriter(new string[] {
                        "[COPY] SHA256(srcPathToFile)        : " + CryptoHelper.FileHash.Sha256(srcPathToFile),
                        "[COPY] SHA256(destPathToFile)       : " + CryptoHelper.FileHash.Sha256(destPathToFile),
                    }, suppressLogs);
                }
            }
        }

        public static void WriteFile(string folderPath, string fileName, string String, bool suppressLogs)
        {
            if (createDirectories) Directory.CreateDirectory(folderPath);

            string pathToFile = Path.Combine(folderPath, fileName);

            LineWriter(new string[] {
                "[WRITE] VARIABLE(folderPath)     : " + folderPath,
                "[WRITE] VARIABLE(fileName)       : " + fileName,
                "[WRITE] VARIABLE(String)         : " + String,
                "[WRITE] OPERATION(pathToFile)    : " + pathToFile,
            }, suppressLogs);

            if (debug) return;

            if (!File.Exists(pathToFile) || overwrite)
            {
                using (FileStream fs = File.Create(pathToFile))
                {
                    byte[] data = new UTF8Encoding(true).GetBytes(String);
                    fs.Write(data, 0, data.Length);
                }

                if (logMd5)
                {
                    LineWriter(new string[] {
                        "[WRITE] MD5(srcPathToFile)       : " + CryptoHelper.FileHash.Md5(pathToFile),
                    }, suppressLogs);
                }

                if (logSha256)
                {
                    LineWriter(new string[] {
                        "[WRITE] SHA256(srcPathToFile)       : " + CryptoHelper.FileHash.Sha256(pathToFile),
                    }, suppressLogs);
                }
            }
            else
            {
                LineWriter(new string[] {
                    $"File \"{fileName}\" already exists.",
                }, suppressLogs);

                return;
            }
        }

        public static byte[] ReadFile(string pathToFile)
        {
            byte[] fileData = null;

            if (File.Exists(pathToFile))
            {
                using (FileStream fs = File.OpenRead(pathToFile))
                {
                    using BinaryReader binaryReader = new(fs);
                    fileData = binaryReader.ReadBytes((int)fs.Length);
                }

                return fileData;
            }

            return fileData;
        }

        public static string UnkBytesToStr(byte[] bytes)
        {
            using MemoryStream stream = new(bytes);
            using StreamReader streamReader = new(stream);
            return streamReader.ReadToEnd();
        }

        public static List<string> GetFiles(string folderPath, string searchPattern, bool suppressLogs)
        {
            List<string> files = new();

            if (Directory.Exists(folderPath))
            {
                foreach (string file in Directory.EnumerateFiles(folderPath, searchPattern, SearchOption.AllDirectories))
                {
                    files.Add(file);
                }
            }
            else
            {
                LineWriter(new string[] {
                    $"Directory \"{folderPath}\" not found.",
                }, suppressLogs);
            }

            return files;
        }
    }
}