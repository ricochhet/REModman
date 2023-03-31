using REMod.Core.Configuration.Enums;
using System;

namespace REMod.Core.Configuration
{
    public class GameTypeResolver
    {
        public static string Path(GameType type) => type switch
        {
            GameType.None => throw new NotImplementedException(),
            GameType.MonsterHunterRise => Constants.MONSTER_HUNTER_RISE_MOD_FOLDER,
            GameType.MonsterHunterWorld => Constants.MONSTER_HUNTER_WORLD_MOD_FOLDER,
            _ => throw new NotImplementedException(),
        };

        public static string ProcessName(GameType type) => type switch
        {
            GameType.None => throw new NotImplementedException(),
            GameType.MonsterHunterRise => Constants.MONSTER_HUNTER_RISE_PROC_NAME,
            GameType.MonsterHunterWorld => Constants.MONSTER_HUNTER_WORLD_PROC_NAME,
            _ => throw new NotImplementedException(),
        };
    }
}