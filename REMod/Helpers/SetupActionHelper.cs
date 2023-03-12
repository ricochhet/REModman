using REMod.Helpers;
using REModman.Configuration.Enums;
using REModman.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace REMod.Helpers
{
    public class SetupActionHelper
    {
        public static void GetAction(SetupType setupType, GameType gameType)
        {
            if (gameType == GameType.None)
            {
                StatusNotifyHelper.Assign($"Please select a valid game.");
            }
            else
            {
                switch (setupType)
                {
                    case SetupType.CREATE_INDEX:
                        SetupManager.CreateIndex(gameType);
                        break;
                    case SetupType.DELETE_INDEX:
                        SetupManager.DeleteIndex(gameType);
                        break;
                    case SetupType.CREATE_LIST:
                        SetupManager.CreateList(gameType);
                        break;
                    case SetupType.DELETE_LIST:
                        SetupManager.DeleteList(gameType);
                        break;
                    case SetupType.CREATE_SETTINGS:
                        SetupManager.CreateSettings();
                        break;
                    case SetupType.DELETE_SETTINGS:
                        SetupManager.DeleteSettings();
                        break;
                    case SetupType.CREATE_MODS_FOLDER:
                        SetupManager.CreateModsFolder(gameType);
                        break;
                    case SetupType.DELETE_DATA_FOLDER:
                        SetupManager.DeleteDataFolder(gameType);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
