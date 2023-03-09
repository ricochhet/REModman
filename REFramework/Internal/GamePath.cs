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
            string gameName = type switch
            {
                GameType.MonsterHunterRise => Constants.MONSTER_HUNTER_RISE_PROC_NAME,
                GameType.MonsterHunterWorld => Constants.MONSTER_HUNTER_WORLD_PROC_NAME,
                _ => throw new NotImplementedException(),
            };

            string gameModFolder = type switch
            {
                GameType.MonsterHunterRise => Constants.MONSTER_HUNTER_RISE_MOD_FOLDER,
                GameType.MonsterHunterWorld => Constants.MONSTER_HUNTER_WORLD_MOD_FOLDER,
                _ => throw new NotImplementedException(),
            };

            int id = ProcessHelper.GetProcIdFromName(gameName);
            
            if (Directory.Exists(Constants.DATA_FOLDER))
            {
                FileStreamHelper.WriteFile(Path.Combine(Constants.DATA_FOLDER, gameModFolder), Constants.GAME_PATH_STORE, ProcessHelper.GetProcPath(id).ToString(), false);
            }
        }

        public static string GetSavedGamePath(GameType type)
        {
            string gameModFolder = type switch
            {
                GameType.MonsterHunterRise => Constants.MONSTER_HUNTER_RISE_MOD_FOLDER,
                GameType.MonsterHunterWorld => Constants.MONSTER_HUNTER_WORLD_MOD_FOLDER,
                _ => throw new NotImplementedException(),
            };

            string dataPath = Path.Combine(Constants.DATA_FOLDER, gameModFolder);
            if (File.Exists(Path.Combine(dataPath, Constants.GAME_PATH_STORE)))
            {
                byte[] bytes = FileStreamHelper.ReadFile(Path.Combine(dataPath, Constants.GAME_PATH_STORE));
                string file = FileStreamHelper.UnkBytesToStr(bytes);
                return Path.GetDirectoryName(file);
            }

            return string.Empty;
        }
    }
}