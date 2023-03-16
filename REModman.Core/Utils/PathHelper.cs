using System;
using System.IO;

namespace REMod.Core.Utils
{
    public class PathHelper
    {
        public static string UnixPath(string value) => value.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        public static string GetAbsolutePath(string path) => GetAbsolutePath(null ?? string.Empty, path);

        public static string GetAbsolutePath(string basePath, string path)
        {
            string finalPath;

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

        public static string[] GetFirstDirectory(string value) => value[2..].Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
        
        public static string MakeValid(string value)
        {
            string temp = value;

            foreach (char invalidChar in Path.GetInvalidFileNameChars())
                temp = temp.Replace(char.ToString(invalidChar), "");

            foreach (char invalidChar in Path.GetInvalidPathChars())
                temp = temp.Replace(char.ToString(invalidChar), "");

            return temp;
        }
    }
}