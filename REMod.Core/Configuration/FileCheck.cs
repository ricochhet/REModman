using System;
using System.Linq;

namespace REMod.Core.Configuration
{
    public class FileCheck
    {
        private static string[] invalidFileTypes = new string[]
        {
            "desktop.ini",
            "thumbs.db",
        };

        public static bool IsSafe(string file)
        {
            if (invalidFileTypes.Contains(file))
            {
                return false;
            }

            return true;
        }
    }
}
