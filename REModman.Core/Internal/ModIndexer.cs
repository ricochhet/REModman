using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using REModman.Utils;
using REModman.Data;
using REModman.Configuration;
using REModman.Configuration.Enums;

namespace REModman.Internal
{
    public class ModIndexer
    {
        public static void CreateDataFolder(GameType type)
        {
            string gameModFolder = EnumSwitch.GetModFolder(type);
            string gameDataFolder = Path.Combine(Constants.DATA_FOLDER, gameModFolder);

            if (!File.Exists(Path.Combine(gameDataFolder, Constants.MOD_INDEX_FILE)))
            {
                FileStreamHelper.WriteFile(gameDataFolder, Constants.MOD_INDEX_FILE, "[]", false);
            }

            if (!File.Exists(Path.Combine(gameDataFolder, Constants.MOD_LIST_FILE)))
            {
                FileStreamHelper.WriteFile(gameDataFolder, Constants.MOD_LIST_FILE, "[]", false);
            }
        }

        public static void DeleteDataFolder()
        {
            if (Directory.Exists(Constants.DATA_FOLDER))
            {
                Directory.Delete(Constants.DATA_FOLDER, true);
            }
        }

        public static void CreateModFolder(GameType type)
        {
            string gameModFolder = EnumSwitch.GetModFolder(type);
            if (!Directory.Exists(Path.Combine(Constants.MODS_FOLDER, gameModFolder)))
            {
                Directory.CreateDirectory(Path.Combine(Constants.MODS_FOLDER, gameModFolder));
            }
        }

        public static List<ModData> IndexModDirectory(GameType type)
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
                        string sha256 = FileStreamHelper.Sha256Checksum(modPath);
                        string md5 = FileStreamHelper.Md5Checksum(modPath);

                        modFiles.Add(new ModFile
                        {
                            LocalFilePath = "." + modPath.Substring(StringHelper.IndexOfNth(modPath, "\\", 2)),
                            SourceRelativePath = modPath,
                            SourceAbsolutePath = PathHelper.GetAbsolutePath(modPath),
                            SHA256 = sha256,
                            MD5 = md5
                        });

                        modList.Add(new ModData
                        {
                            Name = modInfo["name"],
                            Description = modInfo["description"],
                            Author = modInfo["author"],
                            Version = modInfo["version"],
                            LoadOrder = modIni["user"]["loadOrder"],
                            Path = modInfo["files"],
                            Guid = Guid.NewGuid().ToString(),
                            ModFiles = modFiles
                        });
                    }
                    else if (Directory.Exists(modPath))
                    {
                        List<ModFile> modFiles = new List<ModFile>();
                        foreach (string filePath in FileStreamHelper.GetFiles(modPath, "*.*", false))
                        {
                            string sha256 = FileStreamHelper.Sha256Checksum(filePath);
                            string md5 = FileStreamHelper.Md5Checksum(filePath);

                            modFiles.Add(new ModFile
                            {
                                LocalFilePath = "." + filePath.Substring(StringHelper.IndexOfNth(filePath, "\\", 2)),
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
                            LoadOrder = modIni["user"]["loadOrder"],
                            Path = modInfo["files"],
                            Guid = Guid.NewGuid().ToString(),
                            ModFiles = modFiles
                        });
                    }
                }
            }

            return modList.OrderBy(o => o.LoadOrder).ToList();
        }

        public static List<ModData> DeserializeModIndex(GameType type)
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

        public static void SaveModIndex(GameType type, List<ModData> modList)
        {
            string gameModFolder = EnumSwitch.GetModFolder(type);
            string gameDataFolder = Path.Combine(Constants.DATA_FOLDER, gameModFolder);
            FileStreamHelper.WriteFile(gameDataFolder, Constants.MOD_INDEX_FILE, JsonSerializer.Serialize(modList, new JsonSerializerOptions { WriteIndented = true }), false);
        }
    }
}