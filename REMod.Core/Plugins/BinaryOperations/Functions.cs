using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace REMod.Core.Plugins.BinaryOperations
{
    public class Functions
    {
        public static bool CompareBytes(byte[] Arg0, byte[] Arg1) => Conversions.ObjectToHex(Arg0) == Conversions.ObjectToHex(Arg1);

        public static int FileLen(string FilePath) => checked((int)new FileInfo(FilePath).Length);

        public static string GetRatio(long Arg0, long Arg1) => (Arg0 / (double)Arg1).ToString("#.##") + "%";

        public static bool IsNumeric(long Numeric) => new Regex("^[0-9]+\\d").IsMatch(Numeric.ToString());

        public static bool IsValidHex(string Hex) => new Regex("^[A-Fa-f0-9]*$", RegexOptions.IgnoreCase).IsMatch(Hex);

        public static bool IsValidUnicode(string Unicode) => Unicode.Length == LenB(Unicode);

        public static int LenB(string ObjStr)
        {
            if (ObjStr.Length != 0)
            {
                return Encoding.Unicode.GetByteCount(ObjStr);
            }

            return 0;
        }

        public static byte[] RemoveAt(byte[] Bytes, int Index) => Conversions.HexToByteArray(Conversions.ObjectToHex(Bytes).Remove(Index, 2));

        public static byte[] RemoveByte(byte[] Bytes, byte ByteToRemove) => Conversions.HexToByteArray(Conversions.ObjectToHex(Bytes).Replace(ByteToRemove.ToString(), ""));

        public static Array ReverseArray(Array Buffer)
        {
            Array.Reverse(Buffer);
            return Buffer;
        }

        public static string RoundBytes(int ByteCount)
        {
            if (ByteCount < 1024)
            {
                return ByteCount.ToString() + " b";
            }

            if (ByteCount >= 1024 && ByteCount < 1048576)
            {
                return (ByteCount / 1024.0).ToString("#.##") + " kb";
            }

            if (ByteCount >= 1048576 && ByteCount < 1073741824)
            {
                return (ByteCount / 1024.0 / 1024.0).ToString("#.##") + " mb";
            }

            if (ByteCount < 1073741824 || ByteCount >= 0)
            {
                throw new IndexOutOfRangeException();
            }

            return (ByteCount / 1024.0 / 1024.0 / 1024.0).ToString("#.##") + " gb";
        }

        public static byte[] SwapSex(byte[] Buffer) => (byte[])ReverseArray(Buffer);
    }
}
