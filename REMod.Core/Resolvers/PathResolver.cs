using REMod.Core.Configuration;
using REMod.Core.Configuration.Enums;
using REMod.Core.Resolvers.Enums;
using REMod.Core.Resolvers.Structs;
using System;
using System.IO;

namespace REMod.Core.Resolvers
{
    public class PathResolver
    {
        public static string LogFile => FileTypeStruct.Log;
        public static string IndexFile => FileTypeStruct.Cache;
        public static string SettingsFile => FileTypeStruct.Settings;
        public static string ModDir  => FolderTypeStruct.Mods;
        public static string DataDir => FolderTypeStruct.Data;
        public static string LogPath => Path.Combine(FolderTypeStruct.Data, FileTypeStruct.Log);
        public static string SettingsPath => Path.Combine(FolderTypeStruct.Data, FileTypeStruct.Settings);
        public static string DataPath(GameType type) => Path.Combine(FolderTypeStruct.Data, GameTypeResolver.Path(type));
        public static string ModPath(GameType type) => Path.Combine(FolderTypeStruct.Mods, GameTypeResolver.Path(type));
        public static string DownloadPath(GameType type) => Path.Combine(FolderTypeStruct.Downloads, GameTypeResolver.Path(type));
        public static string IndexPath(GameType type) => Path.Combine(FolderTypeStruct.Data, GameTypeResolver.Path(type), FileTypeStruct.Cache);

        public static string Folder(FolderType folder, GameType type) => folder switch
        {
            FolderType.Data => DataPath(type),
            FolderType.Mods => ModPath(type),
            FolderType.Downloads => DownloadPath(type),
            _ => throw new NotImplementedException(),
        };

        public static string File(FileType file, GameType type = GameType.None) => file switch
        {
            FileType.Log => LogPath,
            FileType.Cache => IndexPath(type),
            FileType.Settings => SettingsPath,
            _ => throw new NotImplementedException(),
        };
    }
}
