using System.IO;

namespace REMod.Core.Plugins.Murmur3
{
    public static class MurMurHash3
    {
        public static uint Hash(Stream stream, uint seed)
        {
            uint num = seed;
            uint num2 = 0u;
            uint num3 = 0u;
            using (BinaryReader binaryReader = new(stream))
            {
                byte[] array = binaryReader.ReadBytes(4);
                while (array.Length != 0)
                {
                    num3 += (uint)array.Length;
                    switch (array.Length)
                    {
                        case 4:
                            num2 = (uint)(array[0] | array[1] << 8 | array[2] << 16 | array[3] << 24);
                            num2 *= 3432918353u;
                            num2 = Rotl32(num2, 15);
                            num2 *= 461845907;
                            num ^= num2;
                            num = Rotl32(num, 13);
                            num = num * 5 + 3864292196u;
                            break;
                        case 3:
                            num2 = (uint)(array[0] | array[1] << 8 | array[2] << 16);
                            num2 *= 3432918353u;
                            num2 = Rotl32(num2, 15);
                            num2 *= 461845907;
                            num ^= num2;
                            break;
                        case 2:
                            num2 = (uint)(array[0] | array[1] << 8);
                            num2 *= 3432918353u;
                            num2 = Rotl32(num2, 15);
                            num2 *= 461845907;
                            num ^= num2;
                            break;
                        case 1:
                            num2 = array[0];
                            num2 *= 3432918353u;
                            num2 = Rotl32(num2, 15);
                            num2 *= 461845907;
                            num ^= num2;
                            break;
                    }
                    array = binaryReader.ReadBytes(4);
                }
            }
            num ^= num3;
            return Fmix(num);
        }

        private static uint Rotl32(uint x, byte r)
        {
            return x << r | x >> 32 - r;
        }

        private static uint Fmix(uint h)
        {
            h ^= h >> 16;
            h *= 2246822507u;
            h ^= h >> 13;
            h *= 3266489909u;
            h ^= h >> 16;
            return h;
        }
    }
}