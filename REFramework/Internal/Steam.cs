using System;
using System.Diagnostics;
using REFramework.Configuration;
using REFramework.Configuration.Enums;

namespace REFramework.Internal
{
    public class Steam
    {
        public static void RunGameBy(GameType type)
        {
            string appId = type switch
            {
                GameType.Rise => Constants.MONSTER_HUNTER_RISE_APP_ID,
                GameType.World => Constants.MONSTER_HUNTER_WORLD_APP_ID,
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