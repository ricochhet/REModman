using REMod.Core.Configuration;
using REMod.Core.Configuration.Enums;
using REMod.Core.Configuration.Structs;
using REMod.Core.Integrations;
using REMod.Core.Providers;
using REMod.Core.Resolvers;
using REMod.Core.Utils;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace REMod.Core.Manager
{
    public class ManagerCache
    {
        public static void Save(GameType type, List<ModData> list)
        {
            FsProvider.WriteFile(
                PathResolver.DataPath(type),
                PathResolver.IndexFile,
                JsonSerializer.Serialize(list.OrderBy(i => i.LoadOrder).ToList(),
                new JsonSerializerOptions { WriteIndented = true }
            ));
        }

        public static List<ModData> Load(GameType type)
        {
            List<ModData> list = new();
            string dataFolder = Path.Combine(Constants.DATA_FOLDER, GameTypeResolver.Path(type));

            if (FsProvider.Exists(PathResolver.DataPath(type)) && FsProvider.Exists(PathResolver.IndexPath(type)))
            {
                byte[] bytes = FsProvider.ReadFile(Path.Combine(dataFolder, Constants.MOD_INDEX_FILE));
                string file = FsProvider.UnkBytesToStr(bytes);
                list = JsonSerializer.Deserialize<List<ModData>>(file);
            }

            return list;
        }

        public static ModData Find(List<ModData> list, string identifier) => list.Find(i => i.Hash == identifier);

        public static void SaveHashChanges(GameType type, List<ModData> list)
        {
            List<ModData> deserializedList = Load(type);
            List<ModData> listDiff = list.Where(p => !deserializedList.Any(l => p.Hash == l.Hash)).ToList();

            if (listDiff.Count != 0)
            {
                Save(type, list);
            }
        }

        public static void SaveAnyChanges(GameType type, List<ModData> list)
        {
            List<ModData> deserializedList = Load(type);
            List<ModData> listDiff = list.Where(p =>
                !deserializedList.Any(l => p.Hash == l.Hash) ||
                deserializedList.Any(l => p.IsEnabled != l.IsEnabled) ||
                deserializedList.Any(l => p.LoadOrder != l.LoadOrder)
            ).ToList();

            if (listDiff.Count != 0)
            {
                Save(type, list);
            }
        }

        private static string GetPathByType(GameType type, FileInfo fileInfo) => type switch
        {
            GameType.None => "",
            GameType.MonsterHunterRise => REEDataPatch.InstallPath(fileInfo),
            GameType.MonsterHunterWorld => MTFrameworkDataPatch.GetRelativeFromNatives(fileInfo),
            _ => throw new NotImplementedException(),
        };

        private static void AddToList(GameType type, ref List<ModFile> modFiles, ref string modHash, DirectoryInfo modItem, string gameDirectory)
        {
            FileInfo[] modItemFiles = FsProvider.GetFiles(modItem.FullName, "*.*", SearchOption.AllDirectories);

            foreach (FileInfo modItemFile in modItemFiles)
            {
                if (FsProvider.IsPathSafe(modItemFile.Name))
                {
                    string fileHash = CryptoHelper.FileHash.Sha256(modItemFile.FullName);
                    string currentInstallPath = GetPathByType(type, modItemFile);
                    modHash += fileHash;

                    if (!string.IsNullOrEmpty(currentInstallPath))
                    {
                        modFiles.Add(new ModFile
                        {
                            InstallPath = Path.Combine(gameDirectory, currentInstallPath),
                            SourcePath = modItemFile.FullName,
                            Hash = fileHash,
                        });
                    }
                }
            }
        }

        public static List<ModData> Build(GameType type)
        {
            List<ModData> list = Load(type);
            string gameDirectory = ManagerSettings.GetGamePath(type);
            string gameModDirectory = PathResolver.ModPath(type);

            if (FsProvider.Exists(gameModDirectory))
            {
                DirectoryInfo[] modDirectories = FsProvider.GetDirectories(gameModDirectory, "*.*", SearchOption.TopDirectoryOnly);

                foreach (DirectoryInfo modDirectory in modDirectories)
                {
                    string modHash = string.Empty;
                    List<ModFile> modFiles = new();
                    DirectoryInfo[] modItems = FsProvider.GetDirectories(modDirectory.FullName, "*.*", SearchOption.TopDirectoryOnly);

                    foreach (DirectoryInfo modItem in modItems)
                    {
                        if (type == GameType.MonsterHunterRise && (REEDataPatch.IsNatives(modItem.Name) || REEDataPatch.IsREF(modItem.Name)))
                        {
                            AddToList(type, ref modFiles, ref modHash, modItem, gameDirectory);
                        }
                        else if (type == GameType.MonsterHunterWorld && MTFrameworkDataPatch.IsNatives(modItem.Name))
                        {
                            AddToList(type, ref modFiles, ref modHash, modItem, gameDirectory);
                        }
                    }

                    FileInfo[] files = FsProvider.GetFiles(modDirectory.FullName, "*.*", SearchOption.TopDirectoryOnly);

                    foreach (FileInfo file in files)
                    {
                        if (type == GameType.MonsterHunterRise && REEDataPatch.IsValidPatchPak(file.FullName) && FsProvider.IsPathSafe(file.Name))
                        {
                            string fileHash = CryptoHelper.FileHash.Sha256(file.FullName);
                            modHash += fileHash;

                            modFiles.Add(new ModFile
                            {
                                InstallPath = Path.Combine(gameDirectory, file.Name),
                                SourcePath = file.FullName,
                                Hash = fileHash,
                            });
                        }
                    }

                    string identifier = CryptoHelper.StringHash.Sha256(modHash);

                    if (modFiles.Count != 0 && Find(list, identifier) == null)
                    {
                        string basePath = PathHelper.UnixPath(Path.Combine(gameModDirectory,
                            PathHelper.UnixPath(modDirectory.FullName)
                            .Split(gameModDirectory.TrimStart('.'))[1]
                            .TrimStart(Path.AltDirectorySeparatorChar)));

                        list.Add(new ModData
                        {
                            Name = Path.GetFileName(basePath),
                            Hash = identifier,
                            LoadOrder = 0,
                            BasePath = modDirectory.FullName,
                            IsEnabled = false,
                            Files = modFiles
                        });
                    }
                }
            }

            return list.OrderBy(i => i.LoadOrder).ToList();
        }
    }
}
