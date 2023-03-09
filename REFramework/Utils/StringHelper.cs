using System;
using System.IO;

namespace REFramework.Utils
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
    }
}