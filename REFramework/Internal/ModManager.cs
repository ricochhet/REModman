using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using REFramework.Utils;
using REFramework.Data;
using REFramework.Configuration;

namespace REFramework.Internal
{
    public class ModManager
    {
        public static List<ModData> CreateModlist(string directory, string defaultModPackageName)
        {
            List<ModData> modList = new List<ModData>();
            IniDataParser parser = new IniDataParser();

            foreach (string data in FileStreamHelper.GetFiles(directory, Constants.MOD_INFO, false))
            {
                byte[] bytes = FileStreamHelper.ReadFile(data);
                string file = FileStreamHelper.UnkBytesToStr(bytes);
                IniData modIni = parser.Parse(file);
                string modPath = Path.Join(Path.GetDirectoryName(data), defaultModPackageName);
                PropertyCollection modInfo = modIni["modinfo"];

                if (File.Exists(modPath))
                {
                    string sha256 = FileStreamHelper.Sha256Checksum(modPath);
                    string md5 = FileStreamHelper.Md5Checksum(modPath);

                    modList.Add(new ModData
                    {
                        Name = modInfo["name"],
                        Description = modInfo["description"],
                        Author = modInfo["author"],
                        Version = modInfo["version"],
                        SourceRelativePath = modPath,
                        SourceAbsolutePath = PathHelper.GetAbsolutePath(modPath),
                        InstallRelativePath = "UNK",
                        InstallAbsolutePath = "UNK",
                        FileName = defaultModPackageName,
                        SHA256 = sha256,
                        MD5 = md5
                    });
                }
                else
                {
                    FileStreamHelper.LineWriter(new string[]
                    {
                        $"Error: The PAK for mod \"{modInfo["name"]}\" could not be found."
                    }, false);
                }
            }

            return modList;
        }

        public static List<ModData> SearchModlist(List<ModData> modList, string searchTerm)
        {
            List<ModData> search = new List<ModData>();

            foreach (ModData mod in modList)
            {
                if (mod.Name == searchTerm)
                {
                    search.Add(mod);
                }
            }

            return search;
        }

        public static ModData SearchModlist(List<ModData> modList, string searchTerm, int selection = 0)
        {
            List<ModData> search = new List<ModData>();

            foreach (ModData mod in modList)
            {
                if (mod.Name == searchTerm)
                {
                    search.Add(mod);
                }
            }

            return search[selection];
        }

        public static void WriteModlist(string directory, List<ModData> modList)
        {
            FileStreamHelper.WriteFile(directory, Constants.MOD_LIST, JsonSerializer.Serialize(modList, new JsonSerializerOptions { WriteIndented = true }), false);
        }
    }
}