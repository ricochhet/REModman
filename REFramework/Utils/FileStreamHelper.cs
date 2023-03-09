using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace REFramework.Utils
{
    public class FileStreamHelper
    {
        private static bool createDirectories = true;
        private static bool overwrite = true;
        private static bool logMd5 = true;
        private static bool logSha256 = true;
        private static bool debug = false;

        public static void LineWriter(string[] lines, bool suppressLogs)
        {
            if (!suppressLogs)
            {
                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }
            }
        }

        public static string Md5Checksum(string pathToFile)
        {
            if (File.Exists(pathToFile))
            {
                using (MD5 md5 = MD5.Create())
                {
                    using (FileStream fs = File.OpenRead(pathToFile))
                    {
                        byte[] hash = md5.ComputeHash(fs);
                        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
                    }
                }
            }

            return string.Empty;
        }

        public static string Sha256Checksum(string pathToFile)
        {
            if (File.Exists(pathToFile))
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    using (FileStream fs = File.OpenRead(pathToFile))
                    {
                        byte[] hash = sha256.ComputeHash(fs);
                        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
                    }
                }
            }

            return string.Empty;
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
                        "[COPY] MD5(srcPathToFile)        : " + Md5Checksum(srcPathToFile),
                        "[COPY] MD5(destPathToFile)       : " + Md5Checksum(destPathToFile),
                    }, suppressLogs);
                }

                if (logSha256)
                {
                    LineWriter(new string[] {
                        "[COPY] SHA256(srcPathToFile)        : " + Sha256Checksum(srcPathToFile),
                        "[COPY] SHA256(destPathToFile)       : " + Sha256Checksum(destPathToFile),
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
                        "[WRITE] MD5(srcPathToFile)       : " + Md5Checksum(pathToFile),
                    }, suppressLogs);
                }

                if (logSha256)
                {
                    LineWriter(new string[] {
                        "[WRITE] SHA256(srcPathToFile)       : " + Sha256Checksum(pathToFile),
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
                    using (BinaryReader binaryReader = new BinaryReader(fs))
                    {
                        fileData = binaryReader.ReadBytes((int)fs.Length);
                    }
                }

                return fileData;
            }

            return fileData;
        }

        public static string UnkBytesToStr(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    // System.Text.Encoding.UTF8.GetString
                    return streamReader.ReadToEnd();
                }
            }
        }

        public static List<string> GetFiles(string folderPath, string searchPattern, bool suppressLogs)
        {
            List<string> files = new List<string>();

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

        public static String GetAbsolutePath(String path)
        {
            return GetAbsolutePath(null, path);
        }

        public static String GetAbsolutePath(String basePath, String path)
        {
            String finalPath;

            if (path == null) return null;
            if (basePath == null)
            {
                basePath = Path.GetFullPath(".");
            }
            else
            {
                basePath = GetAbsolutePath(null, basePath);
            }

            if (!Path.IsPathRooted(path) || "\\".Equals(Path.GetPathRoot(path)))
            {
                if (path.StartsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    finalPath = Path.Combine(Path.GetPathRoot(basePath), path.TrimStart(Path.DirectorySeparatorChar));
                }
                else
                {
                    finalPath = Path.Combine(basePath, path);
                }
            }
            else
            {
                finalPath = path;
            }

            return Path.GetFullPath(finalPath);
        }

        public static int GetProcIdFromName(string name)
        {
            Process[] processlist = Process.GetProcesses();

            if (name.ToLower().Contains(".exe"))
            {
                name = name.Replace(".exe", "");
            }
            if (name.ToLower().Contains(".bin"))
            {
                name = name.Replace(".bin", "");
            }

            foreach (System.Diagnostics.Process process in processlist)
            {
                if (process.ProcessName.Equals(name, StringComparison.CurrentCultureIgnoreCase)) return process.Id;
            }

            return 0;
        }

        public static string GetProcPath(int procId)
        {
            return Process.GetProcessById(procId).MainModule.FileName;
        }

        public static string GetProcPath(string procName)
        {
            return Process.GetProcessById(GetProcIdFromName(procName)).MainModule.FileName;
        }

        public static void OpenProcess(string fileName, string command, string workingDir)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(fileName, "-c \" " + command + " \"")
            {
                WorkingDirectory = workingDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = Process.Start(processStartInfo);
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            int exitCode = process.ExitCode;

            process.Close();
        }
    }
}