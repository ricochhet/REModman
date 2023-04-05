using REMod.Core.Configuration;
using REMod.Core.Configuration.Enums;
using REMod.Core.Configuration.Structs;
using REMod.Core.Logger;
using REMod.Core.Providers;
using REMod.Core.Resolvers;
using REMod.Core.Utils;
using System.IO;
using System.Text.Json;

namespace REMod.Core.Manager
{
    public class ManagerSettings
    {
        public static void Save(SettingsData settingsData)
        {
            FsProvider.WriteFile(Constants.DATA_FOLDER, Constants.SETTINGS_FILE, JsonSerializer.Serialize(settingsData, new JsonSerializerOptions { WriteIndented = true }));
        }

        private static SettingsData Load()
        {
            SettingsData settingsData = new();
            if (Directory.Exists(Constants.DATA_FOLDER))
            {
                if (File.Exists(Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE)))
                {
                    byte[] bytes = FsProvider.ReadFile(Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE));
                    string file = FsProvider.UnkBytesToStr(bytes);
                    settingsData = JsonSerializer.Deserialize<SettingsData>(file);
                }
            }

            return settingsData;
        }

        public static GameType GetLastSelectedGame()
        {
            if (FsProvider.Exists(PathResolver.SettingsPath))
            {
                SettingsData settingsData = Load();
                return settingsData.LastSelectedGame;
            }

            return GameType.None;
        }

        public static string GetGamePath(GameType type)
        {
            if (FsProvider.Exists(PathResolver.SettingsPath))
            {
                SettingsData settingsData = Load();
                if (settingsData.GamePaths.TryGetValue(type.ToString(), out string value))
                {
                    return Path.GetDirectoryName(value);
                }
            }

            return string.Empty;
        }

        public static void SaveLastSelectedGame(GameType type)
        {
            if (FsProvider.Exists(PathResolver.SettingsPath))
            {
                SettingsData settingsData = Load();
                settingsData.LastSelectedGame = type;

                LogBase.Info($"Saving last selected game {type}.");
                Save(settingsData);
            }
        }

        public static void SaveGamePath(GameType type)
        {
            if (ProcessHelper.IsProcRunning(type) && FsProvider.Exists(PathResolver.SettingsPath))
            {
                SettingsData settingsData = Load();
                if (!settingsData.GamePaths.ContainsKey(type.ToString()))
                {
                    LogBase.Info($"Saving game path for {type}.");
                    settingsData.GamePaths.Add(type.ToString(), ProcessHelper.GetProcPath(type).ToString());
                }

                Save(settingsData);
            }
        }
    }
}
