using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using REFramework.Utils;
using REFramework.Data;
using REFramework.Configuration;
// rename this to mod indexer or something lol
namespace REFramework.Internal
{
    public class ModManager
    {
        public static List<ModData> IndexModDirectory(string directory)
        {
            List<ModData> modList = new List<ModData>();
            IniDataParser parser = new IniDataParser();

            foreach (string data in FileStreamHelper.GetFiles(directory, Constants.MOD_INFO, false))
            {
                byte[] bytes = FileStreamHelper.ReadFile(data);
                string file = FileStreamHelper.UnkBytesToStr(bytes);
                IniData modIni = parser.Parse(file);
                PropertyCollection modInfo = modIni["modinfo"];
                string modPath = Path.Join(Path.GetDirectoryName(data), modInfo["files"]);

                if (Directory.Exists(modPath))
                {
                    List<ModFile> modFiles = new List<ModFile>();

                    foreach (string filePath in FileStreamHelper.GetFiles(modPath, "*.*", false))
                    {
                        string sha256 = FileStreamHelper.Sha256Checksum(filePath);
                        string md5 = FileStreamHelper.Md5Checksum(filePath);

                        modFiles.Add(new ModFile
                        {
                            SourceRelativePath = filePath,
                            SourceAbsolutePath = PathHelper.GetAbsolutePath(filePath),
                            SHA256 = sha256,
                            MD5 = md5
                        });
                    }

                    modList.Add(new ModData
                    {
                        Name = modInfo["name"],
                        Description = modInfo["description"],
                        Author = modInfo["author"],
                        Version = modInfo["version"],
                        LoadAfter = string.Empty,
                        Path = modInfo["files"],
                        ModFiles = modFiles
                    });
                }
            }

            return modList;
        }

        public static void WriteModIndex(string directory, List<ModData> modList)
        {
            FileStreamHelper.WriteFile(directory, Constants.MOD_INDEX, JsonSerializer.Serialize(modList, new JsonSerializerOptions { WriteIndented = true }), false);
        }
    }
}