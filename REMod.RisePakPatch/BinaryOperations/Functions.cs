using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

namespace REMod.RisePakPatch.BinaryOperations;

public class Functions
{
	public static bool CompareBytes(byte[] Arg0, byte[] Arg1)
	{
		return Operators.CompareString(Conversions.ObjectToHex(Arg0), Conversions.ObjectToHex(Arg1), TextCompare: false) == 0;
	}

	public static int FileLen(string FilePath)
	{
		return checked((int)FileSystem.FileLen(FilePath));
	}

	public static object GetImageThumbnail(Image Picture, int Width, int Height)
	{
		return Picture.GetThumbnailImage(Width, Height, (Image.GetThumbnailImageAbort)Delegate.Combine(), IntPtr.Zero);
	}

	public static string GetRatio(long Arg0, long Arg1)
	{
		return Strings.Format((double)Arg0 / (double)Arg1, "0.00") + "%";
	}

	public static bool IsNumeric(long Numeric)
	{
		return new Regex("^[0-9]+\\d").IsMatch(Numeric.ToString());
	}

	public static bool IsValidHex(string Hex)
	{
		return new Regex("^[A-Fa-f0-9]*$", RegexOptions.IgnoreCase).IsMatch(Hex);
	}

	public static bool IsValidUnicode(string Unicode)
	{
		return Strings.Len(Unicode) == LenB(Unicode);
	}

	public static int LenB(string ObjStr)
	{
		if (Strings.Len(ObjStr) != 0)
		{
			return Encoding.Unicode.GetByteCount(ObjStr);
		}
		return 0;
	}

	public static byte[] RemoveAt(byte[] Bytes, int Index)
	{
		return Conversions.HexToByteArray(Conversions.ObjectToHex(Bytes).Remove(Index, 2));
	}

	public static byte[] RemoveByte(byte[] Bytes, byte ByteToRemove)
	{
		return Conversions.HexToByteArray(Conversions.ObjectToHex(Bytes).Replace(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(ByteToRemove), ""));
	}

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

	public static byte[] SwapSex(byte[] Buffer)
	{
		return (byte[])ReverseArray(Buffer);
	}
}
