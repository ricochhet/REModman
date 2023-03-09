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

namespace REFramework.Configuration
{
    public class EnumSwitch
    {
        public static string GetModFolder(GameType type)
        {
            return type switch
            {
                GameType.MonsterHunterRise => Constants.MONSTER_HUNTER_RISE_MOD_FOLDER,
                GameType.MonsterHunterWorld => Constants.MONSTER_HUNTER_WORLD_MOD_FOLDER,
                _ => throw new NotImplementedException(),
            };
        }

        public static string GetProcName(GameType type)
        {
            return type switch
            {
                GameType.MonsterHunterRise => Constants.MONSTER_HUNTER_RISE_PROC_NAME,
                GameType.MonsterHunterWorld => Constants.MONSTER_HUNTER_WORLD_PROC_NAME,
                _ => throw new NotImplementedException(),
            };
        }
    }
}