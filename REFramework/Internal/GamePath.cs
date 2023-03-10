using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using REFramework.Utils;
using REFramework.Data;
using REFramework.Configuration;
using REFramework.Configuration.Enums;

namespace REFramework.Internal
{
    public class GamePath
    {
        public static void SaveGamePath(GameType type)
        {
            string gameName = EnumSwitch.GetProcName(type);
            string gameModFolder = EnumSwitch.GetModFolder(type);
            int id = ProcessHelper.GetProcIdFromName(gameName);
            
            if (Directory.Exists(Constants.DATA_FOLDER))
            {
                Logger.Log(LogOpType.Write, new string[]
                {
                    $"{Path.Combine(Constants.DATA_FOLDER, gameModFolder)}, {Constants.GAME_PATH_STORE}, {ProcessHelper.GetProcPath(id).ToString()}"
                });
                FileStreamHelper.WriteFile(Path.Combine(Constants.DATA_FOLDER, gameModFolder), Constants.GAME_PATH_STORE, ProcessHelper.GetProcPath(id).ToString(), false);
            }
            else
            {
                Logger.Log(LogType.Error, new string[]
                {
                    $"Could not find directory \"{Constants.DATA_FOLDER}\" in function: SaveGamePath()"
                });
            }
        }

        public static string GetSavedGamePath(GameType type)
        {
            string gameModFolder = EnumSwitch.GetModFolder(type);
            string dataPath = Path.Combine(Constants.DATA_FOLDER, gameModFolder);
            if (File.Exists(Path.Combine(dataPath, Constants.GAME_PATH_STORE)))
            {
                byte[] bytes = FileStreamHelper.ReadFile(Path.Combine(dataPath, Constants.GAME_PATH_STORE));
                string file = FileStreamHelper.UnkBytesToStr(bytes);
                Logger.Log(LogOpType.Read, new string[]
                {
                    $"{bytes}, {file}, {Path.GetDirectoryName(file)}"
                });
                return Path.GetDirectoryName(file);
            }
            else
            {
                Logger.Log(LogType.Error, new string[]
                {
                    $"Could not find file \"{Path.Combine(dataPath, Constants.GAME_PATH_STORE)}\" in function: GetSavedGamePath()"
                });
            }

            return string.Empty;
        }
    }
}