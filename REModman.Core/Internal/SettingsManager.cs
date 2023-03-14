using REModman.Configuration.Enums;
using REModman.Configuration;
using REModman;
using REModman.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using REModman.Configuration.Structs;

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
                    gamePath = Path.GetDirectoryName(settingsData.GamePaths[type.ToString()]);
                }
            }

            return gamePath;
        }

        private static SettingsData DeserializeSettings()
        {
            SettingsData settingsData = new SettingsData();
            if (Directory.Exists(Constants.DATA_FOLDER))
            {
                if (File.Exists(Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE)))
                {
                    byte[] bytes = FileStreamHelper.ReadFile(Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE));
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
