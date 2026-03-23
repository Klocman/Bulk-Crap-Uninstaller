/*
    Copyright (c) 2018 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Klocman.Extensions;
using Klocman.Forms.Tools;

namespace UninstallTools.Factory
{
    internal static class FactoryTools
    {
        private static readonly TimeSpan HelperTimeout = TimeSpan.FromSeconds(45);

        /// <summary>
        /// Warning: only use with helpers that output unicode and use 0 as success return code.
        /// </summary>
        internal static string StartHelperAndReadOutput(string filename, string args)
        {
            if (!File.Exists(filename)) return null;

            var sw = Stopwatch.StartNew();
            using (var process = Process.Start(new ProcessStartInfo(filename, args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.Unicode
            }))
            {
                try
                {
                    if (process == null)
                    {
                        Trace.WriteLine($"[Factory] Failed to start helper process {filename} {args}");
                        return null;
                    }

                    var outputTask = process.StandardOutput.ReadToEndAsync();
                    var errorTask = process.StandardError.ReadToEndAsync();

                    if (!process.WaitForExit((int)HelperTimeout.TotalMilliseconds))
                    {
                        try
                        {
                            process.Kill(true);
                        }
                        catch
                        {
                            // Best effort only.
                        }

                        Trace.WriteLine($"[Factory] Helper {filename} timed out after {HelperTimeout.TotalSeconds:F0}s ({args})");
                        return null;
                    }

                    Task.WaitAll(new[] { outputTask, errorTask }, HelperTimeout);

                    var output = outputTask.Result;
                    var error = errorTask.Result;
                    Trace.WriteLine($"[Performance] Running command {filename} {args} took {sw.ElapsedMilliseconds}ms");

                    if (process.ExitCode != 0)
                    {
                        Trace.WriteLine($"[Factory] Helper {filename} exited with code {process.ExitCode}. stderr={error}");
                        return null;
                    }

                    if (!string.IsNullOrWhiteSpace(error))
                        Trace.WriteLine($"[Factory] Helper {filename} produced stderr output: {error}");

                    return output;
                }
                catch (Win32Exception ex)
                {
                    PremadeDialogs.GenericError(ex);
                    return null;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"[Factory] Failed while running helper {filename} {args} - {ex}");
                    return null;
                }
            }
        }

        internal static IEnumerable<Dictionary<string, string>> ExtractAppDataSetsFromHelperOutput(string helperOutput)
        {
            ICollection<string> allParts = helperOutput.SplitNewlines(StringSplitOptions.None);
            while (allParts.Count > 0)
            {
                var singleAppParts = allParts.TakeWhile(x => !String.IsNullOrEmpty(x)).ToList();
                allParts = allParts.Skip(singleAppParts.Count + 1).ToList();

                if (!singleAppParts.Any())
                    continue;

                var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var part in singleAppParts)
                {
                    var separatorIndex = part.IndexOf(':');
                    if (separatorIndex <= 0)
                        continue;

                    var key = part.Substring(0, separatorIndex).Trim();
                    if (string.IsNullOrEmpty(key))
                        continue;

                    // Some helper outputs may contain duplicate labels. Keep the latest value.
                    var value = part.Substring(separatorIndex + 1).Trim();
                    values[key] = value;
                }

                if (values.Count > 0)
                    yield return values;
            }
        }

        internal static bool CheckIsValid(ApplicationUninstallerEntry target, IEnumerable<Guid> msiProducts)
        {
            if (string.IsNullOrEmpty(target.UninstallerFullFilename))
                return false;

            bool isPathRooted;
            try
            {
                isPathRooted = Path.IsPathRooted(target.UninstallerFullFilename);
            }
            catch (ArgumentException)
            {
                isPathRooted = false;
            }

            if (isPathRooted && File.Exists(target.UninstallerFullFilename))
                return true;

            if (target.UninstallerKind == UninstallerType.Msiexec)
                return msiProducts.Contains(target.BundleProviderKey);

            return !isPathRooted;
        }
    }
}
