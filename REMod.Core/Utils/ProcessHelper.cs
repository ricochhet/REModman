using REMod.Core.Configuration.Enums;
using REMod.Core.Configuration;
using REMod.Core.Logger;
using System;
using System.Diagnostics;

namespace REMod.Core.Utils
{
    public class ProcessHelper
    {
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

            foreach (Process process in processlist)
            {
                if (process.ProcessName.Equals(name, StringComparison.CurrentCultureIgnoreCase)) return process.Id;
            }

            return 0;
        }

        public static string GetProcPath(int procId) => Process.GetProcessById(procId).MainModule.FileName;
        public static string GetProcPath(GameType type) => Process.GetProcessById(GetProcIdFromName(type)).MainModule.FileName;
        public static string GetProcPath(string procName) => Process.GetProcessById(GetProcIdFromName(procName)).MainModule.FileName;
        public static int GetProcIdFromName(GameType type) => GetProcIdFromName(GameTypeResolver.ProcessName(type));
        public static bool IsProcRunning(GameType type) => GetProcIdFromName(type) != 0;

        public static void OpenProcess(string fileName, string command, string workingDir)
        {
            ProcessStartInfo processStartInfo = new(fileName, "-c \" " + command + " \"")
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
            process.Close();
        }
    }
}