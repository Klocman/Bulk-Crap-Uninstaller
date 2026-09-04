/*
    OpenUninstall Pro - Open Source Professional Windows Uninstaller
    Game Launchers Discovery Factory (Epic, GOG, Ubisoft, Origin/EA, Battle.net)
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Win32;
using UninstallTools.Core;
using UninstallTools.Factory;
using UninstallTools.RegistryEngine;

namespace UninstallTools.Detection
{
    public sealed class GameLauncherFactory : IUninstallerFactory
    {
        public IList<ApplicationUninstallerEntry> GetUninstallerEntries(ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            var results = new List<ApplicationUninstallerEntry>();
            StructuredLogger.Info(LogCategory.Discovery, "Discovering games from game launchers");

            try
            {
                // 1. Epic Games Store
                ScanEpicGames(results);

                // 2. GOG Galaxy
                ScanGogGalaxy(results);

                // 3. Ubisoft Connect / Uplay
                ScanUbisoftConnect(results);

                // 4. EA Desktop / Origin
                ScanEaOrigin(results);

                // 5. Battle.net
                ScanBattleNet(results);
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Discovery, "Error scanning game launchers", ex.Message);
            }

            StructuredLogger.Info(LogCategory.Discovery, $"Discovered {results.Count} games from third-party launchers.");
            return results;
        }

        private static void ScanEpicGames(List<ApplicationUninstallerEntry> results)
        {
            var progData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            if (string.IsNullOrEmpty(progData)) return;

            var epicManifestsDir = Path.Combine(progData, "Epic", "EpicGamesLauncher", "Data", "Manifests");
            if (!Directory.Exists(epicManifestsDir)) return;

            try
            {
                foreach (var itemFile in Directory.GetFiles(epicManifestsDir, "*.item"))
                {
                    try
                    {
                        var json = File.ReadAllText(itemFile);
                        using var doc = JsonDocument.Parse(json);
                        var root = doc.RootElement;

                        var displayName = root.TryGetProperty("DisplayName", out var dProp) ? dProp.GetString() : null;
                        var appName = root.TryGetProperty("AppName", out var aProp) ? aProp.GetString() : null;
                        var installLoc = root.TryGetProperty("InstallLocation", out var lProp) ? lProp.GetString() : null;
                        var appVersion = root.TryGetProperty("AppVersionString", out var vProp) ? vProp.GetString() : null;

                        if (!string.IsNullOrWhiteSpace(displayName) && !string.IsNullOrWhiteSpace(installLoc))
                        {
                            var entry = new ApplicationUninstallerEntry
                            {
                                DisplayName = displayName,
                                DisplayVersion = appVersion,
                                Publisher = "Epic Games",
                                InstallLocation = installLoc,
                                UninstallerKind = UninstallerType.EpicGames,
                                UninstallPossible = true,
                                QuietUninstallPossible = true,
                                UninstallString = $"\"com.epicgames.launcher://apps/{appName}?action=uninstall\"",
                                QuietUninstallString = $"\"com.epicgames.launcher://apps/{appName}?action=uninstall\"",
                                IsRegistered = true,
                                RatingId = $"EpicGames_{appName ?? displayName}"
                            };
                            results.Add(entry);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Discovery, "Failed to scan Epic Games manifests", ex.Message);
            }
        }

        private static void ScanGogGalaxy(List<ApplicationUninstallerEntry> results)
        {
            var gogRegRoots = new[]
            {
                @"HKEY_LOCAL_MACHINE\Software\GOG.com\Games",
                @"HKEY_LOCAL_MACHINE\Software\WOW6432Node\GOG.com\Games"
            };

            foreach (var root in gogRegRoots)
            {
                using var key = SafeRegistryEngine.OpenKey(root);
                if (key == null) continue;

                foreach (var gameId in key.GetSubKeyNames())
                {
                    using var subKey = key.OpenSubKey(gameId);
                    if (subKey == null) continue;

                    var gameTitle = subKey.GetValue("gameName")?.ToString() ?? subKey.GetValue("GAMENAME")?.ToString();
                    var path = subKey.GetValue("path")?.ToString() ?? subKey.GetValue("PATH")?.ToString();
                    var uninstaller = subKey.GetValue("uninstallCommand")?.ToString();
                    var version = subKey.GetValue("version")?.ToString();

                    if (!string.IsNullOrWhiteSpace(gameTitle) && !string.IsNullOrWhiteSpace(path))
                    {
                        var entry = new ApplicationUninstallerEntry
                        {
                            DisplayName = gameTitle,
                            DisplayVersion = version,
                            Publisher = "GOG.com",
                            InstallLocation = path,
                            UninstallerKind = UninstallerType.InnoSetup,
                            UninstallString = uninstaller ?? (File.Exists(Path.Combine(path, "unins000.exe")) ? Path.Combine(path, "unins000.exe") : null),
                            QuietUninstallString = uninstaller != null ? $"{uninstaller} /VERYSILENT /SUPPRESSMSGBOXES /NORESTART" : null,
                            UninstallPossible = true,
                            QuietUninstallPossible = true,
                            IsRegistered = true,
                            RatingId = $"GOG_{gameId}"
                        };
                        results.Add(entry);
                    }
                }
            }
        }

        private static void ScanUbisoftConnect(List<ApplicationUninstallerEntry> results)
        {
            var uplayRoots = new[]
            {
                @"HKEY_LOCAL_MACHINE\Software\Ubisoft\Launcher\Installs",
                @"HKEY_LOCAL_MACHINE\Software\WOW6432Node\Ubisoft\Launcher\Installs"
            };

            foreach (var root in uplayRoots)
            {
                using var key = SafeRegistryEngine.OpenKey(root);
                if (key == null) continue;

                foreach (var gameId in key.GetSubKeyNames())
                {
                    using var subKey = key.OpenSubKey(gameId);
                    if (subKey == null) continue;

                    var installDir = subKey.GetValue("InstallDir")?.ToString();
                    if (!string.IsNullOrWhiteSpace(installDir) && Directory.Exists(installDir))
                    {
                        var dirName = Path.GetFileName(installDir.TrimEnd('\\', '/'));
                        var entry = new ApplicationUninstallerEntry
                        {
                            DisplayName = dirName,
                            Publisher = "Ubisoft",
                            InstallLocation = installDir,
                            UninstallerKind = UninstallerType.Uplay,
                            UninstallString = $"\"uplay://uninstall/{gameId}\"",
                            QuietUninstallString = $"\"uplay://uninstall/{gameId}\"",
                            UninstallPossible = true,
                            QuietUninstallPossible = true,
                            IsRegistered = true,
                            RatingId = $"Ubisoft_{gameId}"
                        };
                        results.Add(entry);
                    }
                }
            }
        }

        private static void ScanEaOrigin(List<ApplicationUninstallerEntry> results)
        {
            var progData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            if (string.IsNullOrEmpty(progData)) return;

            var originContentDir = Path.Combine(progData, "Origin", "LocalContent");
            if (!Directory.Exists(originContentDir)) return;

            try
            {
                foreach (var mfstFile in Directory.GetFiles(originContentDir, "*.mfst", SearchOption.AllDirectories))
                {
                    var idName = Path.GetFileNameWithoutExtension(mfstFile);
                    var entry = new ApplicationUninstallerEntry
                    {
                        DisplayName = idName,
                        Publisher = "Electronic Arts",
                        UninstallerKind = UninstallerType.Origin,
                        UninstallString = $"\"origin://uninstall/{idName}\"",
                        QuietUninstallString = $"\"origin://uninstall/{idName}\"",
                        UninstallPossible = true,
                        QuietUninstallPossible = true,
                        IsRegistered = true,
                        RatingId = $"EA_{idName}"
                    };
                    results.Add(entry);
                }
            }
            catch { }
        }

        private static void ScanBattleNet(List<ApplicationUninstallerEntry> results)
        {
            var bnetRoot = @"HKEY_LOCAL_MACHINE\Software\WOW6432Node\Blizzard Entertainment";
            using var key = SafeRegistryEngine.OpenKey(bnetRoot);
            if (key == null) return;

            foreach (var sub in key.GetSubKeyNames())
            {
                using var subKey = key.OpenSubKey(sub);
                var path = subKey?.GetValue("InstallPath")?.ToString();
                if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
                {
                    var entry = new ApplicationUninstallerEntry
                    {
                        DisplayName = sub,
                        Publisher = "Blizzard Entertainment",
                        InstallLocation = path,
                        UninstallerKind = UninstallerType.SimpleDelete,
                        UninstallPossible = true,
                        IsRegistered = true,
                        RatingId = $"BattleNet_{sub}"
                    };
                    results.Add(entry);
                }
            }
        }
    }
}
