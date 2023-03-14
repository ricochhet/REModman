using REModman.Configuration;
using REModman.Configuration.Enums;
using System;
using System.Diagnostics;

namespace REModman.Internal
{
    public class Steam
    {
        public static void RunGameBy(GameType type)
        {
            string appId = type switch
            {
                GameType.MonsterHunterRise => Constants.MONSTER_HUNTER_RISE_APP_ID,
                GameType.MonsterHunterWorld => Constants.MONSTER_HUNTER_WORLD_APP_ID,
                _ => throw new NotImplementedException(),
            };

            try
            {
                _ = Process.Start(new ProcessStartInfo()
                {
                    FileName = $"steam://run/{appId}",
                    UseShellExecute = true
                });
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
        }
    }
}