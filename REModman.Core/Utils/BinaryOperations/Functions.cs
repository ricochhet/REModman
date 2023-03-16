using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace REModman.Utils.BinaryOperations
{
    public class Functions
    {
        public static bool CompareBytes(byte[] Arg0, byte[] Arg1)  => Operators.CompareString(Conversions.ObjectToHex(Arg0), Conversions.ObjectToHex(Arg1), TextCompare: false) == 0;

        public static int FileLen(string FilePath) => checked((int)FileSystem.FileLen(FilePath));

        public static string GetRatio(long Arg0, long Arg1) => Strings.Format((double)Arg0 / (double)Arg1, "0.00") + "%";

        public static bool IsNumeric(long Numeric) => new Regex("^[0-9]+\\d").IsMatch(Numeric.ToString());

        public static bool IsValidHex(string Hex) => new Regex("^[A-Fa-f0-9]*$", RegexOptions.IgnoreCase).IsMatch(Hex);

        public static bool IsValidUnicode(string Unicode) => Strings.Len(Unicode) == LenB(Unicode);

        public static int LenB(string ObjStr)
        {
            if (Strings.Len(ObjStr) != 0)
            {
                return Encoding.Unicode.GetByteCount(ObjStr);
            }
            return 0;
        }

        public static byte[] RemoveAt(byte[] Bytes, int Index) => Conversions.HexToByteArray(Conversions.ObjectToHex(Bytes).Remove(Index, 2));

        public static byte[] RemoveByte(byte[] Bytes, byte ByteToRemove) => Conversions.HexToByteArray(Conversions.ObjectToHex(Bytes).Replace(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(ByteToRemove), ""));
        public static Array ReverseArray(Array Buffer)
        {
            Array.Reverse(Buffer);
            return Buffer;
        }

        public static string RoundBytes(int ByteCount)
        {
            if (ByteCount < 1024)
            {
                return Microsoft.VisualBasic.CompilerServices.Conversions.ToString(ByteCount) + " b";
            }

            if (ByteCount >= 1024 && ByteCount < 1048576)
            {
                return Strings.Format((double)ByteCount / 1024.0, "0.00") + " kb";
            }

            if (ByteCount >= 1048576 && ByteCount < 1073741824)
            {
                return Strings.Format((double)ByteCount / 1024.0 / 1024.0, "0.00") + " mb";
            }

            if (ByteCount < 1073741824 || ByteCount >= 0)
            {
                throw new Exception("WTF!");
            }

            return Strings.Format((double)ByteCount / 1024.0 / 1024.0 / 1024.0, "0.00") + " gb";
        }

        public static byte[] SwapSex(byte[] Buffer) => (byte[])ReverseArray(Buffer);
    }
}
