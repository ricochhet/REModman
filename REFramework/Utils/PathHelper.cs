using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace REFramework.Utils
{
    public class PathHelper
    {
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
    }
}