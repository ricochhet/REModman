using System;

namespace REMod.Core.Utils
{
    public class StringHelper
    {
        public static int IndexOfNth(string str, string value, int nth = 0)
        {
            if (nth < 0)
            {
                throw new ArgumentException("Can not find a negative index of substring in string. Must start with 0");
            }

            int offset = str.IndexOf(value);

            for (int i = 0; i < nth; i++)
            {
                if (offset == -1) return -1;
                offset = str.IndexOf(value, offset + 1);
            }

            return offset;
        }

        public static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return value.Length <= maxLength ? value : value[..maxLength];
        }

        public static string Truncate(string value, int maxLength, string truncationSuffix = "...")
        {
            return value?.Length > maxLength
                ? string.Concat(value.AsSpan(0, maxLength), truncationSuffix)
                : value;
        }
    }
}