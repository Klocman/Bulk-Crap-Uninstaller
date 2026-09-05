/*
    EBUninstaller Pro - Developer & Build Artifact Cache Cleaner
    Detection, sizing, and safe purging of development package caches (NuGet, npm, pip, Cargo, Gradle, Go, etc.).
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public enum DevToolEcosystem
    {
        DotNetNuGet,
        NodeNpmYarnPnpm,
        PythonPipConda,
        RustCargo,
        JavaGradleMaven,
        Golang,
        VisualStudioAndCpp,
        Other
    }

    public class DevCacheLocationItem
    {
        public string EcosystemName { get; set; } = string.Empty;
        public DevToolEcosystem Ecosystem { get; set; }
        public string Description { get; set; } = string.Empty;
        public string DirectoryPath { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public int FilesCount { get; set; }
        public bool Exists { get; set; }
        public bool IsSelected { get; set; } = true;
    }

    public static class DeveloperCacheCleaner
    {
        public static List<DevCacheLocationItem> ScanDeveloperCaches(Action<string>? onProgress = null)
        {
            var results = new List<DevCacheLocationItem>();
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var targets = new List<(string name, DevToolEcosystem eco, string desc, string path)>
            {
                // .NET / NuGet
                (".NET Global NuGet Packages", DevToolEcosystem.DotNetNuGet, "Global cached NuGet packages used for builds", Path.Combine(userProfile, ".nuget", "packages")),
                ("NuGet Local V3 HTTP Cache", DevToolEcosystem.DotNetNuGet, "Downloaded NuGet package archive caches", Path.Combine(localAppData, "NuGet", "v3-cache")),
                ("NuGet Plugins & Scratch Cache", DevToolEcosystem.DotNetNuGet, "Scratch and plugin staging caches", Path.Combine(localAppData, "NuGet", "plugins-cache")),

                // Node / JavaScript
                ("npm Global Cache", DevToolEcosystem.NodeNpmYarnPnpm, "Cached tarballs and git repos for npm", Path.Combine(localAppData, "npm-cache")),
                ("Yarn Cache", DevToolEcosystem.NodeNpmYarnPnpm, "Global yarn package caches", Path.Combine(localAppData, "Yarn", "Cache")),
                ("pnpm Store Cache", DevToolEcosystem.NodeNpmYarnPnpm, "Content-addressable pnpm store cache", Path.Combine(localAppData, "pnpm", "store")),
                ("Electron / Node-Gyp Staging", DevToolEcosystem.NodeNpmYarnPnpm, "Downloaded header files and build artifacts", Path.Combine(userProfile, ".electron")),

                // Python
                ("pip Download Cache", DevToolEcosystem.PythonPipConda, "Cached python wheels and source packages", Path.Combine(localAppData, "pip", "cache")),
                ("Conda Package Cache", DevToolEcosystem.PythonPipConda, "Downloaded conda tarballs and packages", Path.Combine(userProfile, ".conda", "pkgs")),

                // Rust / Cargo
                ("Cargo Package Registry Cache", DevToolEcosystem.RustCargo, "Downloaded crates.io .crate archives", Path.Combine(userProfile, ".cargo", "registry", "cache")),
                ("Cargo Git Checkout Cache", DevToolEcosystem.RustCargo, "Cached git repositories cloned by Cargo", Path.Combine(userProfile, ".cargo", "git", "db")),

                // Java / Gradle / Maven
                ("Gradle Artifact Caches", DevToolEcosystem.JavaGradleMaven, "Downloaded Gradle dependencies and wrappers", Path.Combine(userProfile, ".gradle", "caches")),
                ("Maven Repository Cache", DevToolEcosystem.JavaGradleMaven, "Downloaded Maven .jar artifacts", Path.Combine(userProfile, ".m2", "repository")),

                // Golang
                ("Go Module Download Cache", DevToolEcosystem.Golang, "Downloaded Go modules source archives", Path.Combine(userProfile, "go", "pkg", "mod", "cache")),
                ("Go Build Object Cache", DevToolEcosystem.Golang, "Incremental Go build artifact cache", Path.Combine(localAppData, "go-build")),

                // Visual Studio / C++
                ("Visual Studio Diagnostic Caches", DevToolEcosystem.VisualStudioAndCpp, "IntelliSense and symbol cache leftovers", Path.Combine(localAppData, "Microsoft", "VisualStudio", "Roslyn", "SourceLinkCache")),
                ("MSYS2 Pacman Package Cache", DevToolEcosystem.VisualStudioAndCpp, "Cached MinGW/MSYS2 tarballs", @"C:\msys64\var\cache\pacman\pkg")
            };

            foreach (var t in targets)
            {
                try
                {
                    onProgress?.Invoke($"Scanning {t.name}...");
                    bool exists = Directory.Exists(t.path);
                    long size = 0;
                    int count = 0;

                    if (exists)
                    {
                        var di = new DirectoryInfo(t.path);
                        var files = di.EnumerateFiles("*", SearchOption.AllDirectories);
                        foreach (var f in files)
                        {
                            try
                            {
                                size += f.Length;
                                count++;
                            }
                            catch { }
                        }
                    }

                    results.Add(new DevCacheLocationItem
                    {
                        EcosystemName = t.name,
                        Ecosystem = t.eco,
                        Description = t.desc,
                        DirectoryPath = t.path,
                        SizeBytes = size,
                        FilesCount = count,
                        Exists = exists,
                        IsSelected = size > 0
                    });
                }
                catch (Exception ex)
                {
                    StructuredLogger.Log(LogLevel.Warning, "DeveloperCacheCleaner", $"Failed to scan cache {t.name}: {ex.Message}");
                }
            }

            return results;
        }

        public static (int cleanedCount, long freedBytes) PurgeDeveloperCaches(IEnumerable<DevCacheLocationItem> targets, Action<string>? onProgress = null)
        {
            int cleanedCount = 0;
            long freedBytes = 0;

            foreach (var target in targets)
            {
                if (SecurityGuard.IsPathProtected(target.DirectoryPath))
                    continue;

                if (!Directory.Exists(target.DirectoryPath))
                    continue;

                try
                {
                    onProgress?.Invoke($"Purging {target.EcosystemName}...");
                    var di = new DirectoryInfo(target.DirectoryPath);
                    foreach (var f in di.EnumerateFiles("*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            long sz = f.Length;
                            f.Delete();
                            freedBytes += sz;
                            cleanedCount++;
                        }
                        catch { }
                    }

                    // Remove empty subdirectories
                    foreach (var subDir in di.EnumerateDirectories())
                    {
                        try { subDir.Delete(true); } catch { }
                    }

                    StructuredLogger.Log(LogLevel.Info, "DeveloperCacheCleaner", $"Purged developer cache {target.EcosystemName}");
                }
                catch (Exception ex)
                {
                    StructuredLogger.Log(LogLevel.Error, "DeveloperCacheCleaner", $"Failed to purge {target.DirectoryPath}: {ex.Message}");
                }
            }

            return (cleanedCount, freedBytes);
        }
    }
}
