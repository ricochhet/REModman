using REModman.Configuration;
using REModman.Configuration.Enums;
using REModman.Configuration.Structs;
using REModman.Logger;
using REModman.Utils;
using System.Collections.Generic;
using System.IO;

namespace REModman.Internal
{
    public class DataManager
    {
        public static void CreateIndex(GameType type)
        {
            string dataFolder = Path.Combine(Constants.DATA_FOLDER, EnumSwitch.GetModFolder(type));

            if (!File.Exists(Path.Combine(dataFolder, Constants.MOD_INDEX_FILE)))
            {
                FileStreamHelper.WriteFile(dataFolder, Constants.MOD_INDEX_FILE, "[]", false);
            }
        }

        public static void DeleteIndex(GameType type)
        {
            string dataFolder = Path.Combine(Constants.DATA_FOLDER, EnumSwitch.GetModFolder(type));

            if (File.Exists(Path.Combine(dataFolder, Constants.MOD_INDEX_FILE)))
            {
                LogBase.Info($"[DATAMANAGER] Attempting to delete file: {Path.Combine(dataFolder, Constants.MOD_INDEX_FILE)}.");
                File.Delete(Path.Combine(dataFolder, Constants.MOD_INDEX_FILE));
            }
        }

        public static void CreateSettings()
        {
            if (!File.Exists(Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE)))
            {
                LogBase.Info($"[DATAMANAGER] Attempting to create file: {Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE)}.");
                SettingsManager.SaveSettings(new SettingsData
                {
                    LastSelectedGame = GameType.None,
                    GamePaths = new Dictionary<string, string>()
                });
            }
        }

        public static void DeleteSettings()
        {
            if (!File.Exists(Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE)))
            {
                LogBase.Info($"[DATAMANAGER] Attempting to delete file: {Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE)}.");
                File.Delete(Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE));
            }
        }

        public static void CreateModsFolder(GameType type)
        {
            string modFolder = EnumSwitch.GetModFolder(type);

            if (!Directory.Exists(Path.Combine(Constants.MODS_FOLDER, modFolder)))
            {
                LogBase.Info($"[DATAMANAGER] Attempting to create folder: {Path.Combine(Constants.MODS_FOLDER, modFolder)}.");
                Directory.CreateDirectory(Path.Combine(Constants.MODS_FOLDER, modFolder));
            }
        }

        public static void DeleteDataFolder(GameType type)
        {
            string dataFolder = Path.Combine(Constants.DATA_FOLDER, EnumSwitch.GetModFolder(type));

            if (Directory.Exists(dataFolder))
            {
                LogBase.Info($"[DATAMANAGER] Attempting to delete folder: {dataFolder}.");
                Directory.Delete(dataFolder, true);
            }
        }

        public static bool DataFolderExists(GameType type)
        {
            string dataFolder = Path.Combine(Constants.DATA_FOLDER, EnumSwitch.GetModFolder(type));

            if (Directory.Exists(dataFolder))
            {
                LogBase.Info($"[DATAMANAGER] Folder exists: {dataFolder}.");
                return true;
            }

            return false;
        }

        public static bool ModsFolderExists(GameType type)
        {
            string modFolder = Path.Combine(Constants.MODS_FOLDER, EnumSwitch.GetModFolder(type));

            if (Directory.Exists(modFolder))
            {
                LogBase.Info($"[DATAMANAGER] Folder exists: {modFolder}.");
                return true;
            }

            return false;
        }

        public static bool IndexFileExists(GameType type)
        {
            string dataFolder = Path.Combine(Constants.DATA_FOLDER, EnumSwitch.GetModFolder(type));

            if (File.Exists(Path.Combine(dataFolder, Constants.MOD_INDEX_FILE)))
            {
                LogBase.Info($"[DATAMANAGER] Folder exists: {Path.Combine(dataFolder, Constants.MOD_INDEX_FILE)}.");
                return true;
            }

            return false;
        }

        public static bool SettingsFileExists()
        {
            if (File.Exists(Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE)))
            {
                LogBase.Info($"[DATAMANAGER] File exists: {Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE)}.");
                return true;
            }

            return false;
        }

        public static string GetModFolderPath(GameType type)
        {
            string modFolder = EnumSwitch.GetModFolder(type);

            if (Directory.Exists(Path.Combine(Constants.MODS_FOLDER, modFolder)))
            {
                LogBase.Info($"[DATAMANAGER] Folder exists: {Path.Combine(Constants.MODS_FOLDER, modFolder)}.");
                return Path.Combine(Constants.MODS_FOLDER, modFolder);
            }

            return string.Empty;
        }
    }
}
