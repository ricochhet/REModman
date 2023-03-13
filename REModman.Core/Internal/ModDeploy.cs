using IniParser;
using REModman.Configuration.Enums;
using REModman.Configuration.Structs;
using REModman.Configuration;
using REModman.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser.Model;
using System.Text.Json;
using REModman.Patches;

namespace REModman.Internal
{
    public class ModDeploy
    {
        private static List<ModData> Deserialize(GameType type)
        {
            List<ModData> modList = new List<ModData>();
            string modFolder = Path.Combine(Constants.DATA_FOLDER, EnumSwitch.GetModFolder(type));

            if (Directory.Exists(modFolder))
            {
                if (File.Exists(Path.Combine(modFolder, Constants.MOD_INDEX_FILE)))
                {
                    byte[] bytes = FileStreamHelper.ReadFile(Path.Combine(modFolder, Constants.MOD_INDEX_FILE));
                    string file = FileStreamHelper.UnkBytesToStr(bytes);
                    modList = JsonSerializer.Deserialize<List<ModData>>(file);
                }
            }

            return modList;
        }

        private static ModData Find(List<ModData> list, string identifier)
        {
            return list.Find(i => i.Guid == identifier);
        }

        private static ModFile Find(List<ModFile> list, string identifier)
        {
            return list.Find(i => i.SHA256 == identifier);
        }

        private static bool Exists(List<ModData> list, string identifier)
        {
            if (Find(list, identifier) == null)
            {
                return false;
            }

            return true;
        }

        public static void Save(GameType type, List<ModData> modList)
        {
            string modFolder = Path.Combine(Constants.DATA_FOLDER, EnumSwitch.GetModFolder(type));
            FileStreamHelper.WriteFile(modFolder, Constants.MOD_INDEX_FILE, JsonSerializer.Serialize(modList, new JsonSerializerOptions { WriteIndented = true }), false);
        }

        public static List<ModData> Index(GameType type)
        {
            List<ModData> modList = Deserialize(type);
            IniDataParser parser = new IniDataParser();

            string gamePath = SettingsManager.GetGamePath(type);
            string modFolder = Path.Combine(Constants.MODS_FOLDER, EnumSwitch.GetModFolder(type));

            if (Directory.Exists(modFolder))
            {
                foreach (string infoFile in FileStreamHelper.GetFiles(modFolder, Constants.MOD_INFO_FILE, false))
                {
                    byte[] infoBytes = FileStreamHelper.ReadFile(infoFile);
                    string infoData = FileStreamHelper.UnkBytesToStr(infoBytes);

                    IniData modIni = parser.Parse(infoData);
                    PropertyCollection modInfo = modIni["modinfo"];
                    
                    string modPath = Path.GetDirectoryName(infoFile);
                    List<ModFile> modFiles = new List<ModFile>();
                    string modSha256 = string.Empty;

                    foreach (string modFilePath in FileStreamHelper.GetFiles(modPath, "*.*", false))
                    {
                        string localPath = "." + modFilePath.Substring(StringHelper.IndexOfNth(modFilePath, "\\", 2));
                        string basePath = "." + modFilePath.Substring(StringHelper.IndexOfNth(modFilePath, "\\", 3));
                        string sha256 = CryptoHelper.FileHash.Sha256(modFilePath);
                        modSha256 += sha256;

                        modFiles.Add(new ModFile
                        {
                            LocalFilePath = localPath,
                            SourceRelativePath = modFilePath,
                            SourceAbsolutePath = PathHelper.GetAbsolutePath(modFilePath),
                            InstallAbsolutePath = PathHelper.GetAbsolutePath(Path.Combine(gamePath, basePath)),
                            InstallRelativePath = Path.Combine(gamePath, basePath),
                            SHA256 = sha256,
                        });
                    }

                    string identifier = CryptoHelper.StringHash.Sha256(modSha256);

                    if (!Exists(modList, identifier))
                    {
                        modList.Add(new ModData
                        {
                            Name = modInfo["name"],
                            Description = modInfo["description"],
                            Author = modInfo["author"],
                            Version = modInfo["version"],
                            LoadOrder = modIni["user"]["loadOrder"],
                            Path = modInfo["files"],
                            Guid = identifier,
                            IsEnabled = false,
                            ModFiles = modFiles
                        });
                    }
                }
            }

            return modList;
        }

        public static void Enable(GameType type, string identifier, bool isEnabled)
        {
            List<ModData> data = Deserialize(type);
            ModData mod = Find(data, identifier);
            mod.IsEnabled = isEnabled;

            Save(type, data);
        }

        public static void Remove(GameType type, string identifier)
        {
            List<ModData> data = Deserialize(type);
            data.Remove(Find(data, identifier));

            Save(type, data);
        }

        public static void Patch(GameType type)
        {
            List<ModData> data = Deserialize(type);

            foreach (ModData mod in data)
            {
                List<ModFile> patchable = new List<ModFile>();
                List<ModFile> nonPatchable = new List<ModFile>();

                foreach (ModFile file in mod.ModFiles)
                {
                    if (REChunkPatchPak.IsPatchable(type, file.LocalFilePath))
                    {
                        patchable.Add(file);
                    }
                    else
                    {
                        nonPatchable.Add(file);
                    }
                }

                for (int i = 0; i < patchable.Count; i++)
                {
                    patchable[i].InstallRelativePath = patchable[i].InstallRelativePath.Replace(REChunkPatchPak.CHUNK_PATCH_PAK_DEFAULT, REChunkPatchPak.Patch(i));
                    patchable[i].InstallAbsolutePath = patchable[i].InstallAbsolutePath.Replace(REChunkPatchPak.CHUNK_PATCH_PAK_DEFAULT, REChunkPatchPak.Patch(i));
                }

                mod.ModFiles = patchable.Concat(nonPatchable).ToList();
            }

            Save(type, data);
        }
    }
}
