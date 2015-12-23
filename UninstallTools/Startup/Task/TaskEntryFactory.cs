using System.Collections.Generic;
using System.Xml.Linq;
using Klocman.Tools;
using Microsoft.Win32.TaskScheduler;

namespace UninstallTools.Startup.Task
{
    public static class TaskEntryFactory
    {
        public static IEnumerable<TaskEntry> GetTaskStartupEntries()
        {
            TaskCollection tasks;
            try { tasks = TaskService.Instance.RootFolder.Tasks; }
            catch { yield break; }

            foreach (var task in tasks)
            {
                XNamespace xmlNamespace;
                XElement actionRoot;

                try
                {
                    var rootElement = XDocument.Parse(task.Xml).Root;
                    xmlNamespace = rootElement?.Name.Namespace ?? XNamespace.None;
                    actionRoot = rootElement?.Element(xmlNamespace + "Actions");
                }
                catch
                {
                    continue;
                }

                if (actionRoot == null || actionRoot.IsEmpty || xmlNamespace == XNamespace.None) continue;

                foreach (var actionElement in actionRoot.Elements())
                {
                    var command = actionElement.Element(xmlNamespace + "Command");

                    if (string.IsNullOrEmpty(command?.Value)) continue;

                    var arguments = actionElement.Element(xmlNamespace + "Arguments");
                    var cmdCommand = new ProcessStartCommand(command.Value, arguments?.Value ?? string.Empty);

                    yield return new TaskEntry(task.Name, cmdCommand.ToCommandLine(), cmdCommand.FileName, task);
                }
            }
        }
    }
}