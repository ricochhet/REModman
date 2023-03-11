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
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Common;

namespace REMod.Models
{
    public static class InstalledPageViewModel
    {
        public static ICommand UninstallModCommand => new RelayCommand<ModInfoBridge>(UninstallMod);

        private static void UninstallMod(ModInfoBridge? data)
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
                    List<ModData> selectedMods = Installer.SelectMods(Installer.DeserializeModList(gameType), new Dictionary<string, string>
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

                            if (isModInstalled == true)
                            {
                                ModData? temp = installedModList.Find(i => i.Name == selectedMods[0].Name && i.Guid == selectedMods[0].Guid);
                                if (temp != null)
                                {
                                    installedModList.Remove(temp);
                                }
                                //.Remove(selectedMods[0]);
                                Debug.Write(JsonSerializer.Serialize(installedModList, new JsonSerializerOptions { WriteIndented = true }));
                                // Installer.UninstallMods(gameType, firstInSelection);
                                // Installer.SaveModList(gameType, installedModList);
                            }
                            else
                            {
                                StatusNotifyHelper.Assign($"Mod: {data.Name} is not installed for {gameType}.");
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
