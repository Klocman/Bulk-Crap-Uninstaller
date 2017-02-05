/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UninstallTools.Factory.InfoAdders
{
    internal class InfoAdderManager
    {
        private static readonly IMissingInfoAdder[] InfoAdders;

        private static readonly Dictionary<string, PropInfo> TargetProperties;

        private sealed class PropInfo
        {
            public PropInfo(PropertyInfo propertyInfo, object defaultValue)
            {
                PropertyInfo = propertyInfo;
                DefaultValue = defaultValue;
            }

            public PropertyInfo PropertyInfo { get; }
            public object DefaultValue { get; }
        }

        static InfoAdderManager()
        {
            InfoAdders = GetInfoAdders().ToArray();

            var defaultValues = new ApplicationUninstallerEntry();
            TargetProperties = typeof(ApplicationUninstallerEntry)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                // Skip properties without public setters
                .Where(x => x.GetSetMethod(true) != null)
                .Where(x => IsTypeValid(x.PropertyType))
                .ToDictionary(x => x.Name, x => new PropInfo(x, x.GetValue(defaultValues, null)));
        }

        private static readonly Type BoolType = typeof(bool);

        /// <summary>
        /// Check if we can correctly detect if the type has no value.
        /// </summary>
        private static bool IsTypeValid(Type type)
        {
            // TODO is this filtering neccessary?
            return type != BoolType || Nullable.GetUnderlyingType(type) != null;
        }

        private static IEnumerable<IMissingInfoAdder> GetInfoAdders()
        {
            var type = typeof(IMissingInfoAdder);
            var types = Assembly.GetExecutingAssembly().GetTypes() //AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);

            var instances = types.Select(Activator.CreateInstance);

            return instances.Cast<IMissingInfoAdder>().OrderByDescending(x => x.Priority);
        }

        /// <summary>
        /// Fill in information using all detected IMissingInfoAdder classes
        /// </summary>
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            var adders = InfoAdders.ToList();
            var valueIsDefaultCache = new Dictionary<string, bool>();

            // Checks if the value is default, buffering the result
            Func<string, bool> getIsValDefault = key =>
            {
                bool valIsDefault;
                if (!valueIsDefaultCache.TryGetValue(key, out valIsDefault))
                {
                    // If we can't check if the value is default, assume that it is to be safe
                    if (!TargetProperties.ContainsKey(key)) return true;

                    var property = TargetProperties[key];
                    valIsDefault = Equals(property.PropertyInfo.GetValue(target, null),
                        property.DefaultValue);
                    valueIsDefaultCache.Add(key, valIsDefault);
                }

                return valIsDefault;
            };

            bool anyRan;
            do
            {
                anyRan = false;
                foreach (var infoAdder in adders.ToList())
                {
                    var requirements = infoAdder.RequiredValueNames;

                    //TODO prioritize ones with all values existing from same priority tier?
                    // Basically Any / All

                    // Always run the adder if it has no requirements
                    if (requirements.Any())
                    {
                        if (infoAdder.RequiresAllValues)
                        {
                            if(infoAdder.RequiredValueNames.Any(x => getIsValDefault(x)))
                                continue;
                        }
                        else
                        {
                            if (infoAdder.RequiredValueNames.All(x => getIsValDefault(x)))
                                continue;
                        }
                    }

                    // Only run the adder if it can actually fill in any missing values
                    if (!infoAdder.AlwaysRun && !infoAdder.CanProduceValueNames.Any(getIsValDefault))
                        continue;

                    infoAdder.AddMissingInformation(target);

                    // Remove items that might have changed from cache
                    foreach (var valueName in infoAdder.CanProduceValueNames)
                        valueIsDefaultCache.Remove(valueName);

                    adders.Remove(infoAdder);

                    anyRan = true;
                }
            } while (anyRan);
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

        /// <summary>
        /// Copy missing property values
        /// </summary>
        /// <param name="target">Copy values to this object</param>
        /// <param name="source">Copy from this object</param>
        public void CopyMissingInformation(ApplicationUninstallerEntry target, ApplicationUninstallerEntry source)
        {
            foreach (var property in TargetProperties.Values)
            {
                // Skip if target has non-default value assigned to this property
                if (!Equals(property.PropertyInfo.GetValue(target, null), property.DefaultValue))
                    continue;

                // If source has a non-default value for this property, copy it to target
                var newValue = property.PropertyInfo.GetValue(source, null);
                if (!Equals(newValue, property.DefaultValue))
                    property.PropertyInfo.SetValue(target, newValue, null);
            }
        }
    }
}
