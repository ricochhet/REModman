using REMod.Helpers;
using REModman.Configuration.Enums;
using REModman.Configuration.Structs;
using REModman.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Common;

namespace REMod.Models
{
    public static class CollectionPageViewModel
    {
        public static ICommand InstallModCommand => new RelayCommand<ModInfoBridge>(InstallMod);

        private static void InstallMod(ModInfoBridge? data)
        {
            if (data != null)
            {
                if (data.Name == null || data.Guid == null || data.GameType == null)
                {
                    throw new NullReferenceException();
                }

                GameType gameType = (GameType)Enum.Parse(typeof(GameType), data.GameType ?? "None");

                if (gameType != GameType.None)
                {
                    List<ModData> selectedMods = Installer.SelectMods(Collection.DeserializeModIndex(gameType), new Dictionary<string, string>
                    {
                        { data.Name, data.Guid }
                    });

                    if (selectedMods.Count != 0)
                    {
                        List<ModData> firstInSelection = new List<ModData>
                        {
                            selectedMods[0]
                        };

                        if (Directory.Exists(Settings.GetGamePath(gameType)))
                        {
                            bool isModInstalled = false;
                            List<ModData> installedModList = Installer.DeserializeModList(gameType);

                            foreach (ModData installedMod in installedModList)
                            {
                                if (installedMod.Name == selectedMods[0].Name && installedMod.Guid == selectedMods[0].Guid)
                                {
                                    isModInstalled = true;
                                    break;
                                }
                            }

                            if (isModInstalled == false)
                            {
                                installedModList.Add(selectedMods[0]);
                                Installer.InstallMods(gameType, firstInSelection);
                                Installer.SaveModList(gameType, installedModList);
                                StatusNotifyHelper.Assign($"Installed mod: {data.Name} for {gameType}.");
                            }
                            else
                            {
                                StatusNotifyHelper.Assign($"Mod: {data.Name} is already installed for {gameType}.");
                            }
                        }
                        else
                        {
                            StatusNotifyHelper.Assign($"Could not find the game path for game: {gameType}.");
                        }
                    }
                    else
                    {
                        StatusNotifyHelper.Assign($"Could not get the selected mod: {data.Name}.");
                    }
                }
                else
                {
                    StatusNotifyHelper.Assign($"Please select a valid game.");
                }
            }
            else
            {
                throw new NullReferenceException();
            }
        }
    }
}
