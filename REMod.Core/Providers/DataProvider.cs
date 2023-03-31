using REMod.Core.Configuration.Enums;
using REMod.Core.Configuration.Structs;
using REMod.Core.Manager;
using REMod.Core.Resolvers;
using REMod.Core.Resolvers.Enums;
using System;
using System.Collections.Generic;

namespace REMod.Core.Providers
{
    public class DataProvider
    {
        public static void Create(FileType type, GameType game = GameType.None)
        {
            switch (type)
            {
                case FileType.Log:
                    throw new NotImplementedException();
                case FileType.Cache:
                    FsProvider.WriteFile(PathResolver.DataPath(game), PathResolver.IndexFile, "[]");
                    break;
                case FileType.Settings:
                    if (!FsProvider.Exists(PathResolver.SettingsPath))
                    {
                        ManagerSettings.Save(new SettingsData
                        {
                            LastSelectedGame = GameType.None,
                            GamePaths = new Dictionary<string, string>()
                        });
                    }
                    break;
                default:
                    break;
            }
        }

        public static void Create(FolderType type, GameType game = GameType.None)
        {
            switch (type)
            {
                case FolderType.Data:
                    FsProvider.CreateDirectory(PathResolver.DataPath(game));
                    break;
                case FolderType.Mods:
                    FsProvider.CreateDirectory(PathResolver.ModPath(game));
                    break;
                case FolderType.Downloads:
                    FsProvider.CreateDirectory(PathResolver.DownloadPath(game));
                    break;
                default:
                    break;
            }
        }

        public static void Delete(FileType type, GameType game = GameType.None)
        {
            switch (type)
            {
                case FileType.Log:
                    throw new NotImplementedException();
                case FileType.Cache:
                    FsProvider.DeleteFile(PathResolver.IndexPath(game));
                    break;
                case FileType.Settings:
                    FsProvider.DeleteFile(PathResolver.SettingsPath);
                    break;
                default:
                    break;
            }
        }

        public static void Delete(FolderType type, GameType game)
        {
            switch (type)
            {
                case FolderType.Data:
                    FsProvider.DeleteDirectory(PathResolver.DataPath(game), true);
                    break;
                case FolderType.Mods:
                    throw new NotImplementedException();
                case FolderType.Downloads:
                    throw new NotImplementedException();
                default:
                    break;
            }
        }

        public static bool Exists(FileType type, GameType game = GameType.None) => type switch
        {
            FileType.Log => FsProvider.Exists(PathResolver.LogPath),
            FileType.Cache => FsProvider.Exists(PathResolver.IndexPath(game)),
            FileType.Settings => FsProvider.Exists(PathResolver.SettingsPath),
            _ => throw new NotImplementedException()
        };

        public static bool Exists(FolderType type, GameType game = GameType.None) => type switch
        {
            FolderType.Data => FsProvider.Exists(PathResolver.DataPath(game)),
            FolderType.Mods => FsProvider.Exists(PathResolver.ModPath(game)),
            FolderType.Downloads => FsProvider.Exists(PathResolver.DownloadPath(game)),
            _ => throw new NotImplementedException()
        };
    }
}
