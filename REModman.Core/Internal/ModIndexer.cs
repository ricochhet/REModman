using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using REModman.Utils;
using REModman.Configuration;
using REModman.Configuration.Enums;
using System.Diagnostics;
using REModman.Internal;
using REModman.Configuration.Structs;

namespace REModman.Internal
{
    public class ModIndexer
    {
        public static List<ModData> Index(GameType type)
        {
            List<ModData> modList = new List<ModData>();
            IniDataParser parser = new IniDataParser();
            string gameName = EnumSwitch.GetModFolder(type);

            if (Directory.Exists(Path.Combine(Constants.MODS_FOLDER, gameName)))
            {
                string gameModFolder = Path.Combine(Constants.MODS_FOLDER, gameName);
                foreach (string data in FileStreamHelper.GetFiles(gameModFolder, Constants.MOD_INFO_FILE, false))
                {
                    byte[] bytes = FileStreamHelper.ReadFile(data);
                    string file = FileStreamHelper.UnkBytesToStr(bytes);
                    IniData modIni = parser.Parse(file);
                    PropertyCollection modInfo = modIni["modinfo"];
                    string modPath = Path.Join(Path.GetDirectoryName(data), modInfo["files"]);

                    if (File.Exists(modPath))
                    {
                        List<ModFile> modFiles = new List<ModFile>();
                        string fileSHA256 = CryptoHelper.FileHash.Sha256(modPath);
                        string fileMD5 = CryptoHelper.FileHash.Md5(modPath);

                        modFiles.Add(new ModFile
                        {
                            LocalFilePath = "." + modPath.Substring(StringHelper.IndexOfNth(modPath, "\\", 2)),
                            SourceRelativePath = modPath,
                            SourceAbsolutePath = PathHelper.GetAbsolutePath(modPath),
                            SHA256 = fileSHA256,
                            MD5 = fileMD5
                        });

                        modList.Add(new ModData
                        {
                            Name = modInfo["name"],
                            Description = modInfo["description"],
                            Author = modInfo["author"],
                            Version = modInfo["version"],
                            LoadOrder = modIni["user"]["loadOrder"],
                            Path = modInfo["files"],
                            Guid = fileSHA256,
                            ModFiles = modFiles
                        });
                    }
                    else if (Directory.Exists(modPath))
                    {
                        List<ModFile> modFiles = new List<ModFile>();
                        string modSHA256 = string.Empty;

                        foreach (string filePath in FileStreamHelper.GetFiles(modPath, "*.*", false))
                        {
                            string fileSHA256 = CryptoHelper.FileHash.Sha256(filePath);
                            string fileMD5 = CryptoHelper.FileHash.Md5(filePath);
                            modSHA256 += fileSHA256;

                            modFiles.Add(new ModFile
                            {
                                LocalFilePath = "." + filePath.Substring(StringHelper.IndexOfNth(filePath, "\\", 2)),
                                SourceRelativePath = filePath,
                                SourceAbsolutePath = PathHelper.GetAbsolutePath(filePath),
                                SHA256 = fileSHA256,
                                MD5 = fileMD5
                            });
                        }

                        modList.Add(new ModData
                        {
                            Name = modInfo["name"],
                            Description = modInfo["description"],
                            Author = modInfo["author"],
                            Version = modInfo["version"],
                            LoadOrder = modIni["user"]["loadOrder"],
                            Path = modInfo["files"],
                            Guid = CryptoHelper.StringHash.Sha256(modSHA256),
                            ModFiles = modFiles
                        });
                    }
                }
            }

            return modList.OrderBy(o => o.LoadOrder).ToList();
        }

        public static List<ModData> DeserializeData(GameType type)
        {
            List<ModData> modData = new List<ModData>();
            string gameModFolder = EnumSwitch.GetModFolder(type);
            string gameDataFolder = Path.Combine(Constants.DATA_FOLDER, gameModFolder);

            if (Directory.Exists(gameDataFolder))
            {
                if (File.Exists(Path.Combine(gameDataFolder, Constants.MOD_INDEX_FILE)))
                {
                    byte[] bytes = FileStreamHelper.ReadFile(Path.Combine(gameDataFolder, Constants.MOD_INDEX_FILE));
                    string file = FileStreamHelper.UnkBytesToStr(bytes);
                    modData = JsonSerializer.Deserialize<List<ModData>>(file);
                }
            }

            return modData;
        }

        public static void SaveData(GameType type, List<ModData> modList)
        {
            string gameModFolder = EnumSwitch.GetModFolder(type);
            string gameDataFolder = Path.Combine(Constants.DATA_FOLDER, gameModFolder);
            FileStreamHelper.WriteFile(gameDataFolder, Constants.MOD_INDEX_FILE, JsonSerializer.Serialize(modList, new JsonSerializerOptions { WriteIndented = true }), false);
        }
    }
}