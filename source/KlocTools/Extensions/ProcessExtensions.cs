/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;

namespace Klocman.Extensions
{
    public static class ProcessExtensions
    {
        public static IEnumerable<Process> GetChildProcesses(this Process process)
        {
            if (process == null) throw new ArgumentNullException(nameof(process));

            var results = new List<Process>();
            try
            {
                var searchString = $"Select * From Win32_Process Where ParentProcessID={process.Id}";
                using (var mos = new ManagementObjectSearcher(searchString))
                {
                    foreach (var mo in mos.Get())
                    {
                        //var mo = (ManagementObject) o;
                        Process resultProcess = null;
                        try
                        {
                            resultProcess = Process.GetProcessById(Convert.ToInt32(mo["ProcessID"]));
                        }
                        catch (ArgumentException)
                        {
                            // Process exited by now
                        }
            
                        if (resultProcess != null)
                            results.Add(resultProcess);
                    }
                }
            }
            catch (Exception e1)
            {
               Console.WriteLine(@"Failed to GetChildProcesses using ManagementObjectSearcher: " + e1.Message);

                try
                {
                    var allProcesses = Process.GetProcesses()
                        .Attempt(proc => new { proc, parent = ParentProcessUtilities.GetParentProcess(proc.Handle) })
                        .Where(x => x.parent != null)
                        .ToList();

                    var newChildren = allProcesses.Where(p => p.parent == process).Select(x => x.proc).ToList();
                    while (newChildren.Any())
                    {
                        results.AddRange(newChildren);
                        newChildren = allProcesses.Where(p => newChildren.Contains(p.parent)).Select(x => x.proc).ToList();
                    }
                }
                catch (Exception e2)
                {
                    Console.WriteLine(@"Failed to GetChildProcesses using ParentProcessUtilities: " + e2);
                }
            }

            return results.Distinct();
        }

        public static string GetCommandLine(this Process process)
        {
            if (process == null) throw new ArgumentNullException(nameof(process));
            if (process.MainModule?.FileName == null) throw new ArgumentException("null MainModule");

            var commandLine = new StringBuilder(process.MainModule.FileName);

            commandLine.Append(' ');
            using (var searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
            {
                foreach (var @object in searcher.Get())
                {
                    commandLine.Append(@object["CommandLine"]);
                    commandLine.Append(' ');
                }
            }

            return commandLine.ToString();
        }

        /// <summary>
        ///     Stop the proces and optionally all of its child processes immidiately. Only the main process can throw exceptions.
        /// </summary>
        public static void Kill(this Process pr, bool killChildren)
        {
            if (killChildren)
            {
                foreach (var cp in pr.GetChildProcesses())
                {
                    try
                    {
                        cp.Kill();
                    }
                    catch
                    {
                        // Ignore failures, the process probably ended
                    }
                }
            }
            pr.Kill();
        }

        public static Process Start(this ProcessStartInfo startInfo)
        {
            return Process.Start(startInfo);
        }

        /// <summary>
        ///     Start a new process using Process.Start,
        ///     but don't return until this process and all of its child processes end.
        /// </summary>
        /// <returns>Exit code returned by the main process</returns>
        public static int StartAndWait(this ProcessStartInfo startInfo)
        {
            var uninstaller = Process.Start(startInfo);
            if (uninstaller == null) return -1;
            uninstaller.WaitForExit();
            while (true)
            {
                var children = uninstaller.GetChildProcesses();
                var processes = children as IList<Process> ?? children.ToList();
                if (processes.Any())
                    processes.First().WaitForExit(1000);
                else
                    break;
            }
            return uninstaller.ExitCode;
        }
    }
}