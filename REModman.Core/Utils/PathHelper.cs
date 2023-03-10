using System;
using System.IO;

namespace REModman.Utils
{
    public class PathHelper
    {
        public static String GetAbsolutePath(String path)
        {
            return GetAbsolutePath(null ?? string.Empty, path);
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
    }
}