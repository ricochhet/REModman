using REModman.Configuration.Enums;
using REModman.Data;
using REModman.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Common;

namespace Wpf.Ui.Demo.Simple.Models
{
    public static class CollectionPageViewModel
    {
        public static ICommand InstallModCommand => new RelayCommand<ModInfoBridge>(InstallMod);

        private static void InstallMod(ModInfoBridge? data)
        {
            if (data != null)
            {
                if (data.Name == null || data.Guid == null || data.GameType == null || data.LogBox == null)
                {
                    throw new NullReferenceException();
                }

                GameType gameType = (GameType)Enum.Parse(typeof(GameType), data.GameType ?? "None");
                if (gameType != GameType.None)
                {
                    List<ModData> selectedMods = ModController.SelectMods(ModIndexer.DeserializeModIndex(gameType), new Dictionary<string, string>
                    {
                        { data.Name, data.Guid }
                    });

                    if (selectedMods.Count != 0)
                    {
                        List<ModData> firstInSelection = new List<ModData>
                        {
                            selectedMods[0]
                        };

                        if (Directory.Exists(GamePath.GetSavedGamePath(gameType)))
                        {
                            bool isModInstalled = false;
                            List<ModData> installedModList = ModController.DeserializeModList(gameType);

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
                                ModController.InstallMods(gameType, firstInSelection);
                                ModController.SaveModList(gameType, installedModList);
                                data.LogBox.Text = $"Installed mod: {data.Name} for {gameType}.";
                            }
                            else
                            {
                                data.LogBox.Text = $"Mod: {data.Name} is already installed for {gameType}.";
                            }
                        }
                        else
                        {
                            data.LogBox.Text = $"Could not find the game path for game: {gameType}.";
                        }
                    }
                    else
                    {
                        data.LogBox.Text = $"Could not get the selected mod: {data.Name}.";
                    }
                }
                else
                {
                    data.LogBox.Text = $"Please select a valid game.";
                }
            }
            else
            {
                throw new NullReferenceException();
            }
        }
    }
}
