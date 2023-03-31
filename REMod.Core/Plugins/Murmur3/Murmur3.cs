using System;

namespace REMod.Core.Plugins.Murmur3
{
    public class Murmur3
    {
        public static uint MurmurHash3_x86_32(byte[] data, uint length, uint seed)
        {
            uint num = length >> 2;
            uint num2 = seed;
            int num3 = 0;
            for (uint num4 = num; num4 != 0; num4--)
            {
                uint num5 = BitConverter.ToUInt32(data, num3);
                num5 *= 3432918353u;
                num5 = Rotl32(num5, 15);
                num5 *= 461845907;
                num2 ^= num5;
                num2 = Rotl32(num2, 13);
                num2 = num2 * 5 + 3864292196u;
                num3 += 4;
            }
            num <<= 2;
            uint num6 = 0u;
            uint num7 = length & 3u;
            if (num7 == 3)
            {
                num6 ^= (uint)(data[2 + num] << 16);
            }
            if (num7 >= 2)
            {
                num6 ^= (uint)(data[1 + num] << 8);
            }
            if (num7 >= 1)
            {
                num6 ^= data[num];
                num6 *= 3432918353u;
                num6 = Rotl32(num6, 15);
                num6 *= 461845907;
                num2 ^= num6;
            }
            num2 ^= length;
            return Fmix32(num2);
        }

        private static uint Fmix32(uint h)
        {
            h ^= h >> 16;
            h *= 2246822507u;
            h ^= h >> 13;
            h *= 3266489909u;
            h ^= h >> 16;
            return h;
        }

        private static uint Rotl32(uint x, byte r)
        {
            return x << r | x >> 32 - r;
        }

        public static bool VerificationTest()
        {
            byte[] array = new byte[256];
            byte[] array2 = new byte[1024];
            for (uint num = 0u; num < 256; num++)
            {
                array[num] = (byte)num;
                uint value = MurmurHash3_x86_32(array, num, 256 - num);
                Buffer.BlockCopy(BitConverter.GetBytes(value), 0, array2, (int)(num * 4), 4);
            }
            uint num2 = MurmurHash3_x86_32(array2, 1024u, 0u);
            uint num3 = 2968878819u;
            if (num3 != num2)
            {
                return false;
            }
            Console.WriteLine("works");
            return true;
        }
    }
}