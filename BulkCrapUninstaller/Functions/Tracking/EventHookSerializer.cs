using System.Collections.Generic;
using System.Xml.Linq;
using Klocman.Extensions;

namespace BulkCrapUninstaller.Functions.Tracking
{
    internal static class EventHookSerializer
    {
        public static XDocument SerializeToXml(IEnumerable<EventHook> hooks, XDocument baseDocument)
        {
            var root = baseDocument.Root.GetOrCreateElement("InterfaceStatistics");
            foreach (var mainHook in hooks)
            {
                var parentElement = root.GetOrCreateElement(mainHook.ParentName);
                var fieldElement = parentElement.GetOrCreateElement(mainHook.FieldName);

                foreach (var singleHook in mainHook.Hooks)
                {
                    if (singleHook.HitCount <= 0)
                        continue;

                    var element = fieldElement.GetOrCreateElement(singleHook.EventName);
                    int result;
                    int.TryParse(element.Value, out result);
                    element.SetValue(singleHook.HitCount + result);
                }

                if (fieldElement.IsEmpty)
                    fieldElement.Remove();
            }

            return baseDocument;
        }
    }
}