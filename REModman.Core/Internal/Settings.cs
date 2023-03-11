﻿using REModman.Configuration.Enums;
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
    public class Settings
    {
        public static void SaveGamePath(GameType type)
        {
            string gameName = EnumSwitch.GetProcName(type);
            int id = ProcessHelper.GetProcIdFromName(gameName);

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
