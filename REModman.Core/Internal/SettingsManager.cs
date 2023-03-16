using REModman.Configuration;
using REModman.Configuration.Enums;
using REModman.Configuration.Structs;
using REModman.Logger;
using REModman.Utils;
using System;
using System.IO;
using System.Text.Json;

namespace REModman.Internal
{
    public class SettingsManager
    {
        public static void SaveLastSelectedGame(GameType type)
        {
            if (File.Exists(Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE)))
            {
                SettingsData settingsData = DeserializeSettings();
                settingsData.LastSelectedGame = type;
                LogBase.Info($"Saving last selected game {type}.");
                SaveSettings(settingsData);
            }
        }

        public static GameType GetLastSelectedGame()
        {
            GameType lastSelectedGame = GameType.None;

            if (File.Exists(Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE)))
            {
                SettingsData settingsData = DeserializeSettings();
                lastSelectedGame = settingsData.LastSelectedGame;
                LogBase.Info($"Getting last selected game {settingsData.LastSelectedGame}.");
            }

            return lastSelectedGame;
        }

        public static void SaveGamePath(GameType type)
        {
            int id = ProcessHelper.GetProcIdFromName(EnumSwitch.GetProcName(type));

            if (id != 0)
            {
                if (File.Exists(Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE)))
                {
                    SettingsData settingsData = DeserializeSettings();

                    if (!settingsData.GamePaths.ContainsKey(type.ToString()))
                    {
                        LogBase.Info($"Saving game path for {type}.");
                        settingsData.GamePaths.Add(type.ToString(), ProcessHelper.GetProcPath(id).ToString());
                    }

                    SaveSettings(settingsData);
                }
            }
        }

        public static bool IsGameRunning(GameType type)
        {
            int id = ProcessHelper.GetProcIdFromName(EnumSwitch.GetProcName(type));

            if (id != 0)
            {
                LogBase.Info($"{type} is running.");
                return true;
            }

            return false;
        }

        public static string GetGamePath(GameType type)
        {
            string gamePath = string.Empty;

            if (File.Exists(Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE)))
            {
                SettingsData settingsData = DeserializeSettings();
                if (settingsData.GamePaths.ContainsKey(type.ToString()))
                {
                    LogBase.Info($"Getting game path for {type}.");
                    gamePath = Path.GetDirectoryName(settingsData.GamePaths[type.ToString()]);
                }
            }

            return gamePath;
        }

        private static SettingsData DeserializeSettings()
        {
            SettingsData settingsData = new();
            if (Directory.Exists(Constants.DATA_FOLDER))
            {
                if (File.Exists(Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE)))
                {
                    byte[] bytes = FileStreamHelper.ReadFile(Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE), false);
                    string file = FileStreamHelper.UnkBytesToStr(bytes);
                    settingsData = JsonSerializer.Deserialize<SettingsData>(file);
                }
            }

            return settingsData;
        }

        public static void SaveSettings(SettingsData settingsData)
        {
            FileStreamHelper.WriteFile(Constants.DATA_FOLDER, Constants.SETTINGS_FILE, JsonSerializer.Serialize(settingsData, new JsonSerializerOptions { WriteIndented = true }), false);
        }
    }
}
