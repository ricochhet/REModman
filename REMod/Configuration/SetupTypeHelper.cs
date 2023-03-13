using REMod.Dialogs;
using REModman.Configuration.Enums;
using REModman.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace REMod.Configuration
{
    public class SetupTypeHelper
    {
        public static void Execute(SetupType setupType, GameType gameType)
        {
            if (gameType == GameType.None)
            {
                BaseDialog dialog = new BaseDialog("Configuration Error", $"Please select a valid game.");
                dialog.Show();
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
