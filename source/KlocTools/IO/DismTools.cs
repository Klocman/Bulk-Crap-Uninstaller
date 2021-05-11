/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Klocman.Native;
using Klocman.Tools;

namespace Klocman.IO
{
    public static class DismTools
    {
        public static readonly string DismFullPath = Path.Combine(WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_SYSTEM),
            "Dism.exe");

        private static bool? _dismIsAvailable;

        public static bool DismIsAvailable
        {
            get
            {
                if (!_dismIsAvailable.HasValue)
                {
                    _dismIsAvailable = File.Exists(DismFullPath);
                }
                return _dismIsAvailable.Value;
            }
        }

        /// <summary>
        /// Get a list of feature names and their status (true for enabled)
        /// </summary>
        public static IEnumerable<KeyValuePair<string, bool>> GetWindowsFeatures()
        {
            var output = RunDismCommand("/english /format:list /online /get-features");

            var results = new List<KeyValuePair<string, bool>>();
            string storedFeatureName = null;

            foreach (var line in output.Split(StringTools.NewLineChars.ToArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.StartsWith("Feature Name", StringComparison.Ordinal))
                {
                    storedFeatureName = line.Substring(line.IndexOf(':') + 2);
                }
                else if (line.StartsWith("State", StringComparison.Ordinal))
                {
                    if (storedFeatureName == null)
                        throw new InvalidDataException("Dism output has invalid format");

                    var isEnabled = line.Substring(line.IndexOf(':') + 1)
                        .TrimStart()
                        .Equals("Enabled", StringComparison.InvariantCultureIgnoreCase);
                    results.Add(new KeyValuePair<string, bool>(storedFeatureName, isEnabled));
                    storedFeatureName = null;
                }
            }

            return results;
        }

        private static string RunDismCommand(string command)
        {
            var psi = new ProcessStartInfo(DismFullPath, command)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.GetEncoding(850)
            };

            var process = Process.Start(psi);

            Debug.Assert(process != null);
            var output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                var code = process.ExitCode.ToString();
                var index = output.IndexOf(code, StringComparison.InvariantCulture);
                throw new IOException("Dism returned error code " + code,
                    new Exception(index > 0 ? output.Substring(index + code.Length).Trim() : output));
            }
            return output;
        }

        public static WindowsFeatureInfo GetFeatureInfo(string featureName)
        {
            var output = RunDismCommand("/english /format:list /online /get-featureinfo /featurename=" +
                                        '\"' + featureName + '\"');

            return FromDismOutput(output);
        }

        public static string GetDismUninstallString(string featureName, bool silent)
        {
            return string.Format("Dism.exe /norestart {1}/online /disable-feature /featurename=\"{0}\"",
                featureName, silent ? "/quiet " : string.Empty);
        }

        private static WindowsFeatureInfo FromDismOutput(string output)
        {
            var result = new WindowsFeatureInfo();

            foreach (var line in output
                .Split(StringTools.NewLineChars.ToArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                if (!line.Contains(':'))
                    continue;

                var data = line.Substring(line.IndexOf(':') + 1).TrimStart();

                if (line.StartsWith("Feature Name", StringComparison.Ordinal))
                    result.FeatureName = data;
                else if (line.StartsWith("State", StringComparison.Ordinal))
                    result.Enabled = data.Equals("Enabled", StringComparison.InvariantCultureIgnoreCase);
                else if (line.StartsWith("Display Name", StringComparison.Ordinal))
                    result.DisplayName = data;
                else if (line.StartsWith("Restart Required", StringComparison.Ordinal))
                    result.RestartRequired = data;
                else if (line.StartsWith("Description", StringComparison.Ordinal))
                    result.Description = data;
            }

            return string.IsNullOrEmpty(result.FeatureName) ? null : result;
        }
    }
}