using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace REMod.Core.Plugins.BinaryOperations
{
    public class Conversions
    {
        public static string ASCIIToBase64(string ASCII) => ByteArrayToBase64(ASCIIToByteArray(ASCII));

        public static byte[] ASCIIToByteArray(string Value) => Encoding.ASCII.GetBytes(Value);

        public static string Base64ToASCII(string Base64) => ByteArrayToASCII(Base64ToByteArray(Base64));

        public static byte[] Base64ToByteArray(string ASCII) => Convert.FromBase64String(ASCII);

        public static int BinaryToInteger(string Value)
        {
            ulong num = 0uL;
            checked
            {
                int num2 = Value.Length - 1;
                for (int i = 0; i <= num2; i++)
                {
                    num = (ulong)Math.Round(Math.Round(num + Val(Value.Substring(Value.Length - i + 1, 1)) * Math.Pow(2.0, i - 1)));
                }

                return (int)num;
            }
        }

        public static string ByteArrayToASCII(byte[] Bytes) => Encoding.ASCII.GetString(Bytes);

        public static string ByteArrayToBase64(byte[] Bytes) => Convert.ToBase64String(Bytes);

        public static double ByteArrayToDouble(byte[] Bytes, bool ReturnBigEndian = false)
        {
            if (ReturnBigEndian)
            {
                Bytes = Functions.SwapSex(Bytes);
            }

            return BitConverter.ToDouble(Bytes, 0);
        }

        public static int ByteArrayToInt24(byte[] Buffer) => Buffer[2] | Buffer[1] | Buffer[0];

        public static long ByteArrayToInt40(byte[] Buffer) => (uint)(Buffer[4] | Buffer[3] | Buffer[2] | Buffer[1] | Buffer[0]);

        public static long ByteArrayToInt48(byte[] Buffer) => (uint)(Buffer[5] | Buffer[4] | Buffer[3] | Buffer[2] | Buffer[1] | Buffer[0]);

        public static long ByteArrayToInt56(byte[] Buffer) => (uint)(Buffer[6] | Buffer[5] | Buffer[4] | Buffer[3] | Buffer[2] | Buffer[1] | Buffer[0]);

        public static int ByteArrayToInteger(byte[] Bytes, bool ReturnBigEndian = false)
        {
            if (ReturnBigEndian)
            {
                Bytes = Functions.SwapSex(Bytes);
            }

            return BitConverter.ToInt32(Bytes, 0);
        }

        public static long ByteArrayToLong(byte[] Bytes, bool ReturnBigEndian = false)
        {
            if (ReturnBigEndian)
            {
                Bytes = Functions.SwapSex(Bytes);
            }

            return BitConverter.ToInt64(Bytes, 0);
        }

        public static short ByteArrayToShort(byte[] Bytes, bool ReturnBigEndian = false)
        {
            if (ReturnBigEndian)
            {
                Bytes = Functions.SwapSex(Bytes);
            }

            return BitConverter.ToInt16(Bytes, 0);
        }

        public static float ByteArrayToSingle(byte[] Bytes, bool ReturnBigEndian = false)
        {
            if (ReturnBigEndian)
            {
                Bytes = Functions.SwapSex(Bytes);
            }

            return BitConverter.ToSingle(Bytes, 0);
        }

        public static string ByteArrayToUnicode(byte[] Bytes, bool ReturnBigEndian = false)
        {
            if (!ReturnBigEndian)
            {
                return Encoding.Unicode.GetString(Bytes);
            }

            return Encoding.BigEndianUnicode.GetString(Bytes);
        }

        public static byte[] DoubleToByteArray(double Value, bool ReturnBigEndian = false)
        {
            byte[] array = BitConverter.GetBytes(Value);

            if (ReturnBigEndian)
            {
                array = Functions.SwapSex(array);
            }

            return array;
        }

        public static byte[] HexToByteArray(string Value)
        {
            if (!Functions.IsValidHex(Value))
            {
                throw new Exception("Invalid hex input");
            }
            checked
            {
                byte[] array = new byte[(int)Math.Round(Math.Round(Value.Length / 2.0 - 1.0)) + 1];
                int num = (int)Math.Round(Math.Round(Value.Length / 2.0 - 1.0));

                for (int i = 0; i <= num; i++)
                {
                    array[i] = (byte)Math.Round(Math.Round(Val(string.Concat("&h", Value.AsSpan(i * 2, 2)))));
                }

                return array;
            }
        }

        public static byte[] Int16ToByteArray(int Value)
        {
            byte[] bytes = BitConverter.GetBytes(Value);

            return new byte[2]
            {
            bytes[0],
            bytes[1]
            };
        }

        public static byte[] Int24ToByteArray(int Value)
        {
            byte[] bytes = BitConverter.GetBytes(Value);

            return new byte[3]
            {
            bytes[0],
            bytes[1],
            bytes[2]
            };
        }

        public static byte[] Int40ToByteArray(long Value)
        {
            byte[] bytes = BitConverter.GetBytes(Value);

            return new byte[5]
            {
            bytes[0],
            bytes[1],
            bytes[2],
            bytes[3],
            bytes[4]
            };
        }

        public static byte[] Int48ToByteArray(long Value)
        {
            byte[] bytes = BitConverter.GetBytes(Value);

            return new byte[6]
            {
            bytes[0],
            bytes[1],
            bytes[2],
            bytes[3],
            bytes[4],
            bytes[5]
            };
        }

        public static byte[] Int56ToByteArray(long Value)
        {
            byte[] bytes = BitConverter.GetBytes(Value);

            return new byte[7]
            {
            bytes[0],
            bytes[1],
            bytes[2],
            bytes[3],
            bytes[4],
            bytes[5],
            bytes[6]
            };
        }

        public static string IntegerToBinary(int Value)
        {
            string text = null;
            do
            {
                text += (Value % 2).ToString();
                Value = checked((int)Math.Round(Value / 2.0));
            }

            while (Value >= 1);
            return text;
        }

        public static byte[] IntegerToByteArray(int Value, bool ReturnBigEndian = false)
        {
            byte[] array = BitConverter.GetBytes(Value);
            if (ReturnBigEndian)
            {
                array = Functions.SwapSex(array);
            }

            return array;
        }

        public static byte[] LongToByteArray(long Value, bool ReturnBigEndian = false)
        {
            byte[] array = BitConverter.GetBytes(Value);
            if (ReturnBigEndian)
            {
                array = Functions.SwapSex(array);
            }

            return array;
        }

        public static string ObjectToHex(object Value)
        {
            if (Value.GetType().ToString() != "System.Byte[]")
            {
                byte[] bytes = Encoding.Unicode.GetBytes(RuntimeHelpers.GetObjectValue(RuntimeHelpers.GetObjectValue(Value)).ToString());
                return BitConverter.ToString(bytes);
            }

            return BitConverter.ToString((byte[])Value).Replace("-", "");
        }

        public static byte[] ShortToByteArray(short Value, bool ReturnBigEndian = false)
        {
            byte[] array = BitConverter.GetBytes(Value);
            if (ReturnBigEndian)
            {
                array = Functions.SwapSex(array);
            }

            return array;
        }

        public static byte[] SingleToByteArray(float Value, bool ReturnBigEndian = false)
        {
            byte[] array = BitConverter.GetBytes(Value);
            if (ReturnBigEndian)
            {
                array = Functions.SwapSex(array);
            }

            return array;
        }

        public static byte[] UnicodeToByteArray(string Value, bool ReturnBigEndian = false)
        {
            if (!ReturnBigEndian)
            {
                return Encoding.Unicode.GetBytes(Value);
            }

            return Encoding.BigEndianUnicode.GetBytes(Value);
        }

        private static double Val(string value)
        {
            string result = string.Empty;

            foreach (char c in value)
            {
                if (result.Length == 0 && c.Equals('-'))
                    result += c;
                else if (char.IsNumber(c) || c.Equals('.') && !result.Any(x => x.Equals('.')))
                    result += c;
                else if (!c.Equals(' '))
                    return string.IsNullOrEmpty(result) ? 0 : Convert.ToDouble(result);
            }

            return string.IsNullOrEmpty(result) || result == "-" ? 0 : Convert.ToDouble(result);
        }
    }
}