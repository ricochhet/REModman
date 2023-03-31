using REMod.Core.Configuration.Enums;
using REMod.Core.Configuration.Structs;
using REMod.Core.Integrations;
using REMod.Core.Logger;
using REMod.Core.Providers;
using REMod.Core.Resolvers;
using System;
using System.Collections.Generic;

namespace REMod.Core.Manager
{
    public class ModInstaller
    {
        public static void SetLoadOrder(GameType type, string identifier, int value)
        {
            List<ModData> list = ManagerCache.Load(type);
            if (list == null || list.Count == 0) return;

            ModData mod = ManagerCache.Find(list, identifier);
            if (mod == null) return;
            if (mod.LoadOrder == value) return;

            mod.LoadOrder = value;
            ManagerCache.SaveAnyChanges(type, list);
        }

        public static int GetLoadOrder(GameType type, string identifier)
        {
            List<ModData> list = ManagerCache.Load(type);
            if (list == null || list.Count == 0) return 0;

            ModData mod = ManagerCache.Find(list, identifier);
            if (mod == null) return 0;

            return mod.LoadOrder;
        }

        public static void Enable(GameType type, string identifier, bool isEnabled)
        {
            List<ModData> list = ManagerCache.Load(type);
            if (list == null || list.Count == 0) return;

            ModData mod = ManagerCache.Find(list, identifier);
            if (mod == null) return;
            if (mod.IsEnabled == isEnabled) return;

            mod.IsEnabled = isEnabled;
            bool containsValidPaks = REEDataPatch.HasValidPatchPaks(mod);

            if (isEnabled)
            {
                if (containsValidPaks)
                {
                    list = REEDataPatch.Patch(list);
                }

                Install(type, mod);
            }
            else
            {
                Uninstall(type, mod);

                if (containsValidPaks)
                {
                    list = REEDataPatch.Patch(list);
                }
            }

            ManagerCache.SaveAnyChanges(type, list);
        }

        private static void Install(GameType type, ModData mod)
        {
            if (FsProvider.Exists(ManagerSettings.GetGamePath(type)))
            {
                LogBase.Info($"Attempting to install mod: {mod.Name}.");
                foreach (ModFile file in mod.Files)
                {
                    FsProvider.CopyFile(file.SourcePath, file.InstallPath);
                }
            }
        }

        private static void Uninstall(GameType type, ModData mod)
        {
            if (FsProvider.Exists(ManagerSettings.GetGamePath(type)))
            {
                LogBase.Info($"Attempting to uninstall mod: {mod.Name}.");
                foreach (ModFile file in mod.Files)
                {
                    if (FsProvider.Exists(file.InstallPath))
                    {
                        LogBase.Info($"Removing file: {file.InstallPath}.");

                        try
                        {
                            FsProvider.DeleteFile(file.InstallPath);
                        }
                        catch (Exception e)
                        {
                            LogBase.Error($"Failed to remove file: {file.InstallPath}.");
                            LogBase.Error(e.ToString());
                        }
                    }
                }

                LogBase.Info($"Cleaning folder: {ManagerSettings.GetGamePath(type)}.");
                FsProvider.DeleteEmptyDirectories(ManagerSettings.GetGamePath(type));
            }
        }

        public static void Delete(GameType type, string identifier)
        {
            List<ModData> list = ManagerCache.Load(type);
            if (list == null || list.Count == 0) return;

            ModData mod = ManagerCache.Find(list, identifier);
            if (mod == null) return;

            Enable(type, mod.Hash, false);
            LogBase.Info($"Attempting to delete mod: {mod.Name}.");

            if (FsProvider.Exists(mod.BasePath))
            {
                try
                {
                    FsProvider.DeleteDirectory(mod.BasePath, true);
                }
                catch (Exception e)
                {
                    LogBase.Error($"Failed to remove directory: {mod.BasePath}.");
                    LogBase.Error(e.ToString());
                }
            }

            list.Remove(ManagerCache.Find(list, mod.Hash));
            ManagerCache.Save(type, list);
            FsProvider.DeleteEmptyDirectories(PathResolver.ModDir);
        }
    }
}
