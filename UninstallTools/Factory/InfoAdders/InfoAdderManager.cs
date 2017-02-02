/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UninstallTools.Factory.InfoAdders
{
    internal class InfoAdderManager
    {
        private readonly IMissingInfoAdder[] _infoAdders;

        public InfoAdderManager()
        {
            _infoAdders = GetInfoAdders().ToArray();
        }

        private static IEnumerable<IMissingInfoAdder> GetInfoAdders()
        {
            var type = typeof(IMissingInfoAdder);
            var types = Assembly.GetExecutingAssembly().GetTypes() //AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);

            var instances = types.Select(Activator.CreateInstance);

            return instances.Cast<IMissingInfoAdder>().OrderByDescending(x=>x.Priority);
        }

        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            // TODO
            /*
             create copy list of adders
             find adder that uses any values that are present in target
                prioritize ones with all values existing
                // run in priority buckets? don't run lower priority if possible
                try making reflection prop gets fast
                compare to default value (string isempty)
             execute it and remove it from the list
             continue until nothing can execute or list is empty
             */

            throw new NotImplementedException();
        }

        public void TryAddFieldInformation(ApplicationUninstallerEntry target, string targetValueName)
        {
            // TODO
            /*
             create copy list of adders
             similar logic to above
             try running whatever can add targetValueName
                if can't run any, check if any other adders can fill in the required values and try again
             */

            throw new NotImplementedException();
        }
    }
}
