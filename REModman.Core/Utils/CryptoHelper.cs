using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace REModman.Utils
{
    public class CryptoHelper
    {
        public static class FileHash
        {
            public static string Sha256(string pathToFile)
            {
                if (File.Exists(pathToFile))
                {
                    using (SHA256 sha256 = SHA256.Create())
                    {
                        using (FileStream fs = File.OpenRead(pathToFile))
                        {
                            byte[] hash = sha256.ComputeHash(fs);
                            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
                        }
                    }
                }

                return string.Empty;
            }

            public static string Md5(string pathToFile)
            {
                if (File.Exists(pathToFile))
                {
                    using (MD5 md5 = MD5.Create())
                    {
                        using (FileStream fs = File.OpenRead(pathToFile))
                        {
                            byte[] hash = md5.ComputeHash(fs);
                            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
                        }
                    }
                }

                return string.Empty;
            }
        }

        public static class StringHash
        {
            public static string Sha256(string value)
            {
                StringBuilder stringBuilder = new StringBuilder();

                using (SHA256 sha256 = SHA256.Create())
                {
                    Encoding enc = Encoding.UTF8;
                    byte[] result = sha256.ComputeHash(enc.GetBytes(value));

                    foreach (byte b in result)
                    {
                        stringBuilder.Append(b.ToString("x2"));
                    }
                }

                return stringBuilder.ToString();
            }

            public static string Md5(string value)
            {
                StringBuilder stringBuilder = new StringBuilder();

                using (MD5 md5 = MD5.Create())
                {
                    Encoding enc = Encoding.UTF8;
                    byte[] result = md5.ComputeHash(enc.GetBytes(value));

                    foreach (byte b in result)
                    {
                        stringBuilder.Append(b.ToString("x2"));
                    }
                }

                return stringBuilder.ToString();
            }

            public static string Uid()
            {
                StringBuilder builder = new StringBuilder();
                Enumerable
                   .Range(65, 26)
                    .Select(e => ((char)e).ToString())
                    .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                    .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                    .OrderBy(e => Guid.NewGuid())
                    .Take(11)
                    .ToList().ForEach(e => builder.Append(e));

                return builder.ToString();
            }
        }
    }
}
