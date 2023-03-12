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

namespace REModman.Internal
{
    public class SetupManager
    {
        public static void CreateIndex(GameType type)
        {
            string gameModFolder = EnumSwitch.GetModFolder(type);
            string gameDataFolder = Path.Combine(Constants.DATA_FOLDER, gameModFolder);

            if (!File.Exists(Path.Combine(gameDataFolder, Constants.MOD_INDEX_FILE)))
            {
                FileStreamHelper.WriteFile(gameDataFolder, Constants.MOD_INDEX_FILE, "[]", false);
            }
        }

        public static void DeleteIndex(GameType type)
        {
            string gameModFolder = EnumSwitch.GetModFolder(type);
            string gameDataFolder = Path.Combine(Constants.DATA_FOLDER, gameModFolder);

            if (File.Exists(Path.Combine(gameDataFolder, Constants.MOD_INDEX_FILE)))
            {
                File.Delete(Path.Combine(gameDataFolder, Constants.MOD_INDEX_FILE));
            }
        }

        public static void CreateList(GameType type) 
        {
            string gameModFolder = EnumSwitch.GetModFolder(type);
            string gameDataFolder = Path.Combine(Constants.DATA_FOLDER, gameModFolder);

            if (!File.Exists(Path.Combine(gameDataFolder, Constants.MOD_LIST_FILE)))
            {
                FileStreamHelper.WriteFile(gameDataFolder, Constants.MOD_LIST_FILE, "[]", false);
            }
        }

        public static void DeleteList(GameType type)
        {
            string gameModFolder = EnumSwitch.GetModFolder(type);
            string gameDataFolder = Path.Combine(Constants.DATA_FOLDER, gameModFolder);

            if (File.Exists(Path.Combine(gameDataFolder, Constants.MOD_LIST_FILE)))
            {
                File.Delete(Path.Combine(gameDataFolder, Constants.MOD_LIST_FILE));
            }
        }

        public static void CreateSettings()
        {
            if (!File.Exists(Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE)))
            {
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
                File.Exists(Path.Combine(Constants.DATA_FOLDER, Constants.SETTINGS_FILE));
            }
        }

        public static void CreateModsFolder(GameType type)
        {
            string gameModFolder = EnumSwitch.GetModFolder(type);
            if (!Directory.Exists(Path.Combine(Constants.MODS_FOLDER, gameModFolder)))
            {
                Directory.CreateDirectory(Path.Combine(Constants.MODS_FOLDER, gameModFolder));
            }
        }

        public static void DeleteDataFolder(GameType type)
        {
            string gameModFolder = EnumSwitch.GetModFolder(type);
            string gameDataFolder = Path.Combine(Constants.DATA_FOLDER, gameModFolder);

            if (Directory.Exists(gameDataFolder))
            {
                Directory.Delete(gameDataFolder, true);
            }
        }

        public static bool DataFolderExists(GameType type)
        {
            string gameModFolder = EnumSwitch.GetModFolder(type);
            string gameDataFolder = Path.Combine(Constants.DATA_FOLDER, gameModFolder);

            if (Directory.Exists(gameDataFolder))
            {
                return true;
            }

            return false;
        }

        public static bool ModsFolderExists(GameType type)
        {
            string gameModFolder = EnumSwitch.GetModFolder(type);
            if (Directory.Exists(Path.Combine(Constants.MODS_FOLDER, gameModFolder)))
            {
                return true;
            }

            return false;
        }
    }
}
