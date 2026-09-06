/*
 * EBUninstaller Pro - Application Uninstaller & System Optimization Suite
 * HelperTools Shared Utilities Subsystem
 * Copyright (C) 2026 EBUninstaller Development Team & Contributors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Klocman
{
    /// <summary>
    /// Result of a spawned process execution.
    /// </summary>
    internal sealed class ProcessExecutionResult
    {
        public int ExitCode { get; set; }
        public string StandardOutput { get; set; } = string.Empty;
        public string StandardError { get; set; } = string.Empty;
        public bool TimedOut { get; set; }
        public TimeSpan ExecutionDuration { get; set; }
        public bool Success => !TimedOut && ExitCode == 0;
    }

    /// <summary>
    /// Resilient process runner with structured output capture, timeout protection, and process tree cleanup.
    /// </summary>
    internal static class ProcessRunner
    {
        /// <summary>
        /// Executes a process synchronously and captures its standard output and error streams.
        /// </summary>
        public static ProcessExecutionResult Run(
            string fileName,
            string arguments = "",
            string workingDirectory = null,
            int timeoutMilliseconds = 60000,
            bool runAsAdmin = false)
        {
            var sw = Stopwatch.StartNew();
            var result = new ProcessExecutionResult();

            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments ?? string.Empty,
                WorkingDirectory = workingDirectory ?? Path.GetDirectoryName(fileName) ?? Environment.CurrentDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            if (runAsAdmin)
            {
                psi.UseShellExecute = true;
                psi.RedirectStandardOutput = false;
                psi.RedirectStandardError = false;
                psi.Verb = "runas";
            }

            try
            {
                using var process = new Process { StartInfo = psi };
                var stdout = new StringBuilder();
                var stderr = new StringBuilder();

                if (!psi.UseShellExecute)
                {
                    process.OutputDataReceived += (s, e) => { if (e.Data != null) stdout.AppendLine(e.Data); };
                    process.ErrorDataReceived += (s, e) => { if (e.Data != null) stderr.AppendLine(e.Data); };
                }

                process.Start();

                if (!psi.UseShellExecute)
                {
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }

                if (process.WaitForExit(timeoutMilliseconds))
                {
                    result.ExitCode = process.ExitCode;
                }
                else
                {
                    result.TimedOut = true;
                    result.ExitCode = -1;
                    KillProcessTree(process.Id);
                }

                result.StandardOutput = stdout.ToString();
                result.StandardError = stderr.ToString();
            }
            catch (Exception ex)
            {
                result.ExitCode = -1;
                result.StandardError = ex.Message;
            }
            finally
            {
                sw.Stop();
                result.ExecutionDuration = sw.Elapsed;
            }

            return result;
        }

        /// <summary>
        /// Executes a process asynchronously with cancellation support.
        /// </summary>
        public static async Task<ProcessExecutionResult> RunAsync(
            string fileName,
            string arguments = "",
            string workingDirectory = null,
            int timeoutMilliseconds = 60000,
            CancellationToken cancellationToken = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeoutMilliseconds);

            return await Task.Run(() => Run(fileName, arguments, workingDirectory, timeoutMilliseconds), cts.Token);
        }

        private static void KillProcessTree(int pid)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "taskkill.exe",
                    Arguments = $"/F /T /PID {pid}",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                using var p = Process.Start(psi);
                p?.WaitForExit(5000);
            }
            catch { }
        }
    }
}
